using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stylization
{
    [ExecuteAlways]
    public abstract class StyleHandler : MonoBehaviour, IStyleListener
    {
        [SerializeField]
        protected Style style;

        private void OnEnable()
        {
            if (style != null)
                style.AddListener(this);
        }

        private void OnDestroy()
        {
            if (style != null)
                style.RemoveListener(this);
        }

        public abstract void OnStyleHasChanged();
    }
}
