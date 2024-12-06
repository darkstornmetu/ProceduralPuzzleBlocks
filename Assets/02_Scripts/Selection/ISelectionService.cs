using UnityEngine;

namespace ProceduralBlocks.Selection
{
    public interface ISelectionService
    {
        public bool TryGetSelectable<T>(Vector3 inputPos, LayerMask selectionLayer, out T selectable) 
            where T : ISelectable; 
        
        public bool TryGetSelectable<T>(Vector3 inputPos, LayerMask selectionLayer, out RaycastHit hit, out T selectable)
            where T : ISelectable;
    }
}