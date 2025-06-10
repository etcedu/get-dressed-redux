/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;

namespace FancyScrollView.Example02
{
    class Cell : FancyCell<ItemData, Context>
    {
        [SerializeField] Animator animator = default;
        [SerializeField] Image image = default;
        [SerializeField] Image smallImage = default;

        static class AnimatorHash
        {
            public static readonly int Scroll = Animator.StringToHash("scroll");
        }

        public override void Initialize()
        {
        }

        public override void UpdateContent(ItemData itemData)
        {
            if (itemData.sprite.name.Contains("Shoe"))
            {
                image.color = Color.clear;
                smallImage.color = Color.white;
                smallImage.sprite = itemData.sprite;
            }
            else
            {
                smallImage.color = Color.clear;
                image.color = Color.white;
                image.sprite = itemData.sprite;
            }
        }

        public override void UpdatePosition(float position)
        {
            currentPosition = position;

            if (animator.isActiveAndEnabled)
            {
                animator.Play(AnimatorHash.Scroll, -1, position);
            }

            animator.speed = 0;
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;

        void OnEnable() => UpdatePosition(currentPosition);
    }
}
