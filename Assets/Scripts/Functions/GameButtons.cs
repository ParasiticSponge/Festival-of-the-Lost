using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameButtons : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum TYPE
    {
        scoreSheetBack,
        pauseBack,
        replayMini,
        resetMini,
        pauseBackFromGameToMenu,
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
        StartCoroutine(Play_Menu_Sounds.PlayClip(12, MenuManager_2.sfxVol));
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
    public void OnPointerEnter(PointerEventData eventData)
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");
        if (character.GetComponent<BossDartController>())
            character.GetComponent<BossDartController>().fire = true;
        if (character.GetComponent<RodController>())
            character.GetComponent<RodController>().fire = true;
        if (character.GetComponent<MouseController2D>())
            character.GetComponent<MouseController2D>().fire = true;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        GameObject character = GameObject.FindGameObjectWithTag("Character");
        if (character.GetComponent<BossDartController>())
            character.GetComponent<BossDartController>().fire = false;
        if (character.GetComponent<RodController>())
            character.GetComponent<RodController>().fire = false;
        if (character.GetComponent<MouseController2D>())
            character.GetComponent<MouseController2D>().fire = false;
    }
}
