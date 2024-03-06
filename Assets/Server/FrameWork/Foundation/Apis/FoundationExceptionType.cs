namespace Cr7Sund.Server.Impl
{
    public enum FoundationExceptionType
    {
        // try to preload node twice or more
        duplicate_preloadNode,
        // try to preload parent node is no started
        parent_no_start_preloadNode,
        // try to add node twice or more
        duplicate_addNode,
        // try to add parent node is no started
        parent_no_start_addNode,
        // try to add parent node is removing
        is_removing_addNode,
        // try to add parent node is unloading
        is_unloading_addNode,
        // try to remove parent node but not handled
        unhandled_removeNode,
        // try to unload parent node is no started
        parent_no_start_unloadNode,
        // try to create a go from an invalid scene
        create_from_invalidScene,
    }
}
