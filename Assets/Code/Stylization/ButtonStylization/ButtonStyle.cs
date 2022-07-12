using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Stylization
{
    [CreateAssetMenu(fileName = "New button style", menuName = "styling/New button style", order = 1)]
    [System.Serializable]
    public class ButtonStyle : Style
    {

        public Sprite BGSprite;

        public Color BGColor;


        public bool HasTextInside;

        [SerializeField]
        private int textfontsize;
        public int TextFontSize
        {
            get { return textfontsize; }
            set
            {
                if(value >= 0)
                {
                    textfontsize = value;
                }
                else
                {
                    textfontsize = 0;
                }
            }
        }

        public Color Foreground;

        public Font TextFont;

    }

    

}
