using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;
using static Ecos.SetAnimatorParams;

namespace Ecos
{
    public class WorldFishing : MonoBehaviour
    {
        [Title("Components")]
        [SerializeField] private Transform caughtFishParent;
        [SerializeField] private SpriteRenderer caughtFishPrefab;
        [SerializeField] private WaterDetection waterDetection;
        [SerializeField] private SetAnimatorParams setAnimatorParams;

        public WaterDetection WaterDetection { get => waterDetection; set => waterDetection = value; }

        [Title("Values")]
        [SerializeField] private Vector2 rangeToBite = new Vector2(3, 6);

        [Title("Events")]
        [SerializeField] private UnityEvent castFishingLine = null;
        public UnityEvent CastFishingLine { get => castFishingLine; set => castFishingLine = value; }

        [SerializeField] private UnityEvent<bool> setCanWalk;
        public bool SetCanWalk { set { setCanWalk?.Invoke(value); }  }

        [SerializeField] private UnityEvent<bool> showFishAppearedIndicator;
        public UnityEvent<bool> ShowFishAppearedIndicator { get => showFishAppearedIndicator; set => showFishAppearedIndicator = value; }
        
        public FishSource CurrentFishSource { get; set; }


        State currentState;
        

        public void ChangeState(State state)
        {
            if (this.currentState != null)
                this.currentState.OnExitState();

            currentState = state;
            currentState.worldFishing = this;
            currentState.OnEnterState();
        }

        public void SetAnimatorState(FishingAnimState animState)
        {
            setAnimatorParams.SetIntParameter((int)animState);
        }

        private void Awake()
        {
            ChangeState(new NotFishingState());
        }

        private void Update()
        {
            currentState.Update();
        }

        public bool DetectedMainActionButton()
        {
            return Input.GetMouseButtonDown(0);
        }

        public SpriteRenderer CreateCaughtFish(FishDef fishDef)
        {
            var instantiatedFish = Instantiate(caughtFishPrefab, caughtFishParent);

            instantiatedFish.sprite = fishDef.FishSprite;

            return instantiatedFish;
        }
    }
}
