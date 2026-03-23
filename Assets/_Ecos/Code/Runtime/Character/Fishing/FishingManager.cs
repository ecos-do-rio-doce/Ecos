using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ecos
{
    public class FishingManager : MonoBehaviour
    {
        public static FishingManager Instance;

        [SerializeField] private bool makeFishPersistent = true;

        [SerializeField] private Image bar;

        [SerializeField] private Fish fishPrefab;

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
    }
}