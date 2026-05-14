using DG.Tweening;
using UnityEngine;

namespace Ecos
{
    public class CaughtFishState : State
    {
        public float timeToReturn = 0.5f;

        bool hasFinishedDialog = false;

        SpriteRenderer createdFish;

        public override void OnEnterState()
        {
            hasFinishedDialog = false;

            worldFishing.ShowFishAppearedIndicator?.Invoke(false);
            worldFishing.SetAnimatorState(SetAnimatorParams.FishingAnimState.CaughtFish);
            worldFishing.GetComponentInParent<TopDownCharacterController>().transform.DORotate(new Vector3(0, 180, 0), 0.6f).SetDelay(0.2f);

            createdFish = worldFishing.CreateCaughtFish(FishingManager.Instance.CurrentFish);
            var targetScale = createdFish.transform.localScale;
            createdFish.transform.localScale = Vector3.zero;
            createdFish.transform.DOScale(targetScale, 0.5f);

            var dialog = DialogManager.RequestDialog($"Pegou {FishingManager.Instance.CurrentFish.FishName}!", "Você deseja devolver esse peixe?", "Devolver", "Não devolver");

            Debug.Log($"Should release? {!worldFishing.CurrentFishSource.IsAllowed(FishingManager.Instance.CurrentFish)}");

            dialog.OnConfirm += () => 
            {
                OnSelectDialog(false);
            };
            
            dialog.OnCancel += () => 
            {
                OnSelectDialog(true);
            };
        }

        private void OnSelectDialog(bool capturedFish)
        {
            hasFinishedDialog = true;

            PlayerScore.Instance.FinishCurrentAttempt(capturedFish);
        }

        public override void Update()
        {
            if (!hasFinishedDialog)
                return;

            timeToReturn -= Time.deltaTime;

            if(timeToReturn < 0f)
            {
                createdFish.transform.DOScale(0f, 0.2f).SetDelay(0.4f).OnComplete(() => GameObject.Destroy(createdFish.gameObject));
                worldFishing.ChangeState(new NotFishingState());
            }
        }

        public override void OnExitState()
        {
            
        }

        
    }
}
