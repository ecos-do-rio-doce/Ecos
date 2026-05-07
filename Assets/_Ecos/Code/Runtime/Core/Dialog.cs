using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Ecos
{
    public class Dialog : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Buttons")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        public event Action OnConfirm;
        public event Action OnCancel;

        public void Configure(
            string title,
            string description,
            string confirmMessage,
            string cancelMessage)
        {
            titleText.text = title;
            descriptionText.text = description;

            confirmButton.GetComponentInChildren<TMP_Text>().text = confirmMessage;
            cancelButton.GetComponentInChildren<TMP_Text>().text = cancelMessage;

            confirmButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();

            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(Cancel);

            cancelButton.gameObject.SetActive(!string.IsNullOrWhiteSpace(cancelMessage));
        }

        private void Confirm()
        {
            OnConfirm?.Invoke();
            
            Close();
        }

        private void Cancel()
        {
            OnCancel?.Invoke();
            
            Close();
        }

        public void Close()
        {
            Destroy(gameObject);
        }
    }
}