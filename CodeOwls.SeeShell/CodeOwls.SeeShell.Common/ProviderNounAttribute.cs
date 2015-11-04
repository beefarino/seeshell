using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common
{
    [AttributeUsage( AttributeTargets.Class, Inherited = true)]
    public class ProviderNounAttribute : Attribute
    {
        private readonly string _noun;
        private readonly string _category;

        public ProviderNounAttribute( string noun, string category )
        {
            _noun = noun;
            _category = category;
        }
    }
}
