using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject background;
    public GameObject textBoxPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Fade());
        TextBox.Text("hello", 0.1f);
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
