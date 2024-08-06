namespace Cr7Sund.Editor.NodeGraph
{
    public interface IView
    {
        void StartView(IView parentView);
        void StopView(IView parentView);
    }
}