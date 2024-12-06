using System;
using UnityEngine;

namespace ProceduralBlocks.UserInput
{
    public class InputManager : MonoBehaviour, IInputService
    {
        public event Action onTouch;
        public event Action onHold;
        public event Action onRelease;

        public Vector2 Delta { get; private set; }
        public Vector3 InputPosition => Input.mousePosition;

        private Vector2 _screenRes;
    
        private Vector2 _current;
        private Vector2 _previous;

        private void Start()
        {
            _screenRes.x = Screen.width;
            _screenRes.y = Screen.height;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _previous = _current = InputPosition;
            
                onTouch?.Invoke();
            }

            if (Input.GetMouseButton(0))
            {
                _previous = _current;
                _current = InputPosition;

                Delta = (_current - _previous) / _screenRes;
            
                onHold?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                Delta = Vector2.zero;
                onRelease?.Invoke();
            }
        }
    }
}