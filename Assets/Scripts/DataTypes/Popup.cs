using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace com.medcare360.utt
{
    public class Popup : MonoBehaviour
    {
        public MESSAGE_STATUS popupType;
        public RectTransform rectTransform;
        public TextMeshProUGUI message;
        public Button button;
        public bool active;

        public virtual void Initialize(UnityAction action = null)
        {
            if(rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (rectTransform == null)
            {
                rectTransform = gameObject.AddComponent<RectTransform>();
            }

            InitializeButton(action);
            Hide();
        }

        private void InitializeButton(UnityAction action)
        {
            button.onClick.AddListener(() =>
            {
                action?.Invoke();
                Hide();
            });
        }

        public void Show(string msg)
        {
            active = true;

            message.text = msg;
            message.color = ApplicationConfig.messageColor[popupType];

            rectTransform.anchoredPosition = Vector2.zero;
        }

        public void Hide()
        {
            active = false;
            float width = Screen.width * 2;
            rectTransform.anchoredPosition = new Vector2(-width, 0);
        }
    }
}