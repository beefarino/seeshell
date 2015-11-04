using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CodeOwls.SeeShell.Common.Utility;

namespace CodeOwls.SeeShell.Common
{
    public class ScaleDescriptorAssignmentCollection : ObservableCollection<IScaleDescriptorAssignment>
    {
        private readonly object _lock = new object();

        protected override void InsertItem(int index, IScaleDescriptorAssignment item)
        {
            lock (_lock)
            {
                base.InsertItem(index, item);
            }
        }

        public IScaleDescriptorAssignment ForProperty(string name)
        {
            lock (_lock)
            {
                var scale = (from item in this
                             where StringComparer.InvariantCultureIgnoreCase.Equals(item.PropertyName, name)
                             select item).FirstOrDefault();

                return scale;
            }

        }

        
        public IScaleDescriptor GetScale(string name)
        {
            lock (_lock)
            {
                return (from item in this
                        where StringComparer.InvariantCultureIgnoreCase.Equals(item.Scale.Name, name)
                        select item.Scale).FirstOrDefault();
            }
        }

    }
}