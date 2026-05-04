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

        public Action onFinishFishingAction;

        public float BarSize => bar.rectTransform.sizeDelta.x;

        private List<Fish> instantiatedFishes = new List<Fish>();


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
            if(progress >= targetProgress.Value)
            {
                isFishing.Value = false;

                StartCoroutine(FinishCoroutine());               
            }
        }

        IEnumerator FinishCoroutine()
        {
            yield return new WaitForSeconds(1f);

            onFinishFishing?.Invoke();
            onFinishFishingAction?.Invoke();
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
                    if(fish.RectTransform.localPosition.x < BarSize / 2)
                    {
                        newPos = Random.Range(BarSize / 2f, BarSize - fish.FishSize);
                    }
                    else
                    {
                        newPos = Random.Range(0, BarSize / 2f - fish.FishSize);
                    }

                    fish.RectTransform.localPosition = new Vector3(newPos, fish.RectTransform.localPosition.y);

                    Debug.Log(success);

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

        private void RandomizePosition(Fish fish)
        {
            float xPos = Random.Range(0f, BarSize - fish.FishSize);

            fish.RectTransform.localPosition = Vector3.right * xPos;
        }

        public void StartFishing()
        {
            currentProgress.Value = 0;
            onStartFishing?.Invoke();
        }

        public void OnUpdateCurrentProgress(int currentProgress)
        {

        }
    }
}