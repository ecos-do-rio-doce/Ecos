using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gaton.Transitions
{
    [AddComponentMenu("- Transitions/Change Position 3d Obj", order: 3)]
    public class ChangePosition3dObj : Transition
    {
        [Title("Positions")]
        [SerializeField] Vector3 startPos;

        [SerializeField] Vector3 endPos;

        [Title("Tween Values")]
        [SerializeField] float enterDuration;

        [SerializeField] float exitDuration;

        [SerializeField] float enterDelay = 0f;

        [SerializeField] Ease enterEase = Ease.OutExpo;
        [SerializeField] Ease exitEase = Ease.OutExpo;

        [Title("Conditions")]
        [SerializeField] bool setStartPositionOnAwake = true;
        [SerializeField] bool hideWhenExit = true;

        bool isShowing = false;

        public bool IsShowing { get => isShowing; }

        public override float StartDuration => enterDuration + enterDelay;
        public override float EndDuration => exitDuration;

        public Vector3 StartPos { get => startPos; set => startPos = value; }
        public Vector3 EndPos { get => endPos; set => endPos = value; }

        Tween moveTween;

        public new void Awake()
        {
            if (setStartPositionOnAwake)
                transform.localPosition = StartPos;

            base.Awake();
        }

        public override void PlayStartTransition()
        {
            KillTween();

            isShowing = true;

            gameObject.SetActive(true);

            moveTween = transform.DOLocalMove(EndPos, enterDuration).SetEase(enterEase);

            if (enterDelay > 0)
                moveTween.SetDelay(enterDelay);

            TransitionsUtilities.SetTweenToUnscaled(moveTween, unscaledTime);
        }


        public override void PlayEndTransition()
        {
            KillTween();

            isShowing = false;

            moveTween = transform.DOLocalMove(StartPos, exitDuration).SetEase(exitEase).OnKill(OnFinishMovement);

            TransitionsUtilities.SetTweenToUnscaled(moveTween, unscaledTime);
        }

        private void KillTween()
        {
            if (moveTween != null && moveTween.IsPlaying())
            {
                moveTween.Kill();
            }
        }

        protected override void OnSkip()
        {
            transform.DOComplete(true);
        }

        private void OnFinishMovement()
        {
            moveTween = null;

            if (IsShowing || !hideWhenExit)
                return;

            gameObject.SetActive(false);
        }

        public override void GoToStartState()
        {
            transform.localPosition = startPos;
        }

        public override void GoToEndState()
        {
            transform.localPosition = endPos;
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
            StartPos = transform.localPosition;
        }

        [GUIColor(1, 0, 0)]
        [Button]
        public void SetCurrentPosAsEnd()
        {
            EndPos = transform.localPosition;
        }

        #endregion
    }
}