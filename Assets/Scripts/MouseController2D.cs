using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController2D : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public bool fire;
    public bool canClick = true;
    Animator animator;
    Vector3 velocity;
    public float smoothTime = 0.124f;
    Rigidbody2D rb;

    private void OnEnable()
    {
        Actions.Power += PowerMetre;
        rb.velocity = Vector2.zero;
    }
    private void OnDisable()
    {
        Actions.Power -= PowerMetre;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
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
        if (!fire)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenToWorld.y = transform.position.y;
            screenToWorld.z = transform.position.z;
            transform.position = screenToWorld;
        }

        if (Input.GetMouseButtonDown(0))
        {
            print("holding");
            if (canClick) Actions.Hold.Invoke();
        }
        if(Input.GetMouseButtonUp(0))
        {
            print("released");
            if (canClick) Actions.Release.Invoke();
        }
    }

    public void PowerMetre(float amount)
    {
        animator.Play("dartThrow");
        animator.speed = 1 - amount;
        var cam = Camera.main;
        fire = true;

        print("AMOUNT: " + amount);

        Vector3 worldToScreen = Camera.main.WorldToScreenPoint(transform.position);
        //Vector3 p = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
        Vector3 desiredPos = new Vector3(worldToScreen.x, amount * cam.pixelHeight, transform.position.z);
        Vector3 screenToWorld = cam.ScreenToWorldPoint(desiredPos);
        Vector3 newPos = new Vector3(screenToWorld.x, screenToWorld.y, 0);

        //Vector3 lerp = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        //Vector3 lerp = Vector3.Lerp(transform.position, newPos, 3);

        StartCoroutine(Move(transform.position, newPos));
        //transform.position = newPos;
    }
    IEnumerator Move(Vector3 a, Vector3 b)
    {
        Vector3 desired = b - a;
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i+= 0.01f)
        {
            transform.position = a + (desired * EasingFunctions.EaseOutCubic(i));
            yield return null;
        }
        animator.Play("dartIdle");
        Actions.Shot.Invoke();
    }

    /*private IEnumerator LoadSomeStuff()
    {
        WWW www = new WWW("http://someurl");
        yield return www;
        if (String.IsNullOrEmpty(www.error) {
            yield return "success";
        }
        else
        {
            yield return "fail";
        }
    }
    public void SomeData()
    {
        CoroutineWithData cd = new CoroutineWithData(this, LoadSomeStuff());
        yield return cd.coroutine;
        Debug.Log("result is " + cd.result);  //  'success' or 'fail'
    }*/
    /*void Foo()
    {
        StartCoroutine(Bar((myReturnValue) => {
            if (myReturnValue) { ... }
        });

    }

    IEnumerator Bar(System.Action<bool> callback)
    {
        yield return null;
        callback(true);
    }*/
}
