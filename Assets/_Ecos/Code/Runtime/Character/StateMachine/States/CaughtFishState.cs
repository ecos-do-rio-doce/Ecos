using UnityEngine;

namespace Ecos
{
    public class CaughtFishState : State
    {
        public float timeToReturn = 3f;

        bool hasFinishedDialog = false;

        public override void OnEnterState()
        {
            hasFinishedDialog = false;

            worldFishing.ShowFishAppearedIndicator?.Invoke(false);
            worldFishing.SetAnimatorState(SetAnimatorParams.FishingAnimState.CaughtFish);

            var dialog = DialogManager.RequestDialog("Pegou {nomePeixe}!", "Você deseja devolver esse peixe?", "Devolver", "Não devolver");
            dialog.OnConfirm += () => { Debug.Log("Devolveu!"); hasFinishedDialog = true; };
            dialog.OnCancel += () => { Debug.Log("Não devolveu!"); hasFinishedDialog = true; };
        }

        public override void Update()
        {
            if (!hasFinishedDialog)
                return;

            timeToReturn -= Time.deltaTime;

            if(timeToReturn < 0f)
            {
                worldFishing.ChangeState(new NotFishingState());
            }
        }

        public override void OnExitState()
        {
            
        }

        
    }
}
