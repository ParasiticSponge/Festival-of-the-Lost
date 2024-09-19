using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Intro());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Intro()
    {
        yield return StartCoroutine(Functions.Fade(text, 0));
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(Functions.Fade(text, 1));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
