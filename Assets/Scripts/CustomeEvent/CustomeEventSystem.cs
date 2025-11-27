using System;
using master;
using UnityEngine;

public class CustomeEventSystem : SingletonDDOL<CustomeEventSystem>
{
    public Action StartPlayAction;
    public void StartPlay()
    {
        StartPlayAction?.Invoke();
    }
}
