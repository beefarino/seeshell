using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Text;
using Xunit;

namespace CodeOwls.SeeShell.Common.Tests
{
    public class SolidPSObjectTests
    {
        public class Scrap
        {
            public string Item { get; set; }
            public string ReadOnly
            {
                get { return ""; }
            }
            public string WriteOnly
            {
                set
                {
                    // nop
                }
            }
        }

        [Fact]
        public void CanSolidifyPSObject()
        {
            PSObject pso = PSObject.AsPSObject(DateTime.Now);
            AssertCanSolidify(pso);
        }

        [Fact]
        public void CanGetMembers()
        {
            var client = new WebClient();
            var o = AssertCanSolidify(PSObject.AsPSObject(client));

            var getters = from p in typeof (WebClient).GetProperties()
                          where p.CanRead
                          let prop = o.GetType().GetProperty(p.Name)
                          select prop;

            foreach (var getter in getters)
            {
                Assert.True(getter.CanRead);
                // assert no exception on get
                getter.GetValue(o, null);
            }

        }

        [Fact]
        public void CanSetMembers()
        {
            var client = new Scrap();
            var o = AssertCanSolidify(PSObject.AsPSObject(client));

            var setters = from p in typeof(Scrap).GetProperties()
                          where p.CanWrite
                          let prop = o.GetType().GetProperty(p.Name)
                          select prop;

            foreach (var setter in setters)
            {
                Assert.True(setter.CanWrite);
                
                // assert no exception on set
                setter.SetValue(o, null, null );
            }

        }
        
        private object AssertCanSolidify(PSObject pso)
        {
            var o = Solidify(pso);

            foreach (var prop in pso.Properties)
            {
                var pi = o.GetType().GetProperty(prop.Name);
                Assert.NotNull(pi);

                Assert.Equal(prop.IsSettable, pi.CanWrite);
                Assert.Equal(prop.IsGettable, pi.CanRead);
            }

            return o;
        }

        private static object Solidify(PSObject pso)
        {
            var solidifier = new PSObjectSolidifier();
            var o = solidifier.AsConcreteType(pso);
            Assert.NotNull(o);
            return o;
        }

    }
}
