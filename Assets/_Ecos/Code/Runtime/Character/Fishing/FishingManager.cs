using DeTach;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using System;
using Random = UnityEngine.Random;

namespace Ecos
{
    public class FishingManager : MonoBehaviour
    {
        public static FishingManager Instance;

        [SerializeField] private UnityEvent onStartFishing;
        [SerializeField] private UnityEvent onFinishFishing;
        [SerializeField] private UnityEvent delayedStopFishing;

        [SerializeField] private bool makeFishPersistent = true;

        [SerializeField] private Image bar;

        [SerializeField] private Fish fishPrefab;

        [SerializeField] private BoolVariable isFishing;
        [SerializeField] private IntVariable currentProgress;
        [SerializeField] private IntVariable targetProgress;
        [SerializeField] private FloatVariable hookSpeed;
        [SerializeField] private SpriteVariable fishSprite;

        public Action<FishDef> onFinishFishingAction;

        public float BarSize => bar.rectTransform.sizeDelta.x;


        private List<Fish> instantiatedFishes = new List<Fish>();

        FishDef currentFish;
        public FishDef CurrentFish { get => currentFish; set => currentFish = value; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            currentProgress.Event.OnChange += OnChangeProgress;
        }

        private void OnChangeProgress(int progress)
        {
            if (progress >= targetProgress.Value)
            {
                isFishing.Value = false;

                StartCoroutine(FinishCoroutine());
            }
        }

        IEnumerator FinishCoroutine()
        {
            yield return new WaitForSeconds(1f);

            onFinishFishing?.Invoke();
            onFinishFishingAction?.Invoke(CurrentFish);
        }

        private void Start()
        {
            CreateFish();
        }

        public Fish TryToHookFish(float position, float size)
        {
            foreach (var fish in instantiatedFishes)
            {
                Vector2 contactArea = new Vector2(position, position + size);

                bool success = IsInsideArea(fish.FishPos, contactArea) || IsInsideArea(fish.FishPos + fish.FishSize, contactArea);

                if (success)
                {
                    float newPos;
                    if (fish.RectTransform.localPosition.x < BarSize / 2)
                    {
                        newPos = Random.Range(BarSize / 2f, BarSize - fish.FishSize);
                    }
                    else
                    {
                        newPos = Random.Range(0, BarSize / 2f - fish.FishSize);
                    }

                    if (currentProgress.Value + 1 < targetProgress.Value)
                    {
                        fish.RectTransform.localPosition = new Vector3(newPos, fish.RectTransform.localPosition.y);
                    }

                    return fish;
                }
            }

            return null;
        }

        private bool IsInsideArea(float pos, Vector2 area)
        {
            return pos > area.x && pos < area.y;
        }

        [Button]
        private void CreateFish()
        {
            Fish fish = Instantiate(fishPrefab, bar.transform);
            RandomizePosition(fish);
            instantiatedFishes.Add(fish);
        }

        private void RandomizePosition(Fish fish, bool firstTime = false)
        {
            float xPos = Random.Range(0f, BarSize - fish.FishSize);

            if (firstTime && xPos < BarSize / 2f)
            {
                Debug.Log("Offsetting initial pos");
                xPos = +(BarSize / 4f);
            }

            fish.RectTransform.localPosition = Vector3.right * xPos;
        }

        public void StartFishing(FishDef def)
        {
            CurrentFish = def;
            currentProgress.Value = 0;
            targetProgress.Value = def.ProgressToCapture;
            hookSpeed.Value = def.Speed;
            fishSprite.Value = def.FishSprite;
            onStartFishing?.Invoke();
        }

        public void UpdateFishSprite(Sprite sprite)
        {
            instantiatedFishes.ForEach(fish => fish.GetComponentInChildren<Image>().sprite = sprite);
        }

        public void OnUpdateCurrentProgress(int currentProgress)
        {

        }
    }
}