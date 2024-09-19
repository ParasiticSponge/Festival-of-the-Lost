using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Functions : MonoBehaviour
{
    public static IEnumerator Fade(GameObject image, int fadeIn)
    {
        Color c = new Color(0, 0, 0);
        if (image.transform.GetComponent<Image>()) c = image.transform.GetComponent<Image>().color;
        if (image.transform.GetComponent<Text>()) c = image.transform.GetComponent<Text>().color;
        //if fadeIn is 0, increment upwards, otherwise reverse it.
        float increment = fadeIn == 0 ? 0.01f : -0.01f;
        for (float alpha = fadeIn; fadeIn == 0 ? alpha <= 1 : alpha >= 0; alpha += increment)
        {
            c.a = alpha;
            if (image.transform.GetComponent<Image>()) image.transform.GetComponent<Image>().color = c;
            if (image.transform.GetComponent<Text>()) image.transform.GetComponent<Text>().color = c;
            yield return null;
        }
    }
}
