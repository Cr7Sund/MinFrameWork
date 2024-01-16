﻿namespace Cr7Sund.NodeTree.Api
{
    public enum NodeTreeExceptionType
    {
        /// <summary>
        /// cant add empty child 
        /// </summary>
        EMPTY_NODE_ADD,
        /// <summary>
        /// remove add empty child 
        /// </summary>
        EMPTY_NODE_REMOVE,
        /// <summary>
        /// cant add child still in loading or unloading
        /// </summary>
        INVALID_NODESTATE,

        /// <summary>
        /// empty context, check the gamenode's first root context 
        /// </summary>
        EMPTY_CONTEXT,

        #region Loadable
        /// <summary>
        /// Cant LoadAsync On State of loading or unloading
        /// </summary>
        LOAD_VALID_STATE,
        /// <summary>
        /// Cant UnLoadAsync On State of loading or unloading
        /// </summary>
        UNLOAD_VALID_STATE,
        Two,
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
        #endregion
    }
}
