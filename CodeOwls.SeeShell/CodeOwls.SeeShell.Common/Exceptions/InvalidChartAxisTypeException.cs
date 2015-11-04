using System;
using System.Management.Automation;
using System.Runtime.Serialization;
using CodeOwls.SeeShell.Common.Charts;
using CodeOwls.SeeShell.Common.DataSources;
using CodeOwls.SeeShell.Common.ViewModels.Charts;

namespace CodeOwls.SeeShell.Common.Exceptions
{
    [Serializable]
    public class DataPropertyException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DataPropertyException()
        {
        }

        public DataPropertyException(string message) : base(message)
        {
        }

        public DataPropertyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DataPropertyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class DataPropertyNotFoundException : DataPropertyException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public DataPropertyNotFoundException()
        {
        }

        public DataPropertyNotFoundException(string message) : base(message)
        {
        }

        public DataPropertyNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DataPropertyNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
    [Serializable]
    public class InvalidRangeDescriptorException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public InvalidRangeDescriptorException()
        {
        }

        public InvalidRangeDescriptorException(string message) : base(message)
        {
        }

        public InvalidRangeDescriptorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected InvalidRangeDescriptorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidChartAxisTypeException : ApplicationException
    {
        public ChartSeriesType SeriesType { get; private set; }
        public ChartAxisType AxisType { get; private set; }

        public InvalidChartAxisTypeException(ChartSeriesType seriesType, ChartAxisType axisType)
            : base(String.Format("Invalid axis type [{0}] for series type [{1}]", axisType, seriesType))
        {
            SeriesType = seriesType;
            AxisType = axisType;
        }

        public InvalidChartAxisTypeException()
        {
        }

        protected InvalidChartAxisTypeException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidChartAxisValueMemberException : ApplicationException
    {
        public ChartSeriesType SeriesType { get; private set; }
        public ChartAxisType AxisType { get; private set; }
        public new object Data { get; private set; }
        public string ValueMemberPath { get; private set; }

        public InvalidChartAxisValueMemberException(ChartSeriesType seriesType, ChartAxisType axisType, SolidPSObjectBase data, string valueMemberPath)
            : base(String.Format(
                "The value member [{0}] of data type [{3}] is invalid for axis type [{2}] series type [{1}]", 
                valueMemberPath, 
                seriesType, 
                axisType, 
                null == data ? "null" : data.GetPropTypeName( valueMemberPath )
            ))
        {
            SeriesType = seriesType;
            AxisType = axisType;
            Data = data;
            ValueMemberPath = valueMemberPath;
        }

        public InvalidChartAxisValueMemberException()
        {
        }

        protected InvalidChartAxisValueMemberException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class ChartAxisValueMemberDoesNotExistException : ApplicationException
    {
        public ChartSeriesType SeriesType { get; private set; }
        public ChartAxisType AxisType { get; private set; }
        public string ValueMemberPath { get; private set; }

        public ChartAxisValueMemberDoesNotExistException(ChartSeriesType seriesType, ChartAxisType axisType, string valueMemberPath)
            : base(String.Format(
                "The value member [{0}] for axis type [{2}] series type [{1}] does not exist in the data",
                valueMemberPath,
                seriesType,
                axisType
            ))
        {
            SeriesType = seriesType;
            AxisType = axisType;
            ValueMemberPath = valueMemberPath;
        }

        public ChartAxisValueMemberDoesNotExistException()
        {
        }

        protected ChartAxisValueMemberDoesNotExistException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class InvalidChartValueMemberException : ApplicationException
    {
        public ChartSeriesType SeriesType { get; private set; }
        public PSPropertyInfo Property { get; private set; }
        public string ValueMemberPath { get; private set; }

        public InvalidChartValueMemberException(ChartSeriesType seriesType, PSPropertyInfo prop, string valueMemberPath, string viewModelMemberName)
            : base(String.Format(
                "The value member [{0}] for the [{2}] of series type [{1}] is of an invalid type [{3}].",
                valueMemberPath,
                seriesType,
                viewModelMemberName,
                null != prop ? prop.TypeNameOfValue : "null"
            ))
        {
            SeriesType = seriesType;
            Property = prop;
            ValueMemberPath = valueMemberPath;
        }

        public InvalidChartValueMemberException()
        {
        }

        protected InvalidChartValueMemberException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}