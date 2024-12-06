using ProceduralBlocks.Selection;
using ProceduralBlocks.UserInput;
using UnityEngine;

namespace ProceduralBlocks.Managers
{
    [Injectable]
    public class PieceMovementManager : MonoBehaviour
    {
        [SerializeField] private LayerMask _blockLayer;
        
        private IInputService _inputService;
        private ISelectionService _selectionService;
        private PieceOverlapChecker _overlapChecker;

        private Piece _selectedPiece;
        private Vector3 _pieceStartPosition;
        private Vector3 _offset;

        private Ray _ray;
        private Camera _camera;
        private Plane _plane = new Plane(Vector3.back, -0.55f);
        
        [Inject]
        private void Construct(IInputService inputService, ISelectionService selectionService,
            PieceOverlapChecker overlapChecker, Camera cam)
        {
            _inputService = inputService;
            _selectionService = selectionService;
            _overlapChecker = overlapChecker;
            _camera = cam;
        }
        
        private void SelectionProcess()
        {
            if (_selectionService.TryGetSelectable(_inputService.InputPosition, _blockLayer, out RaycastHit hit,
                    out Piece p))
            {
                _selectedPiece = p;
                _pieceStartPosition = p.transform.position;
                _offset = _pieceStartPosition - hit.point;
                p.Select();
            }
            else
            {
                _selectedPiece = null;
            }
        }

        private void Move()
        {
            if (_selectedPiece == null) return;

            _ray = _camera.ScreenPointToRay(_inputService.InputPosition);

            if (!_plane.Raycast(_ray, out float distance)) return;

            _selectedPiece.transform.position = _ray.GetPoint(distance) + _offset;
        }
        
        private void Deselect()
        {
            if (_selectedPiece == null) return;

            if (_overlapChecker.CheckOverlap(_selectedPiece))
                _selectedPiece.transform.position = _pieceStartPosition;
            
            _selectedPiece.Deselect();
            _selectedPiece = null;
        }
        
        private void OnEnable()
        {
            _inputService.onTouch += SelectionProcess;
            _inputService.onHold += Move;
            _inputService.onRelease += Deselect;
        }

        private void OnDisable()
        {
            _inputService.onTouch -= SelectionProcess;
            _inputService.onHold -= Move;
            _inputService.onRelease -= Deselect;
        }
    }
}