namespace Cr7Sund.NodeTree.Api
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
        UNVALILD_NODESTATE,
        
        #region Loadable
        /// <summary>
        /// Cant LoadAsync On State of loading or unloading
        /// </summary>
        LOAD_VALID_STATE,
        Two,
        
        #endregion
    }
}
