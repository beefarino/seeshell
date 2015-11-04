using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot.Axes;

namespace CodeOwls.SeeShell.Visualizations.ViewModels
{
    public class CategoryDateTimeAxis : CategoryAxis
    {
        public CategoryDateTimeAxis() : base()
        {
        }

        public void Configure(DateTime min, DateTime max, TimeSpan interval)
        {
            var itemSource = new List<DateTime>();
            DateTime current = min;
            while (current <= max)
            {
                itemSource.Add( current );
                current += interval;
            }

            itemSource.Add(current);

            Labels.Clear();
            ActualLabels.Clear();

            Labels.AddRange(itemSource.ConvertAll(a => a.ToString()));
            this.ActualLabels.AddRange( Labels );
        }
    }
}
