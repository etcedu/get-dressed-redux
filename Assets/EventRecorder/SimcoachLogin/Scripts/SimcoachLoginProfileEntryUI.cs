using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimcoachGames.EventRecorder.Login
{
    public class SimcoachLoginProfileEntryUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI profileTextUI;
        [SerializeField] Button selectButton;
        [SerializeField] Button deleteButton;

        string _profileName;
        int _num;

        Action<string> _onSelectCallback;

        public void Init(
            string profileName,
            int number,
            bool isCurrentlyActive,
            Action onSelectCallback,
            Action onDeleteCallback)
        {
            _profileName = profileName;
            _num = number;

            profileTextUI.text = profileName;
            
            selectButton.interactable = !isCurrentlyActive; //Currently active profile should be not selectable
            selectButton.onClick = new Button.ButtonClickedEvent();
            selectButton.onClick.AddListener(onSelectCallback.Invoke);

            deleteButton.gameObject.SetActive(profileName != SimcoachLoginUserManagement.DefaultProfileName);
            deleteButton.onClick = new Button.ButtonClickedEvent();
            deleteButton.onClick.AddListener(onDeleteCallback.Invoke);
            
            gameObject.SetActive(true);
        }
    }
}