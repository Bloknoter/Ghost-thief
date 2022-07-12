using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AICode
{
    public interface IStrategy
    {
        void Init(AIOffline AI);
        void Update();
    }
}
