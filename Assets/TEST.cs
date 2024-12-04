using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TEST : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject go = GameObject.Find("Main");
        go.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -1f);
        go = GameObject.Find("Settings");
        go.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -1f);
        go = GameObject.Find("Dev");
        go.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, -1f);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
