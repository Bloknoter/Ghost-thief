using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Stylization
{
    [System.Serializable]
    [AddComponentMenu("Stylization/Button style handler")]
    public class ButtonStyleHandler : StyleHandler
    {

        [SerializeField]
        private Image ButtonBG;

        [SerializeField]
        private Text ButtonText;

        public override void OnStyleHasChanged()
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObjects(new Object[]{ ButtonBG, ButtonText}, "button");
#endif
            ButtonBG.sprite = ((ButtonStyle)style).BGSprite;
            ButtonBG.color = ((ButtonStyle)style).BGColor;

            ButtonText.fontSize = ((ButtonStyle)style).TextFontSize;
            ButtonText.color = ((ButtonStyle)style).Foreground;
            ButtonText.font = ((ButtonStyle)style).TextFont;

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(ButtonBG);
            UnityEditor.EditorUtility.SetDirty(ButtonText);
#endif
        }

    }

}
