using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseController2D : MonoBehaviour
{
    Vector3 poop;
    public bool fire;
    Animator animator;
    Vector3 velocity;
    public float smoothTime = 0.124f;
    Rigidbody2D rb;

    bool collisionListener;
    bool hasExited;
    Collider2D otherCollider;

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
    }

    private void Update()
    {
        if (!fire)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenToWorld.y = transform.position.y;
            screenToWorld.z = transform.position.z;

            transform.position = screenToWorld;

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

        //>:( silly 2D collision detection only happening once and 3D doesn't help
        //collisions also still triggers when disabled!!!
        if (collisionListener)
        {
            if (otherCollider.transform.position.z == transform.localPosition.z && !hasExited)
            {
                //do stuff
                Actions.HitBalloon.Invoke();
                otherCollider.GetComponent<Animator>().Play("BalloonPop", 0, 0);
                otherCollider.GetComponent<CircleCollider2D>().enabled = false;
                collisionListener = false;
            }
            if (hasExited)
            {
                collisionListener = false;
                hasExited = false;
            }
        }
    }

    public void PowerMetre(float amount)
    {
        animator.Play("dartThrow");
        var cam = Camera.main;
        fire = true;

        Vector3 screenToWorld = cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight * amount, 0));
        Vector3 desiredPos = new Vector3(transform.position.x, screenToWorld.y, transform.position.z);

        //Vector3 lerp = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        //Vector3 lerp = Vector3.Lerp(transform.position, newPos, 3);

        StartCoroutine(Move(transform.position, desiredPos));
        //transform.position = newPos;
    }
    IEnumerator Move(Vector3 a, Vector3 b)
    {
        Vector3 desired = b - a;
        //float FPS = 1.0f / Time.deltaTime;
        for (float i = 0; i <= 1; i+= Time.deltaTime)
        {
            transform.position = a + (desired * EasingFunctions.EaseOutCubic(i));
            yield return null;
        }
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

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
    /*private void OnCollisionEnter2D(Collision2D other)
    {
        if (enabled)
        {
            if (other.transform.position.z == transform.localPosition.z)
            {
                print("COLLISION");
            }
        }
    }*/
    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled)
        {
            otherCollider = other;
            collisionListener = true;
        }
    }*/
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enabled)
        {
            print("has entered");
            collisionListener = true;
            otherCollider = other;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled)
        {
            print("exited");
            hasExited = true;
        }
    }
}
