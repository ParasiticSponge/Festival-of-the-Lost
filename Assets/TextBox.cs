using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    static string dialogue;
    static float shakeIntensity;
    static GameObject textBox;
    static Canvas canvas;
    static GameObject go;
    static TextBox script;

    public static List<string> texts = new List<string>();
    public float speed;
    public float intensity;

    public static void Text(string text, float speed)
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        canvas = FindObjectOfType<Canvas>();
        texts.Add(text);

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            script = go.AddComponent<TextBox>();
        }
        script.speed = speed;
        script.intensity = 0;
    }
    public static void Text(string text, float speed, float intensity)
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        GameObject go = Instantiate(textBox);
        TextBox script = go.AddComponent<TextBox>();
        texts.Add(text);
        script.speed = speed;
        script.intensity = intensity;
    }
    private void Start()
    {
        Begin();
    }
    public void Begin()
    {
        StartCoroutine(DisplayText(texts[0], speed, intensity));
    }
    IEnumerator DisplayText(string text, float speed, float intensity)
    {
        yield return new WaitForSeconds(speed);
        for (int i = 0; i < text.Length; i++)
        {
            dialogue += text[i];
            gameObject.GetComponentInChildren<Text>().text = dialogue;
            yield return new WaitForSeconds(speed);
        }
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if(texts.Count > 1)
            {
                texts.Remove(texts[0]);
                dialogue = "";
                Begin();
            }
            else Destroy(gameObject);
        }
    }
}
