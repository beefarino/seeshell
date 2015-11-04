using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace CodeOwls.SeeShell.Common
{
    public class DynamicMemberDescriptor
    {
        //private readonly DynamicMemberSpecification _memberSpec;
        private readonly PSPropertyInfo _memberinfo;
        private readonly IScaleDescriptor _scaleDescriptor;
        private readonly object _indexValue;

        public DynamicMemberDescriptor( PSPropertyInfo memberinfo, IScaleDescriptor scaleDescriptor)
            : this( memberinfo, scaleDescriptor, null)
        {            
        }

        public DynamicMemberDescriptor(PSPropertyInfo memberinfo, IScaleDescriptor scaleDescriptor, object indexValue)
        {
            _memberinfo = memberinfo;
            _scaleDescriptor = scaleDescriptor;
            _indexValue = indexValue;
        }

        public object IndexValue
        {
            get { return _indexValue; }
        }

        public PSPropertyInfo MemberInfo
        {
            get { return _memberinfo; }
        }

        public IScaleDescriptor ScaleDescriptor
        {
            get { return _scaleDescriptor; }
        }
    }
    public class DynamicMemberSpecification
    {
        private readonly object[] _plotItems;
        private readonly object _againstItem;
        private readonly PSScriptProperty _acrossSpecifier;
        private readonly PSScriptProperty _indexSpecifier;
        private readonly List<DynamicMemberDescriptor> _plotItemDescriptors;
        private readonly IDictionary<Regex, IScaleDescriptor> _scaleDescriptors;

        public DynamicMemberSpecification(object[] plotItems, object againstItem, PSScriptProperty acrossSpecifier, PSScriptProperty indexSpecifier,
            IDictionary<Regex,IScaleDescriptor> scaleDescriptors )
        {
            if (plotItems == null)
            {
                throw new ArgumentNullException("plotItems");
            }

            _scaleDescriptors = scaleDescriptors;
            _plotItemDescriptors = new List<DynamicMemberDescriptor>();
            _plotItems = plotItems;
            _againstItem = againstItem;
            _acrossSpecifier = acrossSpecifier;
            _indexSpecifier = indexSpecifier;          
        }

        public DynamicMemberSpecification(object[] plotItems, object againstItem, PSScriptProperty acrossSpecifier, PSScriptProperty indexSpecifier)
            : this( plotItems, againstItem, acrossSpecifier, indexSpecifier,new Dictionary<Regex, IScaleDescriptor>())
        {
        }

        public IDictionary<Regex, IScaleDescriptor> ScaleDescriptors
        {
            get { return _scaleDescriptors; }
        }

        public IList<DynamicMemberDescriptor> PlotItemDescriptors
        {
            get { return _plotItemDescriptors; }
        }

        public PSPropertyInfo AgainstProperty { get; set; }

        public object AgainstItem
        {
            get { return _againstItem; }
        }

        public PSScriptProperty IndexSpecifier
        {
            get { return _indexSpecifier; }
        }

        public object IndexValue { get; set; }

        public PSScriptProperty AcrossSpecifier
        {
            get { return _acrossSpecifier; }
        }

        public object[] PlotItems
        {
            get { return _plotItems; }
        }

        public string AgainstPropertyName
        {
            get
            {
                if (null == AgainstProperty)
                {
                    return null;
                }
                return AgainstProperty.Name;
            }
        }
    }
}