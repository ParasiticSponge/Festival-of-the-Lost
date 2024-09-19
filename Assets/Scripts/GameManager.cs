using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject background;
    public GameObject textBoxPrefab;
    public CharacterMovement character;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade());
        TextBox.Text("hello", 0.05f);
        TextBox.Text("Who are you?", 0.05f);
        TextBox.Text();
        TextBox.Text("...", 0.1f);
        //TextBox.Text($"Oh! Your name is {character.charName}?", 0.05f, true);
        //I* ouputs the input of the player
        TextBox.Text("Oh! Your name is I*?", 0.05f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Fade()
    {
        Color c = background.transform.GetComponent<Image>().color;
        for (float alpha = 1f; alpha >= 0; alpha -= 0.01f * 0.5f)
        {
            c.a = alpha;
            background.transform.GetComponent<Image>().color = c;
            yield return null;
        }
    }
}
