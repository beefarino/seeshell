using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using CodeOwls.SeeShell.Common.Attributes;
using CodeOwls.SeeShell.Common.Exceptions;

namespace CodeOwls.SeeShell.Common.DataSources
{
    public class DynamicMemberFactory
    {
        private static readonly Log Log = new Log(typeof (DynamicMemberFactory));
        private readonly IPowerShellDataSource _dataSource;
        private readonly IEnumerable<DynamicMemberSpecification> _spec;
        private readonly PSObject _dataItem;

        public DynamicMemberFactory( IPowerShellDataSource dataSource, IEnumerable<DynamicMemberSpecification> spec, PSObject dataItem )
        {
            _dataSource = dataSource;
            _spec = spec;
            _dataItem = dataItem;
        }

        public IEnumerable<DynamicMemberDescriptor> CreateDynamicMembers()
        {
            using(Log.PushContext( "CreateDynamicMembers"))
            {
                List<DynamicMemberDescriptor> descriptors = new List<DynamicMemberDescriptor>();

                try
                {
                    using (DefaultRunspaceManager.ForCurrentThread)
                    {
                        foreach (var spec in _spec)
                        {
                            var specDescriptors = CreateDynamicMembers(spec, _dataItem);
                            descriptors.AddRange(specDescriptors);
                        }
                    }
                }
                catch( Exception ei )
                {
                    Log.Error( "An exception was raised while creating dynamic members", ei );
                    throw;
                }

                return descriptors.Where(l => l != null).ToList();
            }
        }

        private void CreateDynamicMemberProperty(ref PSPropertyInfo plotMember, string name, string proxyPropertyName)
        {
            if (String.IsNullOrEmpty(proxyPropertyName))
            {
                return;
            }

            var pm = plotMember as PSScriptProperty;
            var script = String.Format("$this.'{0}' | foreach-object  {{ {1} }}", proxyPropertyName, pm.GetterScript);
            plotMember = new PSScriptProperty(name, System.Management.Automation.ScriptBlock.Create(script));
        }

        private PSScriptProperty CreateDynamicMemberProperty(string memberName, string proxyPropertyName)
        {
            if (null == proxyPropertyName)
            {
                return null;
            }
            var newMemberName = proxyPropertyName + "_" + memberName;
            const string scriptFormat = "$this.'{1}'.'{0}'";

            var script = String.Format(scriptFormat, memberName, proxyPropertyName);
            return new PSScriptProperty(newMemberName, System.Management.Automation.ScriptBlock.Create(script));
        }

        IEnumerable<DynamicMemberDescriptor> AddWildcardDynamicMembers(DynamicMemberSpecification spec, WildcardPattern pattern, PSObject ps, string proxyPropertyName)
        {
            var props = ps.Properties;
            var scriptFormat = "$this.'{0}'";
            if (null != proxyPropertyName)
            {
                props = ps.Properties[proxyPropertyName].ToPSObject().Properties;
                scriptFormat = "$this.'{1}'.'{0}'";
            }
            var matchingPropertyNames = from prop in props
                                        where pattern.IsMatch(prop.Name)
                                        select prop.Name;


            var members = matchingPropertyNames.ToList().ConvertAll(
                s => new
                {
                    PropertyName = s,
                    Member = new PSScriptProperty(
                         (proxyPropertyName ?? "" ) + "_" + s,
                         System.Management.Automation.ScriptBlock.Create(String.Format(scriptFormat, s,
                                                                                       proxyPropertyName))
                         )
                });
            return (from m in members
                    let s = (from sd in spec.ScaleDescriptors
                                 where sd.Key.IsMatch(m.PropertyName)
                                 select sd.Value).FirstOrDefault()
                    select new DynamicMemberDescriptor(m.Member, s)).ToList();
        }

        private IEnumerable<DynamicMemberDescriptor> CreateDynamicMembers(DynamicMemberSpecification spec, PSObject ps)
        {
            using (Log.PushContext("CreateDynamicMembers"))
            {
                List<DynamicMemberDescriptor> descriptors = new List<DynamicMemberDescriptor>();
                if (null != spec.AcrossSpecifier)
                {
                    descriptors.Add(new DynamicMemberDescriptor(spec.AcrossSpecifier, null));
                }

                var byItemMembers = CreateByItemMemberDescriptors(spec, ps).ToList();
                if (byItemMembers.Any())
                {
                    descriptors.AddRange(byItemMembers);

                    foreach (var byItemMember in byItemMembers)
                    {
                        descriptors.AddRange(CreateMemberDescriptors(spec, ps, byItemMember));
                    }
                }
                else
                {
                    descriptors.AddRange(CreateMemberDescriptors(spec, ps, null));
                }
                return descriptors;
            }
        }

        private IEnumerable<DynamicMemberDescriptor> CreateMemberDescriptors(DynamicMemberSpecification spec, PSObject ps, DynamicMemberDescriptor byItemMember)
        {
            var list = new List<DynamicMemberDescriptor>();
            PSScriptProperty scriptProperty = null;
            
            var plotItemMembers = UpdatePlotItemMembers(spec, ps, byItemMember);
            
            list.AddRange( plotItemMembers );
            list.Add( UpdateAgainstItemMember(spec, ps, byItemMember) );
            return list;
        }

        private DynamicMemberDescriptor UpdateAgainstItemMember(DynamicMemberSpecification spec, PSObject ps, DynamicMemberDescriptor byItemMember)
        {
            if (null == spec.AgainstItem)
            {
                return null;

            }
            var againstMember = GetMemberDescriptorForSpecItem(spec, ps, spec.AgainstItem, byItemMember).FirstOrDefault();
                if (null == againstMember)
                {
                    //todo
                    return null;
                }

                spec.AgainstProperty = againstMember.MemberInfo;
                //SafeAddDynamicMember(againstMember.MemberInfo);
                return new DynamicMemberDescriptor( againstMember.MemberInfo, null );
            
        }

        private IEnumerable<DynamicMemberDescriptor> UpdatePlotItemMembers(DynamicMemberSpecification spec, PSObject ps, DynamicMemberDescriptor byItemMember)
        {
            var plotItemMembers = CreatePlotItemMembers(byItemMember, spec, ps);
            if (null == plotItemMembers || !plotItemMembers.Any())
            {
                return new List<DynamicMemberDescriptor>();
            }

            foreach (var item in plotItemMembers.Where(a=>null != a && null !=a.MemberInfo))
            {
                Log.DebugFormat( "adding plot property [{0}]", item.MemberInfo );
                spec.PlotItemDescriptors.Add(item);
            }

            return plotItemMembers
                .Where( pm => null != pm && null != pm.MemberInfo && ! String.IsNullOrEmpty( pm.MemberInfo.Name))
                .Where(pm => !ps.Properties.Match(pm.MemberInfo.Name).Any())
                .ToList();
        }

        private IEnumerable<DynamicMemberDescriptor> CreatePlotItemMembers(DynamicMemberDescriptor byItemMember, DynamicMemberSpecification spec, PSObject ps)
        {
            List<DynamicMemberDescriptor> list = new List<DynamicMemberDescriptor>();
            bool acrossSpecifierAdded = false;
            bool bySpecifierAdded = false;
            if (null != spec.AcrossSpecifier)
            {
                acrossSpecifierAdded = ps.SafeAddDynamicProperty(spec.AcrossSpecifier);
            }

            if (null != byItemMember)
            {                
                bySpecifierAdded = ps.SafeAddDynamicProperty(byItemMember.MemberInfo);
            }

            foreach (var plot in spec.PlotItems)
            {
                foreach (var memberDescriptor in GetMemberDescriptorForSpecItem(spec, ps, plot, byItemMember))
                {
                    list.Add(memberDescriptor);
                }
            }

            if (bySpecifierAdded)
            {
                ps.Properties.Remove(byItemMember.MemberInfo.Name);
            }
            if (acrossSpecifierAdded)
            {
                ps.Properties.Remove(spec.AcrossSpecifier.Name);
            }
            return list;
        }

        private IEnumerable<DynamicMemberDescriptor> GetMemberDescriptorForSpecItem(DynamicMemberSpecification spec, PSObject ps, object specItem, DynamicMemberDescriptor byProperty)
        {
            var list = new List<DynamicMemberDescriptor>();
            var specMember = specItem as PSPropertyInfo;
            string proxyPropertyName = null;
            
            if( null != byProperty && null != byProperty.MemberInfo )
            {
                proxyPropertyName = byProperty.MemberInfo.Name;
            }

            if (null != specMember)
            {
                var name = specItem.ToString();
                
                var psp = specMember as PSScriptProperty;
                if (null != psp)
                {
                    name = ScriptBlockDynamicPropertyArgumentTransformationAttribute.GetReadableName(psp);
                }
                
                if (null != byProperty && null != byProperty.IndexValue)
                {
                    name = byProperty.IndexValue.ToString() + "\\" + name;
                    //name.Replace("$", "").Replace("{","").Replace("}","").Replace(".","");
                }

                CreateDynamicMemberProperty(ref specMember, "_" + Guid.NewGuid().ToString(), proxyPropertyName);
                IScaleDescriptor scale = new DynamicPropertyScaleDescriptor(_dataSource, specMember.Name);
                scale = (from sd in spec.ScaleDescriptors
                         where sd.Key.IsMatch(name)
                         select sd.Value).FirstOrDefault() ??
                        (from sd in spec.ScaleDescriptors
                         where sd.Key.IsMatch(specMember.Name)
                         select sd.Value).FirstOrDefault() ??                                                  
                        scale;

                spec.ScaleDescriptors.Add( new Regex( Regex.Escape(specMember.Name)), scale );
                //var assignment = AddDynamicScaleForProperty(specMember.Name);

                
                list.Add( new DynamicMemberDescriptor(specMember, scale, name) );
            }
            else if (specItem is string && WildcardPattern.ContainsWildcardCharacters(specItem.ToString()))
            {
                var p = new WildcardPattern(specItem.ToString());
                var members = AddWildcardDynamicMembers(spec, p, ps, proxyPropertyName);
                foreach (var member in members)
                {

                    list.Add(member);
                }
            }
            else
            {
                var name = specItem.ToString();
                if( null != byProperty && null != byProperty.IndexValue )
                {
                    name = byProperty.IndexValue.ToString() + "\\" + name;
                }

                if (null != proxyPropertyName)
                {
                    var m = CreateDynamicMemberProperty(specItem.ToString(), proxyPropertyName);
                    var scale = (from sd in spec.ScaleDescriptors
                                 where sd.Key.IsMatch(m.Name)
                                 select sd.Value).FirstOrDefault();
                    //var s = GetOrCreateScaleAssignment(name, m).Scale;
                    list.Add(new DynamicMemberDescriptor(m, scale, name));
                }
                else
                {
                    var scale = (from sd in spec.ScaleDescriptors
                                 where sd.Key.IsMatch(name)
                                 select sd.Value).FirstOrDefault();
                    if (null == ps.Properties[name])
                    {
                        //throw new DataPropertyNotFoundException(name);

                        list.Add(new DynamicMemberDescriptor( 
                            new PSScriptProperty( 
                                "_" + Guid.NewGuid().ToString("N") + "_" + name,
                                ScriptBlock.Create(String.Format("$local:val = $this.'{0}'; if( $local:val ) {{ $local:val }} else {{ 0 }}", specItem.ToString()))
                                ),
                            scale,
                            name
                            )
                        );
                    }
                    list.Add(new DynamicMemberDescriptor(ps.Properties[name],scale,name));
                }
            }

            return list;
        }

        private IEnumerable<DynamicMemberDescriptor> CreateByItemMemberDescriptors(DynamicMemberSpecification spec, PSObject ps)
        {
            if (null == spec.AcrossSpecifier)
            {
                return new DynamicMemberDescriptor[]{ };
            }
            var list = new List<DynamicMemberDescriptor>();
            var acrossAccessorName = spec.AcrossSpecifier.Name;
            ps.Properties.Add(spec.AcrossSpecifier);
            var sps = new PSObjectSolidifier().AsConcreteType(ps) as SolidPSObjectBase;
            object oitem = sps.GetPropValue<object>(acrossAccessorName);
            ps.Properties.Remove(acrossAccessorName);

            var items = oitem as ICollection;
            if (null == items)
            {
                items = new object[] { oitem };
            }

            if (null == spec.IndexSpecifier)
            {

                for (int i = 0; i < items.Count; ++i)
                {
                    var byItemScript = String.Format("$this.'{1}' | select-object -index '{0}'", i, acrossAccessorName);
                    var prop = new PSScriptProperty(
                        acrossAccessorName + i,
                        System.Management.Automation.ScriptBlock.Create(byItemScript));
                    var scale = ( from sd in spec.ScaleDescriptors
                                where sd.Key.IsMatch(prop.Name)
                                select sd.Value ).FirstOrDefault();
                    list.Add( new DynamicMemberDescriptor( prop, scale, "Index " + i ));
                }
            }
            else
            {
                foreach (var item in items)
                {
                    var pso = item.ToPSObject();
                    var tempProperty = new PSScriptProperty(
                        "_" + Guid.NewGuid().ToString("N"),
                        System.Management.Automation.ScriptBlock.Create(
                            "$this | " + spec.IndexSpecifier.GetterScript));
                    pso.Properties.Add(tempProperty);
                    var specValue = pso.Properties[tempProperty.Name].Value.ToPSObject();
                    pso.Properties.Remove(tempProperty.Name);
                    var name = specValue.Properties["Name"].Value;
                    
                    var byItemScript =
                        String.Format("$this.'{1}' | {2} | where {{ $_.Name -eq '{0}' }} | select -expand Group",
                                      name, acrossAccessorName, spec.IndexSpecifier.GetterScript);
                    var prop = new PSScriptProperty(
                                 acrossAccessorName + "_" + name,
                                 System.Management.Automation.ScriptBlock.Create(byItemScript));
                    var scale = (from sd in spec.ScaleDescriptors
                                 where sd.Key.IsMatch(prop.Name)
                                 select sd.Value).FirstOrDefault();
                    list.Add( new DynamicMemberDescriptor( prop, scale, name));
                }
            }
            return list;
        }


    }
}
