using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Ecos
{
    public class PlayerScore : MonoBehaviour
    {
        public static PlayerScore Instance;

        [Title("Values")]
        [SerializeField] int maximumCatchesPerDay = 4;

        [Title("Scoring System")]
        [SerializeField] int scorePerPerfectCatch = 30;
        [SerializeField] float modifierPerCorrectFish = 1f;
        [SerializeField] float modifierPerWrongFish = 0.5f;
        [SerializeField] AnimationCurve missedTimingRewardCurve;
        [SerializeField] AnimationCurve wrongClicksRewardCurve;

        [Title("Debug")]
        public List<FishDef> correctFishes;
        public List<FishDef> wrongFishes;

        public List<AttemptInfos> allAttempts = new List<AttemptInfos>();

        public AttemptInfos currentAttempt;

        public int ScorePerPerfectCatch { get => scorePerPerfectCatch; set => scorePerPerfectCatch = value; }
        public float ModifierPerCorrectFish { get => modifierPerCorrectFish; set => modifierPerCorrectFish = value; }
        public float ModifierPerWrongFish { get => modifierPerWrongFish; set => modifierPerWrongFish = value; }
        public AnimationCurve MissedTimingRewardCurve => missedTimingRewardCurve;
        public AnimationCurve WrongClicksRewardCurve => wrongClicksRewardCurve;

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            //allAttempts = new List<AttemptInfos>();
            
        }

        public void FinishCurrentAttempt(bool capturedFish)
        {
            currentAttempt.wasCaptured = capturedFish;

            allAttempts.Add(currentAttempt);

            currentAttempt = null;


            if(allAttempts.Count >= maximumCatchesPerDay)
            {
                Debug.Log("Finish");

                FindFirstObjectByType<EndgameScreen>().Play();
            }
        }
    }
}
