namespace Cr7Sund.Editor.NodeTree
{
    public interface IView
    {
        void StartView(IView parentView);
        void StopView(IView parentView);
    }
}