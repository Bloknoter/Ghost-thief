using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Stylization
{
    [System.Serializable]
    [AddComponentMenu("Stylization/Scrollview style handler")]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollViewStyleHandler : StyleHandler
    {
        public Image BG;

        public enum ScrollingTypeEnum
        {
            Horizontal, 
            Vertical,
            Horizontal_and_Vertical
        }

        public ScrollingTypeEnum scrollingType = ScrollingTypeEnum.Vertical;

        public Image VerticalScrollBarHandler;
        public Image VerticalScrollBarBG;
        public Image HorizontalScrollBarHandler;
        public Image HorizontalScrollBarBG;

        public override void OnStyleHasChanged()
        {
            /*#if UNITY_EDITOR
                        UnityEditor.Undo.RecordObjects(new Object[] { VerticalScrollBarHandler, VerticalScrollBarBG, HorizontalScrollBarHandler, HorizontalScrollBarBG }, "scrollview");
            #endif*/

            BG.color = ((ScrollViewStyle)style).BGColor;
            BG.sprite = ((ScrollViewStyle)style).BGSprite;

            if (VerticalScrollBarHandler != null)
            {
                VerticalScrollBarHandler.sprite = ((ScrollViewStyle)style).ScrollBarHandlerSprite;
                VerticalScrollBarHandler.color = ((ScrollViewStyle)style).ScrollBarHandlerColor;
                VerticalScrollBarBG.color = ((ScrollViewStyle)style).ScrollBarBGColor;
            }

            if (HorizontalScrollBarHandler != null)
            {
                HorizontalScrollBarHandler.sprite = ((ScrollViewStyle)style).ScrollBarHandlerSprite;
                HorizontalScrollBarHandler.color = ((ScrollViewStyle)style).ScrollBarHandlerColor;
                HorizontalScrollBarBG.color = ((ScrollViewStyle)style).ScrollBarBGColor;
            }

#if UNITY_EDITOR
            if (VerticalScrollBarHandler != null)
                UnityEditor.EditorUtility.SetDirty(VerticalScrollBarHandler);
            if (VerticalScrollBarBG != null)
                UnityEditor.EditorUtility.SetDirty(VerticalScrollBarBG);
            if (HorizontalScrollBarHandler != null)
                UnityEditor.EditorUtility.SetDirty(HorizontalScrollBarHandler);
            if (HorizontalScrollBarBG != null)
                UnityEditor.EditorUtility.SetDirty(HorizontalScrollBarBG);
#endif
        }
    }

    
}
