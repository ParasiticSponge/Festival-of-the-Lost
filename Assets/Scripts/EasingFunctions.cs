using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasingFunctions : MonoBehaviour
{
    public enum Type
    {
        Linear,
        Cubic,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInBounce,
        EaseOutBounce,
        EaseInBack,
        Heartbeat,
        HeartbeatLoss
    }

    //domain (input) is between 0->1
    //range (output) can be between -1->2
    //normalised. Easing always starts at 0
    public static float EasingLinear(float time)
    {
        //Mathf.Clamp(time, 0, 1.0f);
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        return time;
    }

    public static float EasingCubic(float time)
    {
        //Mathf.Clamp(time, 0, 1.0f);
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        return Mathf.Pow(time, 3);
    }

    public static float EaseOutBounce(float time)
    {
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (time < 1 / d1)
        {
            return n1 * time * time;
        }
        else if (time < 2 / d1)
        {
            return n1 * (time -= 1.5f / d1) * time + 0.75f;
        }
        else if (time < 2.5 / d1)
        {
            return n1 * (time -= 2.25f / d1) * time + 0.9375f;
        }
        else
        {
            return n1 * (time -= 2.625f / d1) * time + 0.984375f;
        }

    }

    public static float EaseInBounce(float time)
    {
        //Mathf.Clamp(time, 0, 1.0f);
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        return 1 - EaseOutBounce(1 - time);
    }

    public static float EaseInCubic(float time)
    {
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        return time * time * time;
    }

    public static float EaseOutCubic(float time)
    {
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        return 1 - Mathf.Pow(1 - time, 3);
    }

    public static float EaseInOutCubic(float time) 
    {
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        return time < 0.5 ? 4 * time * time * time : 1 - Mathf.Pow(-2 * time + 2, 3) / 2;
    }

    public static float EaseInBack(float time)
    {
        if (time < 0)
            time = 0;
        else if (time > 1)
            time = 1;

        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return c3 * time * time * time - c1 * time * time;
    }
}
