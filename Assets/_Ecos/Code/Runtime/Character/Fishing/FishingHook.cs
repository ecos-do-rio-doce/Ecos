using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ecos
{
    public class FishingHook : MonoBehaviour
    {
        [SerializeField] private Ease ease;

        [SerializeField] private float barSize = 1000f;
        [SerializeField] private float hookSize = 100f;
        [SerializeField] private UnityEvent<float> onChangeHookSize;

        [SerializeField] private float speed = 1;

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

            MoveToRight();
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
            rectTransform.DOLocalMoveX(0, speed).SetSpeedBased().SetEase(ease).OnComplete(() =>
            {
                MoveToRight();
            });
        }

        [Button]
        public void MoveToRight()
        {
            hookDirection = true;
            rectTransform.DOKill();
            rectTransform.DOLocalMoveX(barSize - hookSize, speed).SetSpeedBased().SetEase(ease).OnComplete(() =>
            {
                MoveToLeft();
            });
        }
        
        [Button]
        public void KillLoop()
        {
            rectTransform.DOKill();
        }

        

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Fish caughtFish = FishingManager.Instance.TryToHookFish(HookPosition, hookSize);
                if (caughtFish != null)
                {
                    hookDirection = !hookDirection;
                    
                    rectTransform.DOKill();

                    Move();

                }             
            }
        }
    }
}