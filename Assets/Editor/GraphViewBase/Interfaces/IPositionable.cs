using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.GraphView
{
    public interface IPositionable
    {
        public event Action<PositionData> OnPositionChange;
        public Vector2 GetGlobalCenter();
        public Vector2 GetCenter();
        public Vector2 GetPosition();
        public void SetPosition(Vector2 position);
        public void ApplyDeltaToPosition(Vector2 delta);
        public bool IsValid();
    }

    public struct PositionData
    {
        public Vector2 position;
        public VisualElement element;
    }
}