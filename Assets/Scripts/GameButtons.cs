using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameButtons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public enum TYPE
    {
        scoreSheetBack,
        pauseBack,
        replayMini,
        resetMini,
        exitToMenu,
        pauseBackYes,
        pauseBackNo,
        settings
    }
    public TYPE type;

    public void OnPointerUp(PointerEventData eventData)
    {
        Actions.Back.Invoke(type);
        /*switch (type)
        {
            case TYPE.back:
                Actions.Back.Invoke(type);
                break;
        }*/
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
}
