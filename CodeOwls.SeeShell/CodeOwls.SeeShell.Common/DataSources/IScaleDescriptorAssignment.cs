namespace CodeOwls.SeeShell.Common
{
    public interface IScaleDescriptorAssignment
    {
        string PropertyName { get; }
        IScaleDescriptor Scale { get; }
    }

    public class ScaleDescriptorAssignment : IScaleDescriptorAssignment
    {
        public string PropertyName { get; set; }

        public IScaleDescriptor Scale
        {
            get;
            set;
        }
    }
}