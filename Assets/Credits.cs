using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform transform = GetComponent<RectTransform>();
        Vector2 position = new Vector2(transform.anchoredPosition.x, 3950);
        StartCoroutine(Move());
        IEnumerator Move()
        {
            yield return StartCoroutine(Functions.Move(transform.anchoredPosition, position, value => transform.anchoredPosition = value, 0.1f));
            transform = transform.parent.GetChild(1).GetComponent<RectTransform>();
            yield return StartCoroutine(Functions.Move(transform.anchoredPosition, new Vector2(0, 0), value => transform.anchoredPosition = value, 0.5f));
        }
    }

}
