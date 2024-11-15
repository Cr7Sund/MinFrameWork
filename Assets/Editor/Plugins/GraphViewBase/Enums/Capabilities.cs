// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace UnityEditor.GraphView
{
    [Flags]
    public enum Capabilities
    {
        Selectable = 1 << 0,
        Collapsible = 1 << 1,
        Resizable = 1 << 2,
        Movable = 1 << 3,
        Deletable = 1 << 4,
        Droppable = 1 << 5,
        Ascendable = 1 << 6,
        Renamable = 1 << 7,
        Copiable = 1 << 8,
        Snappable = 1 << 9,
        Groupable = 1 << 10,
        Stackable = 1 << 11
    }
}
