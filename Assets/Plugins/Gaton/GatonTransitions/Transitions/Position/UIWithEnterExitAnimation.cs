using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/UI Enter-Exit Transition", order: 5)]
    public class UIWithEnterExitAnimation : Transition
    {
        [Title("Positions")]
        [SerializeField] Vector2 startPos;

        [SerializeField] Vector2 endPos;

        [Title("Tween Values")]
        [SerializeField] float enterDuration;

        [SerializeField] float exitDuration;

        [SerializeField] float enterDelay = 0f;

        [SerializeField] Ease startEase = Ease.OutExpo;

        [SerializeField] Ease endEase = Ease.OutExpo;

        [Title("Conditions")]
        [SerializeField] bool hideWhenExit = true;

        bool isShowing = false;

        RectTransform rect;

        Tween moveTween;

        public bool IsShowing { get => isShowing; }

        public override float StartDuration => enterDuration + enterDelay;
        public override float EndDuration => exitDuration;

        public Vector2 StartPos { get => startPos; set => startPos = value; }
        public Vector2 EndPos { get => endPos; set => endPos = value; }
        public RectTransform Rect { get => rect; set => rect = value; }

        public new void Awake()
        {
            Rect = GetComponent<RectTransform>();

            Rect.anchoredPosition = StartPos;

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            if (moveTween != null)
                moveTween.Kill();

            isShowing = true;

            gameObject.SetActive(true);

            moveTween = Rect.DOAnchorPos(EndPos, enterDuration).SetEase(startEase).OnComplete(OnComplete);

            if (enterDelay > 0)
                moveTween.SetDelay(enterDelay);

            TransitionsUtilities.SetTweenToUnscaled(moveTween, unscaledTime);
        }

        private void OnComplete()
        {
            moveTween = null;
        }

        public override void PlayEndTransition()
        {
            if (moveTween != null)
                moveTween.Kill();

            isShowing = false;

            moveTween = Rect.DOAnchorPos(StartPos, exitDuration).SetEase(endEase).OnKill(OnFinishMovement).OnComplete(OnComplete);

            TransitionsUtilities.SetTweenToUnscaled(moveTween, unscaledTime);
        }

        protected override void OnSkip()
        {
            Rect.DOComplete(true);
        }

        private void OnFinishMovement()
        {
            if (IsShowing || !hideWhenExit)
                return;

            gameObject.SetActive(false);
        }

        public override void GoToStartState()
        {
            var rect = GetComponent<RectTransform>();

            rect.anchoredPosition = StartPos;
        }

        public override void GoToEndState()
        {
            var rect = GetComponent<RectTransform>();

            rect.anchoredPosition = EndPos;
        }

        private void OnDestroy()
        {
            transform.DOKill();
        }

        #region Editor Set Positions Buttons

        [Title("SET NEW START AND END POSITIONS - BE CAREFUL", Bold = true)]
        [GUIColor(1, 0, 0)]
        [Button]
        public void SetCurrentPosAsStart()
        {
            var rect = GetComponent<RectTransform>();

            StartPos = rect.anchoredPosition;
        }

        [GUIColor(1, 0, 0)]
        [Button]
        public void SetCurrentPosAsEnd()
        {
            var rect = GetComponent<RectTransform>();

            EndPos = rect.anchoredPosition;
        }

        #endregion
    }
}