using System;
using UnityEngine;

namespace ProceduralBlocks.UserInput
{
    public interface IInputService
    {
        public event Action onTouch;
        public event Action onHold;
        public event Action onRelease;

        public Vector2 Delta { get; }
        public Vector3 InputPosition { get; }
    }
}