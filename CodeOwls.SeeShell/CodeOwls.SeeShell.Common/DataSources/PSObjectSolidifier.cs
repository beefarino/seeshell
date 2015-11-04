using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using CodeOwls.SeeShell.Common.DataSources;

namespace CodeOwls.SeeShell.Common
{
    public class PSObjectSolidifier
    {
        private static readonly Log Log = new Log( typeof( PSObjectSolidifier));
        private static AssemblyBuilder assemblyBuilder;
        private static ModuleBuilder moduleBuilder;
        private const string DynamicAssemblyName = "CodeOwls.SeeShell.DynamicTypes";

        static PSObjectSolidifier()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Log.Error( "an unhandled exception has been received by the appdomain exception handler", e.ExceptionObject as Exception);

            }
            catch
            {
            }
        }

        public object AsConcreteType(PSObject pso)
        {
            if (null == pso)
            {
                return null;
            }

            var assemblyName = new AssemblyName
                                   {
                                       Name = DynamicAssemblyName,
                                       Version = new Version(1, 0, 0, 0),
                                       CultureInfo = CultureInfo.CurrentCulture
                                   };

            if (null == assemblyBuilder)
            {
                var appDomain = AppDomain.CurrentDomain;
                assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
                moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");
            }

            string typename = GenerateTypeName(assemblyName.Name, pso);

            Type solidType = null;
            lock (assemblyBuilder)
            {
                solidType = assemblyBuilder.GetType(typename, false);

                if (null == solidType)
                {
                    var names = (from p in pso.Properties select p.Name).ToList();
                    try
                    {
                        TypeBuilder typeBuilder = moduleBuilder.DefineType(typename, TypeAttributes.Public,
                                                                           typeof (SolidPSObjectBase));

                        DefineCtor(typeBuilder);
                        DefineAccessors(typeBuilder, names, pso);
                        solidType = typeBuilder.CreateType();
                    }
                    catch( Exception e )
                    {
                        Log.Error( "an exception was raised while trying to build a concrete type", e );
                        solidType = assemblyBuilder.GetType(typename, false);
                        if( null == solidType )
                        {
                            Log.WarnFormat( "failed to locate solid type for typename [{0}] after exception during type generation", typename);
                        }
                    }

                }
            }

            if (null == solidType)
            {
                solidType = Type.GetType(typename);
            }

            if( null == solidType )
            {
                Log.WarnFormat( "failed to find/define concrete type for ");
                return null;
            }

            //assemblyBuilder.Save( assemblyName.Name + ".dll");
            return Activator.CreateInstance(solidType, new[] { pso });
        }

        private void DefineCtor(TypeBuilder typeBuilder)
        {
            var baseType = typeof(SolidPSObjectBase);
            var ctor = baseType.GetConstructor(new[] { typeof(PSObject) });
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard,
                                                            new[] { typeof(PSObject) });
            var generator = ctorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, ctor);
            generator.Emit(OpCodes.Ret);
        }

        private void DefineAccessors(TypeBuilder typeBuilder, List<string> names, PSObject pso)
        {
            foreach (var name in names)
            {
                Log.DebugFormat("Defining accessor for property [{0}]", name);
                DefineAccessor(typeBuilder, name, pso);
            }
        }

        private void DefineAccessor(TypeBuilder typeBuilder, string name, PSObject pso)
        {
            var prop = pso.Properties.Match(name).FirstOrDefault();
            if( null == prop )
            {
                return;
            }
            var propType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                            where ! asm.FullName.StartsWith( DynamicAssemblyName )
                            from type in asm.GetTypes()
                            where StringComparer.InvariantCultureIgnoreCase.Equals(type.FullName, prop.TypeNameOfValue)
                            select type).FirstOrDefault() ?? typeof(object);

            var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.HasDefault, propType, null);
            var attrs = MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public;
            if (prop.IsGettable)
            {
                var method = typeof(SolidPSObjectBase).GetMethod("GetPropValue",
                                                                 new[] { typeof( string ) } );
                var gmi = method.MakeGenericMethod(propType);

                var getter = typeBuilder.DefineMethod("get_" + name, attrs, propType, Type.EmptyTypes);
                var generator = getter.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, name);
                generator.Emit(OpCodes.Call, gmi);
                generator.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(getter);
            }
            if (prop.IsSettable)
            {
                var method = typeof(SolidPSObjectBase).GetMethod("SetPropValue",
                                                                 BindingFlags.Public | BindingFlags.Instance);

                var setter = typeBuilder.DefineMethod("set_" + name, attrs, null, new[] { propType });
                var generator = setter.GetILGenerator();
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldstr, name);
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Call, method);
                generator.Emit(OpCodes.Ret);

                propertyBuilder.SetSetMethod( setter );
            }
        }

        private string GenerateTypeName(string s, PSObject pso)
        {
            var names = (from p in pso.Properties select p.TypeNameOfValue.ToUpperInvariant() + "$" + p.Name.ToUpperInvariant()).OrderBy(a => a);
            var name = String.Join("_", names.ToArray());
            var bytes = Encoding.ASCII.GetBytes(name);
            using (var sha = SHA1Managed.Create())
            {
                bytes = sha.ComputeHash(bytes);
            }

            name = BitConverter.ToString(bytes).Replace("-", "");

            return s + ".__$" + name;
        }
    }
}