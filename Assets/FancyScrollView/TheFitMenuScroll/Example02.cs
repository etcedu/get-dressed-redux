/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example02
{
    class Example02 : MonoBehaviour
    {
        [SerializeField] ScrollView scrollView = default;
        [SerializeField] List<Sprite> sprites;
        [SerializeField] float scrollDelay = 2.0f;

        void Start()
        {
            scrollView.OnSelectionChanged(OnSelectionChanged);

            sprites.Shuffle();
            var items = Enumerable.Range(0, sprites.Count)
                .Select(i => new ItemData(sprites[i]))
                .ToArray();

            scrollView.UpdateData(items);
            scrollView.SelectCell(0);

            StartCoroutine(ScrollForever());
        }

        IEnumerator ScrollForever()
        {
            while (true)
            {
                scrollView.SelectNextCell();
                yield return new WaitForSeconds(scrollDelay);
            }
        }

        void OnSelectionChanged(int index)
        {
        }
    }
}
