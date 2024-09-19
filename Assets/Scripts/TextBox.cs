using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    static string dialogue;
    static string input;
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
        script.intensity = intensity;
    }
    public static void Text()
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        canvas = FindObjectOfType<Canvas>();
        texts.Add("I*");

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            script = go.AddComponent<TextBox>();
        }
        script.intensity = 0;
    }

    private void Start()
    {
        Begin();
    }
    public void Begin()
    {
        if (texts[0] != "I*") StartCoroutine(DisplayText(texts[0], speed, intensity));
    }
    IEnumerator DisplayText(string text, float speed, float intensity)
    {
        if (text.Contains("I*"))
        {
            string[] tokens = text.Split(new[] { "I*" }, StringSplitOptions.None);
            text = tokens[0] + input + tokens[1];
            texts[0] = text;
        }
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
        //TODO: tidy pyramid of if statements
        if (texts[0] != "I*")
        {
            transform.GetChild(2).gameObject.SetActive(false);

            if (dialogue != null)
            {
                if (dialogue.Length == texts[0].Length) transform.GetChild(1).gameObject.SetActive(true);
                else transform.GetChild(1).gameObject.SetActive(false);
            }

            if (Input.anyKeyDown && dialogue.Length == texts[0].Length)
            {
                if (texts.Count > 1)
                {
                    texts.Remove(texts[0]);
                    dialogue = "";
                    Begin();
                }
                else Destroy(gameObject);
            }
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            ProcessString();
        }
    }

    public void ProcessString()
    {
        foreach (char c in Input.inputString)
        {
            if (c == '\b') // has backspace/delete been pressed?
            {
                if (dialogue.Length != 0)
                {
                    dialogue = dialogue.Substring(0, dialogue.Length - 1);
                }
            }
            else if ((c == '\n') || (c == '\r')) // enter/return
            {
                Actions.Input.Invoke(dialogue);
                input = dialogue;
                texts.Remove(texts[0]);
                dialogue = "";
                if (texts.Count > 1) Begin();
            }
            else
            {
                dialogue += c;
            }
        }
        gameObject.GetComponentInChildren<Text>().text = dialogue;
    }

    public static void UpdateOnInput(string textToUpdate)
    {

    }
}
