using UnityEngine;

namespace UnityEditor.GraphView
{
    public interface ISelectable
    {
        ISelector Selector { get; }
        bool Selected { get; set; }
        bool Overlaps(Rect rectangle);
        bool ContainsPoint(Vector2 localPoint);
    }
}