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
    public static Action<Collider2D, bool> isOverDoor;
    public static Action<GameButtons.TYPE> Back;
}
