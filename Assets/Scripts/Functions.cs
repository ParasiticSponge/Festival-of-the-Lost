using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.Reflection;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine.Rendering.Universal;

public class Functions : MonoBehaviour
{
    static System.Random random = new System.Random();
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
            if (image.transform.GetComponent<Light2D>()) image.transform.GetComponent<Light2D>().intensity = c.a;
            yield return null;
        }
    }
    public static IEnumerator Shake(GameObject obj, float magnitude, float duration, float speed)
    {
        Vector3 initialPos = obj.transform.position;
        float angle = random.Next(361);
        float percent = random.Next(101);
        float time = 0;

        while (time < duration)
        {
            Vector3 pos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * magnitude * percent, Mathf.Sin(Mathf.Deg2Rad * angle) * magnitude * percent, initialPos.z);
            obj.transform.position = pos;
            time += 0.01f;
            yield return null;
        }
    }
    public static void Shake(GameObject obj, float magnitude, float speed)
    {
        Vector3 initialPos = obj.transform.position;
        float angle = random.Next(361);
        float percent = random.Next(101);
        Vector3 pos = initialPos + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * magnitude * percent, Mathf.Sin(Mathf.Deg2Rad * angle) * magnitude * percent, 0);
        obj.transform.position = pos;
    }
    public static Vector3 Around(Vector3 position, float magnitude)
    {
        float angle = random.Next(361);
        float percent = random.Next(101);
        return position + new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle) * magnitude * percent, Mathf.Sin(Mathf.Deg2Rad * angle) * magnitude * percent, 0);
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
    public static float Loop2(float time, float start, float max)
    {
        float distance = max - start;
        return (time % distance) * Mathf.Sign(distance) + start;
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
    public static IEnumerator Move(Vector3 a, Vector3 b, Action<Vector3> OP, float speed)
    {
        Vector3 desired = b - a;
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i += Time.deltaTime * speed)
        {
            OP.Invoke(a + (desired * EasingFunctions.EasingLinear(i)));
            yield return null;
        }
    }
    public static IEnumerator MoveCubic(Vector3 a, Vector3 b, Action<Vector3> OP)
    {
        Vector3 desired = b - a;
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i += Time.deltaTime)
        {
            OP.Invoke(a + (desired * EasingFunctions.EaseOutCubic(i)));
            yield return null;
        }
    }

    public static void PlayAnimation(Animator animator, string name, bool reversed)
    {
        /*bool count = false;
        for (int i = 0; i < animator.parameterCount; i++)
        {
            if (animator.parameters[i].name == "Speed")
            {
                count = true;
                break;
            }
        }
        //TODO: get correct layer for state
        if (!count)
        {
            //get controller of animator at runtime
            //AnimatorController controller = (animator.runtimeAnimatorController as AnimatorController);
            var controller = animator.runtimeAnimatorController;
            AnimatorControllerParameter paramater = new AnimatorControllerParameter();
            paramater.name = "Speed";
            paramater.type = AnimatorControllerParameterType.Float;
            paramater.defaultFloat = 1;
            controller.AddParameter(paramater);

            ChildAnimatorState[] states = controller.layers[0].stateMachine.states;
            AnimatorState state = states[0].state;

            //get correct state provided by name
            for (int i = 0; i < states.Length; i++)
            {
                if (states[i].state.name == name)
                    state = controller.layers[0].stateMachine.states[i].state;
            }
            //state.speed = animator.GetFloat("Speed");
            print(state.name);
            state.speedParameterActive = true;
            state.speedParameter = "Speed";
        }*/
        if (!animator.enabled) animator.enabled = true;

        switch (reversed)
        {
            case false:
                animator.SetFloat("Speed", 1);
                animator.Play(name, 0, 0);
                break;
            case true:
                animator.SetFloat("Speed", -1);
                animator.Play(name, -1, 1);
                break;
        }
    }




    /*private delegate void GetWidthAndHeight(TextureImporter importer, ref int width, ref int height);
    private static GetWidthAndHeight getWidthAndHeightDelegate;

    public struct Size
    {
        public int width;
        public int height;
    }

    public static Size GetOriginalTextureSize(Texture2D texture)
    {
        if (texture == null)
            throw new NullReferenceException();

        var path = AssetDatabase.GetAssetPath(texture);
        if (string.IsNullOrEmpty(path))
            throw new Exception("Texture2D is not an asset texture.");

        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            throw new Exception("Failed to get Texture importer for " + path);

        return GetOriginalTextureSize(importer);
    }

    public static Size GetOriginalTextureSize(TextureImporter importer)
    {
        if (getWidthAndHeightDelegate == null)
        {
            var method = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            getWidthAndHeightDelegate = Delegate.CreateDelegate(typeof(GetWidthAndHeight), null, method) as GetWidthAndHeight;
        }

        var size = new Size();
        getWidthAndHeightDelegate(importer, ref size.width, ref size.height);

        return size;
    }*/



    /*private static bool GetImageSize(string assetPath, out int width, out int height)
    {
        SpriteDataProviderFactories dataProviderFactories = new SpriteDataProviderFactories();

        dataProviderFactories.Init();

        ISpriteEditorDataProvider importer = dataProviderFactories.GetSpriteEditorDataProviderFromObject(AssetImporter.GetAtPath(assetPath));

        if (importer != null)
        {
            importer.InitSpriteEditorDataProvider();

            ITextureDataProvider textureDataProvider = importer.GetDataProvider<ITextureDataProvider>();

            textureDataProvider.GetTextureActualWidthAndHeight(out width, out height);

            return true;
        }

        width = height = 0;
        return true;
    }*/


    public static bool GetImageSize(Texture2D asset, out int width, out int height)
    {
        if (asset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                width = (int)args[0];
                height = (int)args[1];

                return true;
            }
        }

        height = width = 0;
        return false;
    }
    public static bool GetImageInfo(Texture2D asset, out int width, out int height, out float ppu)
    {
        if (asset != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;

            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                width = (int)args[0];
                height = (int)args[1];
                ppu = importer.spritePixelsPerUnit;
                return true;
            }
        }

        height = width = 0;
        ppu = 0;
        return false;
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
