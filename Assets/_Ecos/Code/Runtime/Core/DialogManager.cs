using UnityEngine;

namespace Ecos
{
    public class DialogManager : MonoBehaviour
    {
        public static DialogManager Instance { get; private set; }

        [Header("Prefab")]
        [SerializeField] private Dialog dialogPrefab;

        [Header("Hierarchy")]
        [SerializeField] private Transform dialogParent;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static Dialog RequestDialog(
            string title,
            string description,
            string confirmMessage,
            string cancelMessage)
        {
            if (Instance == null)
            {
                Instance = FindFirstObjectByType<DialogManager>();

                if (Instance == null)
                {
                    Debug.LogError("DialogManager.RequestDialog was called, but no DialogManager exists in the scene.");
                    return null;
                }
            }

            if (Instance.dialogPrefab == null)
            {
                Debug.LogError("DialogManager has no Dialog prefab assigned.");
                return null;
            }

            Dialog dialog = Instantiate(
                Instance.dialogPrefab,
                Instance.dialogParent != null ? Instance.dialogParent : Instance.transform);

            dialog.Configure(
                title,
                description,
                confirmMessage,
                cancelMessage);

            return dialog;
        }
    }
}