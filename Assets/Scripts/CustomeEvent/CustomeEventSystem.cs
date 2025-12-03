using System;
using master;
using UnityEngine;

public class CustomeEventSystem : SingletonDDOL<CustomeEventSystem>
{
    // start game
    public Action StartPlayAction;
    public void StartPlay()
    {
        StartPlayAction?.Invoke();
    }

    // used Booster Hammer
    public Action<GameObject> UsedBoosterHammerPos;
    public void UserBoosterHammer(GameObject obj)
    {
        UsedBoosterHammerPos.Invoke(obj);
    }


}
