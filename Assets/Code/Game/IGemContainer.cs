using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGemContainer
{
    bool GetGem();
    void AddGem();
    int GemAmount();
    bool CanBeTaken();
    bool CanBeAdded();
}
