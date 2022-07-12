using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stylization
{
    [CreateAssetMenu(fileName = "New scrollview style", menuName = "styling/New scrollview style", order = 2)]
    [System.Serializable]
    public class ScrollViewStyle : Style
    {
        public Sprite ScrollBarHandlerSprite;
        public Color ScrollBarHandlerColor;

        public Color ScrollBarBGColor;

        public Sprite BGSprite;
        public Color BGColor;

    }
}
