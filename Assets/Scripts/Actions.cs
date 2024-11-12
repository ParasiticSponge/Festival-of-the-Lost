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
    //public static Action<float> Power;
    public static Action<Vector3> Power;
    public static Action<float> Bow;
    public static Action<bool> HitBalloon;
    public static Action<GameObject> Talk;
    public static Action<bool> Settings;
    public static Action<int> TextBoxColour;
    public static Action<string> FinishTalk;
    public static Action Pause;
    public static Action<bool, Action<bool>> Toggles;
    public static Action<int> starCount;
    public static Action<int> BalloonType;
    public static Action<GameObject> RideCart;
    public static Action<float> BulletHit;
    public static Action<int> BossPhase;
    public static Action<GameObject> FoundPlushie;
}
