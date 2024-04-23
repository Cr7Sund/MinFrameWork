namespace Cr7Sund.NodeTree.Api
{
    public enum NodeTreeExceptionType
    {
        /// <summary>
        /// cant add empty child 
        /// </summary>
        EMPTY_NODE_ADD,
        /// <summary>
        /// remove empty child 
        /// </summary>
        EMPTY_NODE_REMOVE,
        /// <summary>
        /// cant add child still in loading or unloading
        /// </summary>
        INVALID_NODESTATE,

        /// <summary>
        /// empty context, check the game-node's first root context 
        /// </summary>
        EMPTY_CONTEXT,

        /// <summary>
        /// can not remove an existed node
        /// </summary>
        REMOVE_NO_EXISTEDNODE,

        /// <summary>
        /// try to add an already added node
        /// </summary>
        already_added,
        /// <summary>
        /// should be default state
        /// </summary>
        default_state,
        /// <summary>
        /// try to dispose not init or already destroyed
        /// </summary>
        dispose_not_int,
        #region Loadable
        /// <summary>
        /// Cant LoadAsync On State of loading or unloading
        /// </summary>
        LOAD_VALID_STATE,
        /// <summary>
        /// Cant UnLoadAsync On State of loading or unloading
        /// </summary>
        UNLOAD_VALID_STATE,
        /// <summary>
        /// Cant cancel when not loaded 
        /// </summary>
        CANCEL_NOT_LOADED,


        #endregion

        #region Controller
        /// <summary>
        ///  Add controller error: controller is null!!
        /// </summary>
        EMPTY_CONTROLLER_ADD,
        /// <summary>
        ///  Remove controller error: controller is null!!
        /// </summary>
        EMPTY_CONTROLLER_REMOVE,
        #endregion

        #region IOC
        /// <summary>
        /// there should be a cross context
        /// </summary>
        EMPTY_CROSS_CONTEXT,
        /// <summary>
        /// try to assign duplicate cross context
        /// </summary>
        DUPLICATE_CROSS_CONTEXT,
        /// <summary>
        /// try to add node witch existed unfulfilled operation
        /// </summary>
        ADD_RECYCLED,
        #endregion
    }
}
