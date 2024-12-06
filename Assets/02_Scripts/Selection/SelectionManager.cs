using ProceduralBlocks.Selection;
using UnityEngine;

namespace ProceduralBlocks.Managers
{
    public class SelectionManager : ISelectionService
    {
        private readonly Camera _camera;
        
        private Ray _ray;
        private RaycastHit _hit;
        
        public SelectionManager(Camera camera)
        {
            _camera = camera;
        }

        public bool TryGetSelectable<T>(Vector3 inputPos, LayerMask selectionLayer, out T selectable) 
            where T : ISelectable
        {
            selectable = default;
            
            if (TryGetSelectable(inputPos, selectionLayer, out _, out T s)) selectable = s;
            
            return selectable != null;
        }

        public bool TryGetSelectable<T>(Vector3 inputPos, LayerMask selectionLayer, out RaycastHit hit, out T selectable) 
            where T : ISelectable
        {
            selectable = default;
            hit = default;

            if (!SelectionLock.CanSelect) return false;
            
            _ray = _camera.ScreenPointToRay(inputPos);

            if (!Physics.Raycast(_ray, out _hit, _camera.farClipPlane, selectionLayer)) return false;

            selectable = GetComponentFromAllHierarchy<T>(_hit.collider);
            hit = _hit;
            return selectable != null;
        }

        private T GetComponentFromAllHierarchy<T>(Collider c)
        {
            var t = c.GetComponentInChildren<T>();
            if (t != null) return t;

            t = c.GetComponentInParent<T>();
            return t;
        }
    }
}