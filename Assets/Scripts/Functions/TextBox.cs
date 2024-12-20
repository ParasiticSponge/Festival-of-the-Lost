using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    static string dialogue;
    static string input;
    static bool destroying;
    static bool typed;
    static AudioSource audioSource;
    public static bool dialogueAudio = true;

    static GameObject textBox;
    static Animator mask;
    static Animator appearance;
    static Canvas canvas;
    static GameObject go;
    static TextBox script;

    static GameObject imageChild;
    static GameObject textChild;

    static List<string> texts = new List<string>();
    static List<float> speeds = new List<float>();
    static List<float> intensities = new List<float>();
    static List<string> speakers = new List<string>();
    static List<Sprite> appearances = new List<Sprite>();
    static List<GameObject> objects = new List<GameObject>();

    //static CharacterMovement character;
    //static carMovement character;
    static CharacterController2D character;
    static GameManager gameManager;
    static List<char> pauses = new List<char>() { '.', '?', '!'};

    public static void Text(Sprite image, string speaker, string text, float speed, GameObject obj)
    {
        textBox = Resources.Load<GameObject>("TextVisor");
        canvas = FindObjectOfType<Canvas>();
        speakers.Add(speaker);
        texts.Add(text);
        speeds.Add(speed);
        intensities.Add(0);
        appearances.Add(image);
        objects.Add(obj);
        dialogue = "";

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            go.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = MenuManager_2.textBoxColourLight;
            script = go.AddComponent<TextBox>();
        }
    }
    public static void Text(Sprite image, string speaker, string text, float speed)
    {
        textBox = Resources.Load<GameObject>("TextVisor");
        canvas = FindObjectOfType<Canvas>();
        speakers.Add(speaker);
        texts.Add(text);
        speeds.Add(speed);
        intensities.Add(0);
        appearances.Add(image);
        objects.Add(null);
        dialogue = "";

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            go.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = MenuManager_2.textBoxColourLight;
            script = go.AddComponent<TextBox>();
        }
    }
    public static void Text(Sprite image, string speaker, string text, float speed, float intensity)
    {
        textBox = Resources.Load<GameObject>("TextVisor");
        canvas = FindObjectOfType<Canvas>();
        speakers.Add(speaker);
        texts.Add(text);
        speeds.Add(speed);
        intensities.Add(intensity);
        appearances.Add(image);
        objects.Add(null);
        dialogue = "";

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            go.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = MenuManager_2.textBoxColourLight;
            script = go.AddComponent<TextBox>();
        }
    }
    public static void Text()
    {
        textBox = Resources.Load<GameObject>("TextVisor");
        canvas = FindObjectOfType<Canvas>();
        speakers.Add("");
        texts.Add("I*");
        speeds.Add(0);
        intensities.Add(0);
        appearances.Add(null);
        objects.Add(null);
        dialogue = "";

        if (texts.Count <= 1)
        {
            go = Instantiate(textBox);
            go.transform.SetParent(canvas.transform, false);
            go.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = MenuManager_2.textBoxColourLight;
            script = go.AddComponent<TextBox>();
        }
    }

    private void Start()
    {
        imageChild = transform.GetChild(0).gameObject;
        textChild = transform.GetChild(1).gameObject;

        //character = FindObjectOfType<CharacterMovement>();
        character = FindObjectOfType<CharacterController2D>();
        gameManager = FindObjectOfType<GameManager>();
        //mask = GameObject.Find("TextVisor").GetComponent<Animator>();
        //mask.gameObject.SetActive(true);
        mask = GetComponent<Animator>();
        appearance = imageChild.GetComponent<Animator>();

        mask.Play("ShowDialogue");
        appearance.Play("showCharacter");

        Begin();
    }
    public void Begin()
    {
        if (texts[0] != "I*") StartCoroutine(DisplayText(appearances[0], speakers[0], texts[0], speeds[0], intensities[0], objects[0]));
    }
    IEnumerator DisplayText(Sprite appearance, string speaker, string text, float speed, float intensity, GameObject obj)
    {
        if (dialogueAudio)
            audioSource = Play_Menu_Sounds.CreateClipReturn(3, MenuManager_2.sfxVol);
        if (appearance != null)
        {
            TextBox.appearance.enabled = true;
            imageChild.GetComponent<Image>().color = Color.white;
        }
        else
        {
            TextBox.appearance.enabled = false;
            imageChild.GetComponent<Image>().color = Color.clear;
        }
        imageChild.GetComponent<Image>().sprite = appearance;

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

        textChild.transform.GetChild(3).GetComponent<Text>().text = speaker;
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
        if (gameManager != null) gameManager.canPause = true;
        if (character != null) character.canMove = 0;
        //TODO: tidy pyramid of if statements
        if (texts[0] != "I*")
        {
            textChild.transform.GetChild(2).gameObject.SetActive(false);

            if (dialogue != null)
            {
                if (dialogue.Length == texts[0].Length) textChild.transform.GetChild(1).gameObject.SetActive(true);
                else textChild.transform.GetChild(1).gameObject.SetActive(false);
            }

            if (Input.anyKeyDown && !destroying && dialogue.Length == texts[0].Length)
            {
                Actions.ClickedDialogue.Invoke();
                destroying = true;
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
            if (gameManager != null) gameManager.canPause = false;
            textChild.transform.GetChild(1).gameObject.SetActive(false);
            textChild.transform.GetChild(2).gameObject.SetActive(true);
            ProcessString();
        }
    }

    void ProcessString()
    {
        imageChild.GetComponent<Image>().color = Color.clear;

        if (!typed) dialogue = "Type here...";
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
                if (typed)
                {
                    Actions.Input.Invoke(dialogue);
                    input = dialogue;
                    if (texts.Count > 1)
                    {
                        StartCoroutine(LoadNext());
                    }
                }
            }
            else
            {
                if (!typed)
                {
                    typed = true;
                    dialogue = "";
                }
                dialogue += c;
            }
        }
        gameObject.GetComponentInChildren<Text>().text = dialogue;
    }

    IEnumerator LoadNext()
    {
        if (audioSource != null)
            Destroy(audioSource.gameObject);

        mask.Play("HideDialogue", 0, 0);
        appearance.Play("hideCharacter", 0, 0);

        yield return new WaitForSeconds(0.5f);
        mask.Play("ShowDialogue", 0, 0);
        appearance.Play("showCharacter", 0, 0);

        speakers.Remove(speakers[0]);
        texts.Remove(texts[0]);
        speeds.Remove(speeds[0]);
        intensities.Remove(intensities[0]);
        appearances.Remove(appearances[0]);
        objects.Remove(objects[0]);
        dialogue = "";
        typed = false;

        destroying = false;
        Begin();
    }
    IEnumerator Destroy()
    {
        if (audioSource != null)
            Destroy(audioSource.gameObject);

        mask.Play("HideDialogue", 0, 0);
        appearance.Play("hideCharacter", 0, 0);
        yield return new WaitForSeconds(0.5f);

        speakers.Remove(speakers[0]);
        texts.Remove(texts[0]);
        speeds.Remove(speeds[0]);
        intensities.Remove(intensities[0]);
        appearances.Remove(appearances[0]);
        dialogue = "";
        typed = false;

        if (character != null) character.canMove = 1;
        mask.gameObject.SetActive(false);

        if (objects[0] != null) Actions.FinishTalk.Invoke(objects[0].name);
        objects.Remove(objects[0]);

        destroying = false;
        Destroy(gameObject);
    }
}
