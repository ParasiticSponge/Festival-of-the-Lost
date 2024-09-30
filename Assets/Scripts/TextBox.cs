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

    static GameObject textBox;
    static Animator mask;
    static Canvas canvas;
    static GameObject go;
    static TextBox script;

    static List<string> texts = new List<string>();
    static List<float> speeds = new List<float>();
    static List<float> intensities = new List<float>();
    static List<string> speakers = new List<string>();

    //static CharacterMovement character;
    //static carMovement character;
    static CharacterController2D character;
    static List<char> pauses = new List<char>() { '.', '?', '!'};

    public static void Text(string speaker, string text, float speed)
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        canvas = FindObjectOfType<Canvas>();
        speakers.Add(speaker);
        texts.Add(text);
        speeds.Add(speed);
        intensities.Add(0);

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            script = go.AddComponent<TextBox>();
        }
    }
    public static void Text(string speaker, string text, float speed, float intensity)
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        canvas = FindObjectOfType<Canvas>();
        speakers.Add(speaker);
        texts.Add(text);
        speeds.Add(speed);
        intensities.Add(intensity);

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            script = go.AddComponent<TextBox>();
        }
    }
    public static void Text()
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        canvas = FindObjectOfType<Canvas>();
        speakers.Add("");
        texts.Add("I*");
        speeds.Add(0);
        intensities.Add(0);

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            script = go.AddComponent<TextBox>();
        }
    }

    private void Start()
    {
        //character = FindObjectOfType<CharacterMovement>();
        character = FindObjectOfType<CharacterController2D>();
        //mask = GameObject.Find("TextVisor").GetComponent<Animator>();
        //mask.gameObject.SetActive(true);
        mask = GetComponent<Animator>();
        mask.Play("ShowDialogue");
        Begin();
    }
    public void Begin()
    {
        if (texts[0] != "I*") StartCoroutine(DisplayText(speakers[0], texts[0], speeds[0], intensities[0]));
    }
    IEnumerator DisplayText(string speaker, string text, float speed, float intensity)
    {
        if (text.Contains("I*"))
        {
            string[] tokens = text.Split(new[] { "I*" }, StringSplitOptions.None);
            text = tokens[0] + input + tokens[1];
            texts[0] = text;
        }
        if (speaker.Contains("I*"))
        {
            speaker = input;
            speakers[0] = speaker;
        }

        transform.GetChild(0).GetChild(3).GetComponent<Text>().text = speaker;
        yield return new WaitForSeconds(speed);
        for (int i = 0; i < text.Length; i++)
        {
            dialogue += text[i];
            gameObject.GetComponentInChildren<Text>().text = dialogue;
            foreach (char c in pauses)
            {
                if (text[i] == c) yield return new WaitForSeconds(speed + 0.5f);
                else yield return new WaitForSeconds(speed);
            }
        }
    }
    private void Update()
    {
        character.canMove = 0;
        //TODO: tidy pyramid of if statements
        if (texts[0] != "I*")
        {
            transform.GetChild(0).GetChild(2).gameObject.SetActive(false);

            if (dialogue != null)
            {
                if (dialogue.Length == texts[0].Length) transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                else transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            }

            if (Input.anyKeyDown && dialogue.Length == texts[0].Length)
            {
                if (texts.Count > 1)
                {
                    StartCoroutine(LoadNext());
                }
                else
                {
                    StartCoroutine(Destroy());
                }
            }
        }
        else
        {
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            ProcessString();
        }
    }

    void ProcessString()
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
                if (texts.Count > 1)
                {
                    StartCoroutine(LoadNext());
                }
            }
            else
            {
                dialogue += c;
            }
        }
        gameObject.GetComponentInChildren<Text>().text = dialogue;
    }

    IEnumerator LoadNext()
    {
        mask.Play("HideDialogue", 0, 0);
        yield return new WaitForSeconds(0.5f);
        mask.Play("ShowDialogue", 0, 0);

        speakers.Remove(speakers[0]);
        texts.Remove(texts[0]);
        speeds.Remove(speeds[0]);
        intensities.Remove(intensities[0]);
        dialogue = "";

        Begin();
    }
    IEnumerator Destroy()
    {
        mask.Play("HideDialogue", 0, 0);
        yield return new WaitForSeconds(0.5f);

        speakers.Remove(speakers[0]);
        texts.Remove(texts[0]);
        speeds.Remove(speeds[0]);
        intensities.Remove(intensities[0]);
        dialogue = "";

        character.canMove = 1;
        mask.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}
