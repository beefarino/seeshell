using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Xunit;
namespace CodeOwls.SeeShell.Common.Tests
{
    public class RangeDescriptorFactoryTests
    {
        [Fact]
        public void CanProcessSingleFullRangeColorSpec()
        {
            var values = new object[] {0, Colors.Green, 100};
            var range = RangeDescriptorFactory.Create(Colors.Green, values);

            Assert.NotNull( range );
            Assert.Equal( 1, range.Count() );
            Assert.Equal( 0, range.First().Minimum );
            Assert.Equal(100, range.First().Maximum);
            Assert.Equal( Colors.Green, range.First().Color.Value);
        }

        [Fact]
        public void CanProcessMultipleFullRangeColorSpec()
        {
            var values = new object[] { 0, Colors.Green, 80, Colors.Yellow, 95, Colors.Red, 100 };
            var range = RangeDescriptorFactory.Create(Colors.Blue, values);

            Assert.NotNull(range);
            Assert.Equal(3, range.Count());
            var r = range.First();
            Assert.Equal(0, r.Minimum);
            Assert.Equal(80, r.Maximum);
            Assert.Equal(Colors.Green, r.Color.Value);
            r = range.Skip(1).First();
            Assert.Equal(80, r.Minimum);
            Assert.Equal(95, r.Maximum);
            Assert.Equal(Colors.Yellow, r.Color.Value);
            r = range.Skip(2).First();
            Assert.Equal(95, r.Minimum);
            Assert.Equal(100, r.Maximum);
            Assert.Equal(Colors.Red, r.Color.Value);
        }

        [Fact]
        public void CanProcessOpenStartRangeColorSpec()
        {
            var values = new object[] { Colors.Green, 80 };
            var range = RangeDescriptorFactory.Create(values);

            Assert.NotNull(range);
            Assert.Equal(1, range.Count());
            var r = range.First();

            Assert.Equal(80, r.Maximum);
            Assert.Equal(Colors.Green, r.Color.Value);
        }

        [Fact]
        public void CanProcessOpenEndRangeColorSpec()
        {
            var values = new object[] { 0, Colors.Green };
            var range = RangeDescriptorFactory.Create(values);

            Assert.NotNull(range);
            Assert.Equal(1, range.Count());
            var r = range.First();
            Assert.Equal(0, r.Minimum); 
            Assert.Equal(Colors.Green, r.Color.Value);
        }

        [Fact]
        public void CanProcessMultipleOpenEndRangeColorSpec()
        {
            var values = new object[] { Colors.Green, 75, Colors.Gold };
            var range = RangeDescriptorFactory.Create(values);

            Assert.NotNull(range);
            Assert.Equal(2, range.Count());
            var r = range.First();
            Assert.Equal(75, r.Maximum);
            Assert.Equal(Colors.Green, r.Color.Value);
            r = range.Skip(1).First();
            Assert.Equal(75, r.Minimum);
            Assert.Equal(Colors.Gold, r.Color.Value);
        }

        [Fact]
        public void CanProcessColorlessRangeSpec()
        {
            var values = new object[] { 5, 75, 155 };
            var range = RangeDescriptorFactory.Create(values);

            Assert.NotNull(range);
            Assert.Equal(2, range.Count());
            var r = range.First();
            Assert.Equal(5, r.Minimum);
            Assert.Equal(75, r.Maximum);
            Assert.False(r.Color.HasValue);
            r = range.Skip(1).First();
            Assert.Equal(75, r.Minimum);
            Assert.Equal(155, r.Maximum);
            Assert.False(r.Color.HasValue);
        }

        [Fact]
        public void CanProcessColorlessDoubleRangeSpec()
        {
            var values = new object[] { 0.5, 0.75, 1.55 };
            var range = RangeDescriptorFactory.Create(values);

            Assert.NotNull(range);
            Assert.Equal(2, range.Count());
            var r = range.First();
            Assert.Equal(0.5, r.Minimum);
            Assert.Equal(0.75, r.Maximum);
            Assert.False(r.Color.HasValue);
            r = range.Skip(1).First();
            Assert.Equal(0.75, r.Minimum);
            Assert.Equal(1.55, r.Maximum);
            Assert.False(r.Color.HasValue);
        }
    }
}
