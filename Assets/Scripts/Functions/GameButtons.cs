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
        settings,
        cross,
        wiggle,
        PAUSE,
        Power
    }
    public TYPE type;

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.Power:
                Actions.Release.Invoke();
                break;
            case TYPE a when a != TYPE.Power:
                Actions.Back.Invoke(type);
                break;
        }
        /*switch (type)
        {
            case TYPE.back:
                Actions.Back.Invoke(type);
                break;
        }*/
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (type)
        {
            case TYPE.Power:
                Actions.Hold.Invoke();
                break;
        }
    }
}
