using DeTach;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ecos
{
    public class FishingHook : MonoBehaviour
    {
        [SerializeField] private bool isReeling;

        [SerializeField] private Ease ease;
        [SerializeField] private float barSize = 1000f;
        [SerializeField] private float hookSize = 100f;
        [SerializeField] private UnityEvent<float> onChangeHookSize;
        [SerializeField] private float delayToStartReeling = 2f;
        [SerializeField] private float speed = 1;
        public float Speed { get => speed; set => speed = value; }


        [SerializeField] private IntVariable progressVariable;

        public float HookPosition => rectTransform.localPosition.x;


        Image image;
        RectTransform rectTransform;

        bool hookDirection = true;


        private void Awake()
        {
            image = GetComponent<Image>();
            rectTransform = image.rectTransform;
        }

        private void Start()
        {
            rectTransform.sizeDelta = new Vector2(hookSize, rectTransform.sizeDelta.y);
            onChangeHookSize?.Invoke(hookSize);

            if (isReeling)
            {
                MoveToRight();
            }
        }

        public void SetIsReeling(bool isReeling)
        {
            this.isReeling = isReeling;

            rectTransform.DOKill();

            if (isReeling)
            {
                rectTransform.localPosition = new Vector3(0, rectTransform.localPosition.y, rectTransform.localPosition.z);
                MoveToRight(true);
            }
        }

        public void Move()
        {
            if (hookDirection)
            {
                MoveToRight();
            }
            else
            {
                MoveToLeft();
            }
        }

        [Button]
        public void MoveToLeft()
        {
            hookDirection = false;
            rectTransform.DOKill();
            rectTransform.DOLocalMoveX(0, Speed).SetSpeedBased().SetEase(ease).OnComplete(() =>
            {
                MoveToRight();
            });
        }

        [Button]
        public void MoveToRight(bool delay = false)
        {
            hookDirection = true;
            rectTransform.DOKill();
            var tween = rectTransform.DOLocalMoveX(barSize - hookSize, Speed).SetSpeedBased().SetEase(ease).OnComplete(() =>
            {
                MoveToLeft();
            });

            if (delay)
            {
                tween.SetDelay(delayToStartReeling);
            }
        }
        
        [Button]
        public void KillLoop()
        {
            rectTransform.DOKill();
        }

        

        private void Update()
        {
            if (!isReeling)
                return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fish caughtFish = FishingManager.Instance.TryToHookFish(HookPosition, hookSize);

                if (caughtFish != null)
                {
                    hookDirection = !hookDirection;
                    
                    rectTransform.DOKill();

                    Move();

                    progressVariable.Value += 1;
                }             
            }
        }
    }
}