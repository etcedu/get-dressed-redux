/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.TheFitCharacterSelect
{
    class TheFitCharacterSelect : MonoBehaviour
    {
        [SerializeField] TheFitScrollView scrollView = default;
        [SerializeField] Button prevCellButton = default;
        [SerializeField] Button nextCellButton = default;

        IEnumerator Start()
        {
            while (GlobalData.Instance == null)
                yield return null;

            scrollView.UpdateData(GlobalData.GetCharacters());
            prevCellButton.onClick.AddListener(scrollView.SelectPrevCell);
            nextCellButton.onClick.AddListener(scrollView.SelectNextCell);
        }
    }
}
