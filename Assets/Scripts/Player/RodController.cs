using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RodController : MonoBehaviour
{
    GameManager gameManager;
    public bool fire;
    public bool hasFish;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!fire && !gameManager.paused)
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                Vector3 screenToWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 vector = screenToWorld - transform.position;

                transform.localRotation = Quaternion.Euler(0, 0, (Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg) - 90);
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
