using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController2D : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        print("clicked");
    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    private void FixedUpdate()
    {
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenToWorld.z = -10;
        transform.position = screenToWorld;
    }
}
