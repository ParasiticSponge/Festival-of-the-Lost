using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actions
{
    public static Action Begin;
    public static Action MenuBeginSound;
    public static Action<string> Input;
    public static Action<int> EnterRoom;
    public static Action<GameObject, bool> isOverDoor;
    public static Action<GameButtons.TYPE> Back;
    public static Action Hold;
    public static Action Release;
    public static Action<float> Power;
    public static Action Shot;
    public static Action HitBalloon;
    public static Action<GameObject> Talk;
    public static Action<bool> Settings;
}
