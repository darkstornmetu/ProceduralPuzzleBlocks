using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ProceduralBlocks.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class GenericTweenUI : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private Sequence _sequence;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        [Title("Tween Properties", TitleAlignment = TitleAlignments.Centered)]
        [PropertySpace]
        [Title("Animation Properties")]
        [SerializeField] private bool isIndependentFromTimeScale = false;
        [SerializeField] private float animationTime = 1;
        [SerializeField] private Ease animationEase = Ease.Linear;
        [Title("Loop Properties")]
        [SerializeField] private bool canLoop = false;
        [InfoBox("-1 for infinity")]
        [SerializeField] [EnableIf("@this.canLoop")] private int loopCount = -1;
        [SerializeField] [EnableIf("@this.canLoop")] private LoopType loopType = LoopType.Yoyo;
        [Title("Position Properties")]
        [SerializeField] private bool posChange = true;
        [SerializeField] [EnableIf("@this.posChange")] private Vector2 startPos = Vector3.zero;
        [SerializeField] [EnableIf("@this.posChange")] private Vector2 endPos = Vector3.zero;
        [Title("Scale Properties")]
        [SerializeField] private bool scaleChange = false;
        [SerializeField] [EnableIf("@this.scaleChange")] private Vector3 startScale = Vector3.one;
        [SerializeField] [EnableIf("@this.scaleChange")] private Vector3 endScale = Vector3.one;
        [Title("Rotation Properties")]
        [SerializeField] private bool rotChange = false;
        [SerializeField] [EnableIf("@this.rotChange")] private Vector3 startRot = Vector3.zero;
        [SerializeField] [EnableIf("@this.rotChange")] private Vector3 endRot = Vector3.zero;


        private void OnEnable()
        {
            _sequence = DOTween.Sequence();
            
            if (isIndependentFromTimeScale)
                _sequence.SetUpdate(true);

            if (posChange)
                _sequence.Join(_rectTransform.DOAnchorPos(endPos, animationTime).From(startPos).SetEase(animationEase));

            if (scaleChange)
                _sequence.Join(_rectTransform.DOScale(endScale, animationTime).From(startScale).SetEase(animationEase));

            if (rotChange)
                _sequence.Join(_rectTransform.DORotate(startRot, animationTime).From(endRot).SetEase(animationEase));

            if (canLoop)
                _sequence.SetLoops(loopCount, loopType);
        }

        private void OnDestroy()
        {
            _sequence.Kill();
        }
    }
}
