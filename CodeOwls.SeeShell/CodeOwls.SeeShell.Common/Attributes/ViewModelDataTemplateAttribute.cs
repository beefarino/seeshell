using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeOwls.SeeShell.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class ViewModelDataTemplateAttribute : Attribute
    {
        private readonly string _templateName;

        public ViewModelDataTemplateAttribute( string templateName )
        {
            _templateName = templateName;
        }

        public string TemplateName
        {
            get { return _templateName; }
        }
    }
}
