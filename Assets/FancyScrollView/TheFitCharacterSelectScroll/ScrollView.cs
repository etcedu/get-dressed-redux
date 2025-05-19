/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using EasingCore;

namespace FancyScrollView.TheFitCharacterSelect
{
    class ScrollView : FancyScrollView<CharacterData>
    {
        [SerializeField] Scroller scroller = default;
        [SerializeField] GameObject cellPrefab = default;
        [SerializeField] SoundVolumePair[] pageChangeSounds;

        Action<int> onSelectionChanged;
        public int currentCell;

        protected override GameObject CellPrefab => cellPrefab;

        protected override void Initialize()
        {
            base.Initialize(); 
            
            scroller.OnValueChanged(UpdatePosition);
            scroller.OnSelectionChanged(UpdateSelection);
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
            return ItemsSource[currentCell];
        }
    }
}
