using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using UnityEngine.TextCore.Text;

public class RodController : MonoBehaviour
{
    GameManager gameManager;
    public bool fire;
    public bool hasFish;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (!fire && !gameManager.paused)
        {
            Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 vector = screenToWorld - transform.position;

            float angle = (Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg);
            Quaternion rot = Quaternion.Euler(0, 0, angle - 90);
            transform.localRotation = rot;

            Vector3 mouse = screenToWorld - transform.position;
            Vector3 rodTip = transform.position + (new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0) * 11);

            BezierKnot knot = gameManager.bobberSpline.Spline.ToArray()[gameManager.bobberSpline.Spline.Count - 1];
            knot.Position = gameManager.bobberSpline.transform.InverseTransformPoint(rodTip);
            knot.Rotation = Quaternion.Inverse(gameManager.bobberSpline.transform.rotation) * rot;
            gameManager.bobberSpline.Spline.SetKnot(gameManager.bobberSpline.Spline.Count - 1, knot);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!fire && !gameManager.paused)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
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
                    Vector3 vector = touchPosition - transform.position;

                    transform.localRotation = Quaternion.Euler(0, 0, (Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg) - 90);
                }
            }
        }
    }
}
