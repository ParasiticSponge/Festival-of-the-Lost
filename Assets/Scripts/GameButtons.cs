using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameButtons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public enum TYPE
    {
        back,
    }
    public TYPE type;

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.back:
                Actions.Back.Invoke(type);
                break;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
