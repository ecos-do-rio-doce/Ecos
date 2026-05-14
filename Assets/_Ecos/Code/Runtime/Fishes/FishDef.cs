using UnityEngine;

namespace Ecos
{
    [CreateAssetMenu(fileName = "Fish", menuName = "Ecos/Fish")]
    public class FishDef : ScriptableObject
    {
        [SerializeField] private string fishName = "Peixe";
        public string FishName { get => fishName; set => fishName = value; }

        [SerializeField] private Sprite fishSprite = null;
        public Sprite FishSprite { get => fishSprite; set => fishSprite = value; }

        [SerializeField] private int size = 100;
        public int Size { get => size; set => size = value; }

        [SerializeField] private int progressToCapture = 10;
        public int ProgressToCapture { get => progressToCapture; set => progressToCapture = value; }
        [SerializeField] private float speed = 1000f;
        public float Speed { get => speed; set => speed = value; }

        [SerializeField] int pointsValue;
        public int PointsValue { get => pointsValue; set => pointsValue = value; }
    }
}
