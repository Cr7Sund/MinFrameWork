namespace Cr7Sund
{
    public interface ILoading
    {
        public bool IsLoading();
        public void StartLoad();
        public void SetAdding();
        public void StartPreload();
        public void EndPreload();
        public void SetReady();
        public void StartUnload(bool unload);
        public void EndLoad(bool unload);
    }


}
