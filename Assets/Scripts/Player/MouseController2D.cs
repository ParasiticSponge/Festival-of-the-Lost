using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class MouseController2D : MonoBehaviour
{
    public GameObject crosshair;
    Vector3 poop;
    public bool fire;
    Animator animator;
    Vector3 velocity;
    public float smoothTime = 0.124f;
    Rigidbody2D rb;
    float speed = 1.5f;

    bool collisionListener;
    bool hasExited = true;
    Collider2D otherCollider;
    GameManager gameManager;

    private void OnEnable()
    {
        Actions.Power += PowerMetre;
        rb.velocity = Vector2.zero;

        poop = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.4f, transform.localPosition.z);
    }
    private void OnDisable()
    {
        Actions.Power -= PowerMetre;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (!fire && !gameManager.paused)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                screenToWorld.y = transform.position.y;
                screenToWorld.z = transform.position.z;

                transform.position = screenToWorld;

                if (crosshair.activeSelf)
                    crosshair.transform.position = new Vector3(screenToWorld.x, crosshair.transform.position.y, crosshair.transform.position.z);

                if (Input.GetMouseButtonDown(0))
                {
                    //StartCoroutine(Move(transform.position, poop));
                    Actions.Hold.Invoke();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    Actions.Release.Invoke();
                }
            }
            else
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    Vector3 touchPosition = UnityEngine.Input.GetTouch(0).position;
                    touchPosition.y = transform.position.y;
                    touchPosition.z = transform.position.z;

                    transform.position = touchPosition;
                }
            }
        }
    }

    //public void PowerMetre(float amount)
    public void PowerMetre(Vector3 amount)
    {
        /*animator.Play("dartThrow");
        var cam = Camera.main;
        fire = true;

        Vector3 screenToWorld = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight * amount, 0));
        Vector3 desiredPos = new Vector3(transform.position.x, screenToWorld.y, transform.position.z);

        //Vector3 lerp = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        //Vector3 lerp = Vector3.Lerp(transform.position, newPos, 3);

        StartCoroutine(Move(transform.position, desiredPos));
        //transform.position = newPos;*/

        animator.Play("dartThrow");
        var cam = Camera.main;
        fire = true;

        Vector3 desiredPos = new Vector3(amount.x, amount.y, transform.position.z);
        StartCoroutine(Move(transform.position, desiredPos));
    }
    IEnumerator Move(Vector3 a, Vector3 b)
    {
        Vector3 desired = b - a;
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i+= Time.deltaTime * speed)
        {
            transform.position = a + (desired * EasingFunctions.EaseOutCubic(i));
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -1);

        //if (otherCollider.transform.position.z == transform.localPosition.z && !hasExited)
        if (!hasExited)
        {
            //do stuff
            otherCollider.GetComponent<CircleCollider2D>().enabled = false;
            Actions.BalloonType.Invoke((int)otherCollider.GetComponent<BalloonType>().type);
            Actions.HitBalloon.Invoke(true);
            otherCollider.GetComponent<Animator>().Play("BalloonPop", 0, 0);
        }
        else
            Actions.HitBalloon.Invoke(false);
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled)
        {
            hasExited = false;
            otherCollider = other;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled)
        {
            hasExited = true;
        }
    }
}
