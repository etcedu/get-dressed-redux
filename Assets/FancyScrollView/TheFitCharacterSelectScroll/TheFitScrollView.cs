﻿/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using EasingCore;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Pool;
using UnityEngine.Experimental.XR.Interaction;

namespace FancyScrollView.TheFitCharacterSelect
{
    class TheFitScrollView : FancyScrollView<CharacterData>
    {
        [SerializeField] Scroller scroller = default;
        [SerializeField] GameObject cellPrefab = default;
        [SerializeField] SoundVolumePair[] pageChangeSounds;
        [SerializeField] Button continueButton;
        [SerializeField] CanvasGroup contentGroup;

        [SerializeField] GameObject[] hidableUI;
        [SerializeField] GameObject prevButton, nextButton;

        public bool lockInteraction;
        Action<int> onSelectionChanged;
        public int currentCell;
        public bool init = false;

        protected override GameObject CellPrefab => cellPrefab;

        private IEnumerator Start()
        {
            while (!init)
                yield return null;
            yield return new WaitForSeconds(1f);

            int targetCell = Mathf.Clamp(GlobalData.GetLastCharacterIndex(), 0, ItemsSource.Count-1);
            SelectCell(targetCell);
        }

        protected override void Initialize()
        {
            base.Initialize();

            scroller.OnValueChanged(UpdatePosition);
            scroller.OnSelectionChanged(UpdateSelection);
            init = true;
        }

        void UpdateSelection(int index)
        {
            if (currentCell == index)
            {
                return;
            }

            currentCell = index;
            
            Refresh();
            onSelectionChanged?.Invoke(index);

            SFXManager.instance.PlayOneShot(pageChangeSounds[UnityEngine.Random.Range(0, pageChangeSounds.Length)]);

            //for simplicity we're just using index 0 as the tutorial level check
            continueButton.interactable = GlobalData.GetTutorialFinished() || index == 0;
        }

        public void UpdateData(IList<CharacterData> items)
        {
            UpdateContents(items);
            scroller.SetTotalCount(items.Count);
        }

        public void OnSelectionChanged(Action<int> callback)
        {
            onSelectionChanged = callback;
        }

        public void SelectNextCell()
        {
            SelectCell(currentCell + 1);
        }

        public void SelectPrevCell()
        {
            SelectCell(currentCell - 1);
        }

        public void SelectCell(int index)
        {
            if (index < 0 || index >= ItemsSource.Count || index == currentCell)
            {
                return;
            }

            UpdateSelection(index);
            scroller.ScrollTo(index, 0.35f, Ease.OutCubic);
        }

        public CharacterData GetCurrentCharacter()
        {
            GlobalData.SetLastCharacterIndex(currentCell);
            return ItemsSource[currentCell];
        }

        private void Update()
        {
            prevButton.SetActive(currentCell != 0);
            nextButton.SetActive(currentCell != ItemsSource.Count-1);
        }

        public void TapOnCharacter()
        {
            if (continueButton.GetComponentInParent<TweenScale>().isActiveAndEnabled)
                return;

            continueButton.GetComponentInParent<TweenScale>().PlayForward_FromBeginning();
        }

        public void HideUI()
        {
            GetComponent<Scroller>().Draggable = false;
            lockInteraction = true;
            for (int i = 0; i < hidableUI.Length; i++)
            {
                hidableUI[i].SetActive(false);
            }
        }

        public void ShowUI()
        {
            GetComponent<Scroller>().Draggable = true;
            lockInteraction = false;
            for (int i = 0; i < hidableUI.Length; i++)
            {
                hidableUI[i].SetActive(true);
            }
        }
    }
}
