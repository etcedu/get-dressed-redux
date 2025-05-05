/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

namespace FancyScrollView.TheFitCharacterSelect
{
    class ItemData
    {
        public string Title { get; }
        public string Description { get; }
        public string AssetPath { get; }
        public string Tag { get; }

        public ItemData(string title, string description, string assetPath, string tag)
        {
            Title = title;
            Description = description;
            AssetPath = assetPath;
            Tag = tag;
        }
    }
}
