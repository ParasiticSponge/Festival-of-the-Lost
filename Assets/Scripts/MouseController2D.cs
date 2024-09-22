using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController2D : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool canClick = true;
    Animator animator;

    private void OnEnable()
    {
        Actions.Power += PowerMetre;
    }
    private void OnDisable()
    {
        Actions.Power -= PowerMetre;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        print("released");
        if (canClick) Actions.Release.Invoke();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        print("holding");
        if (canClick) Actions.Hold.Invoke();
    }

    private void Update()
    {
        Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        screenToWorld.z = 0;
        screenToWorld.y = -17f;
        transform.position = screenToWorld;

        if (Input.GetMouseButtonDown(0))
        {
            print("holding");
            if (canClick) Actions.Hold.Invoke();
        }
        if(Input.GetMouseButtonUp(0))
        {
            print("released");
            animator.Play("dartThrow");
            if (canClick) Actions.Release.Invoke();
        }
    }

    public void PowerMetre(float amount)
    {
        //transform position equals screen to world in relation to width and height
    }
}
