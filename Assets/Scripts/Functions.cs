using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Functions : MonoBehaviour
{
    public static IEnumerator Fade(GameObject image, int fadeIn)
    {
        Color c = new Color(0, 0, 0);
        if (image.transform.GetComponent<Image>()) c = image.transform.GetComponent<Image>().color;
        if (image.transform.GetComponent<Text>()) c = image.transform.GetComponent<Text>().color;
        //if fadeIn is 0, increment upwards, otherwise reverse it.
        float increment = fadeIn == 0 ? 0.01f : -0.01f;
        for (float alpha = fadeIn; fadeIn == 0 ? alpha <= 1 : alpha >= 0; alpha += increment)
        {
            c.a = alpha;
            if (image.transform.GetComponent<Image>()) image.transform.GetComponent<Image>().color = c;
            if (image.transform.GetComponent<Text>()) image.transform.GetComponent<Text>().color = c;
            yield return null;
        }
    }
    public static float Loop(float time, float factor, float offset)
    {
        //example
        // 0 % 3 = 0. 0 * 3 = 0.
        // 1 % 3 = 0.3r. 0.3r * 3 = 1
        // 2 % 3 = 0.6r. 0.6r * 3 = 2
        // 3 % 3 = 0. 0 * 3 = 0
        // 4 % 3 = 0.3r. 0.3r * 3 = 1
        // 5 % 3 = 0.6r. 0.6r * 3 = 2
        return ((time % factor) * factor) + offset;
    }

    /*public static IEnumerator CodeBlock(Action action)
    {
        action();
        yield return action;
    }*/

    public static int Digits(int value)
    {
        int count = 1;
        while (value > 9)
        {
            value = value / 10;
            count++;
        }
        return count;
    }

    public static Quaternion Quat(float angle, Int32 axis)
    {
        float w = Mathf.Cos(angle / 2 * Mathf.Deg2Rad);
        float component = Mathf.Sin(angle / 2 * Mathf.Deg2Rad);

        axis = (int)(100 / Mathf.Pow(10, axis));
        float x = component * (axis / 100);
        float y = component * (axis % 100 / 10);
        float z = component * (axis % 10);

        return new Quaternion(x, y, z, w);
    }
}
