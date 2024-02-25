using System;
namespace Cr7Sund
{
    ///////////////////////////
    // Types
    ///////////////////////////

    [Flags]
    public enum LogChannel : uint
    {

        /// <summary>
        ///     Logs directly from the Lua VM
        /// </summary>
        LuaNative = 1 << 2,
        /// <summary>
        ///     Logs to do with graphics/rendering
        /// </summary>
        Rendering = 1 << 3,
        /// <summary>
        ///     Logs to do with the physics system
        /// </summary>
        Physics = 1 << 4,
        /// <summary>
        ///     Logs to do with our UI system
        /// </summary>
        UILogic = 1 << 5,
        /// <summary>
        ///     Logs about NetDevices and networks
        /// </summary>
        NetDevice = 1 << 6,
        /// <summary>
        ///     Logs to do with sound and Wwise
        /// </summary>
        Audio = 1 << 7,
        /// <summary>
        ///     Logs to do with level loading
        /// </summary>
        Loading = 1 << 8,
        /// <summary>
        ///     Logs to do with localization
        /// </summary>
        Localization = 1 << 9,
        /// <summary>
        ///     Logs to do with platform services
        /// </summary>
        Platform = 1 << 10,
        /// <summary>
        ///     Logs asserts
        /// </summary>
        Assert = 1 << 11,
        /// <summary>
        ///     Build/Content generation logs
        /// </summary>
        Build = 1 << 12,
        /// <summary>
        ///     Analytics logs
        /// </summary>
        Analytics = 1 << 13,
        /// <summary>
        ///     Animation logs
        /// </summary>
        Animation = 1 << 14,
        /// <summary>
        ///     Assets logs
        /// </summary>
        Assets = 1 << 15,
        /// <summary>
        ///     scene logic logs
        /// </summary>

        SceneLogic = 1 << 19,
        /// <summary>
        ///     Logs to do with AI/GOAP/
        /// </summary>
        AI = 1 << 20,
        /// <summary>
        ///     game logic logs
        /// </summary>
        GameLogic = 1 << 21,
        /// <summary>
        ///     game entrance
        /// </summary>
        Entrance = 1 << 23,
    }
}
