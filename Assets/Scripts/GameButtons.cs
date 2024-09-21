using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameButtons : MonoBehaviour
{
    public enum TYPE
    {
        back
    }
    public TYPE type;

    public void OnClick()
    {
        Actions.Back.Invoke(type);
        print("pointer up");
    }
}
