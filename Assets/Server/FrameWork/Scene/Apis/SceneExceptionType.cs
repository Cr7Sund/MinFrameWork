namespace Cr7Sund.Server.Impl
{
    public enum SceneExceptionType
    {
        // scene key's type is not assignable from scene builder 
        INVALID_SCENE_BUILDER_TYPE,
        // the node should not loading state again
        CHANGE_LOADING_STATE,
        CHANGE_ADDING_STATE,
    }
}
