using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stylization
{
    public class Style : ScriptableObject
    {
        [SerializeField]
        protected List<IStyleListener> listeners;

        public int GetListenersAmount()
        {
            if (listeners == null)
                listeners = new List<IStyleListener>();
            return listeners.Count;
        }

        public void AddListener(IStyleListener newlistener)
        {
            if (listeners == null)
            {
                listeners = new List<IStyleListener>();
            }
            if (!listeners.Contains(newlistener) && newlistener != null)
                listeners.Add(newlistener);
        }

        public void RemoveListener(IStyleListener listener)
        {
            if (listeners == null)
            {
                listeners = new List<IStyleListener>();
            }
            listeners.Remove(listener);
        }

        public void OnStyleHasChangedNotification()
        {
            if (listeners == null)
            {
                listeners = new List<IStyleListener>();
                return;
            }
            for (int i = 0; i < listeners.Count; i++)
            {
                if (listeners[i] == null)
                {
                    listeners.RemoveAt(i);
                }
                else
                {
                    listeners[i].OnStyleHasChanged();
                }
            }
        }
    }

    public interface IStyleListener
    {
        void OnStyleHasChanged();
    }
}
