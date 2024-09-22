using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameButtons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public enum TYPE
    {
        back
    }
    public TYPE type;

    public void OnPointerUp(PointerEventData eventData)
    {
        Actions.Back.Invoke(type);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
