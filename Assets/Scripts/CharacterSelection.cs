using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterSelection : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public enum TYPE
    {
        girl1,
        sleepy,
        confirm
    }
    public TYPE type;

    public void OnPointerUp(PointerEventData eventData)
    {
        Actions.ChooseCharacter.Invoke((int)type);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
