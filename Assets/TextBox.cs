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

    public string text;
    public float speed;
    public float intensity;

    public static void Text(string text, float speed)
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        canvas = FindObjectOfType<Canvas>();
        GameObject go = Instantiate(textBox);
        go.transform.SetParent(canvas.transform, false);
        TextBox script = go.AddComponent<TextBox>();
        script.text = text;
        script.speed = speed;
        script.intensity = 0;
    }
    public static void Text(string text, float speed, float intensity)
    {
        textBox = FindObjectOfType<GameManager>().textBoxPrefab;
        GameObject go = Instantiate(textBox);
        TextBox script = go.AddComponent<TextBox>();
        script.text = text;
        script.speed = speed;
        script.intensity = intensity;
    }
    private void Start()
    {
        StartCoroutine(DisplayText(text, speed, intensity));
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
        if (Input.anyKeyDown) Destroy(gameObject);
    }
}
