using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

public class Functions : MonoBehaviour
{
    //0 = fadeIn, 1 = fadeOut
    public static IEnumerator Fade(GameObject image, float start, float stop, float speed)
    {
        Color c = new Color(0, 0, 0);
        if (image.transform.GetComponent<Image>()) c = image.transform.GetComponent<Image>().color;
        if (image.transform.GetComponent<Text>()) c = image.transform.GetComponent<Text>().color;
        if (image.transform.GetComponent<SpriteRenderer>()) c = image.transform.GetComponent<SpriteRenderer>().color;
        //if fadeIn is 0, increment upwards, otherwise reverse it.
        float increment = stop >= start ? 1 : -1;
        for (float alpha = start; stop >= start ? alpha <= stop : alpha >= stop; alpha += (increment * Time.deltaTime * speed))
        {
            c.a = alpha;
            if (image.transform.GetComponent<Image>()) image.transform.GetComponent<Image>().color = c;
            if (image.transform.GetComponent<Text>()) image.transform.GetComponent<Text>().color = c;
            if (image.transform.GetComponent<SpriteRenderer>()) image.transform.GetComponent<SpriteRenderer>().color = c;
            yield return null;
        }
    }
    public static float Loop(float time, float start, float max)
    {
        //example
        // 0 % 3 = 0. 0 * 3 = 0.
        // 1 % 3 = 0.3r. 0.3r * 3 = 1
        // 2 % 3 = 0.6r. 0.6r * 3 = 2
        // 3 % 3 = 0. 0 * 3 = 0
        // 4 % 3 = 0.3r. 0.3r * 3 = 1
        // 5 % 3 = 0.6r. 0.6r * 3 = 2
        //return ((time % factor) * factor) + offset;
        //different approach
        return (time % max) + start;
    }
    public static float Oscillate(float time, float start, float max)
    {
        float length = max - start;
        float d = length * 2;
        float value = Loop(time, start, d);
        if (value >= max)
        {
            value = -((value-(start + max)) - length);
        }
        return value;
    }
    public static IEnumerator Zoom(Camera camera, float zoom)
    {
        float currentSize = camera.orthographicSize;
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            camera.orthographicSize = currentSize + (EasingFunctions.EaseOutCubic(i) * zoom);
            yield return null;
        }
    }

    public static IEnumerator WaitFor(Action action)
    {
        action();
        yield return action;
    }

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
    static Vector3 vec;
    /*public static IEnumerator Move(Func<Vector3, Vector3> a, Vector3 b)
    {
        Vector3 position = a.Invoke(Vector3.zero);
        Vector3 desired = b - position;
        print(position);
        print(b);
        print(desired);
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            position = a.Invoke(Vector3.zero);
            desired = b - position;
            //Func<> mouse = mouse + Invokation
            a.Invoke(desired * EasingFunctions.EaseOutCubic(i));
            yield return null;
        }
    }*/
    public static IEnumerator Move(Vector3 a, Vector3 b, Action<Vector3> OP)
    {
        Vector3 desired = b - a;
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            OP.Invoke(a + (desired * EasingFunctions.EaseOutCubic(i)));
            yield return null;
        }
    }

    /*// Action rpg = () => { };
    Func<Vector3, Vector3, Vector3> rpg = (input, value) =>
    {
        Vector3 original = input;
        input = value;
        return original;
    };
    public delegate Vector3 MethodNameDelegate(ref Vector3 y);
    public Vector3 Ret(ref Vector3 vec)
    {
        return vec;
    }*/

    /*float x = 44.93875f;
    x *= 100;
    x = Mathf.Floor(x);
    x /= 100;
    Debug.Log(x); // 44.93*/

    /*public void asf()
    {
        //<Vector3> action = value => position = value;
        //Action<Vector3> action2 = value => position = value;
        //Action<object> test2 = (object o) => a((Vector3)o);
        //a = (arg) => { print(arg); };
        *//*Action<string> greet = name =>
        {
            string greeting = $"Hello {name}!";
            Console.WriteLine(greeting);
        };
        greet("World");*//*
        //print(a.Method.GetParameters()[0].ParameterType);
        //print(a.Method.GetInvocationParameters(true)); //use in foreach

        //Action<object> obj = new Action<object>(o => a((Vector3)o));

        //var method = a.GetType().GetMethod("MyMethod");
        *//*GameManager m = new GameManager();
        var method = a.Method;
        var result = (IEnumerable<Vector3>)method.Invoke(m, null);
        foreach (var item in result)
            Console.WriteLine("Printing:" + item);*/

    /*Vector3 desired = b - position;
    //float FPS = 1.0f / Time.deltaTime;
    for (float i = 0; i <= 1; i += Time.deltaTime)
    {
        a.Invoke(position + (desired * EasingFunctions.EaseOutCubic(i)));
        yield return null;
    }*//*
}*/
}
