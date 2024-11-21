using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BossDartController : MonoBehaviour
{
    GameManager gameManager;
    float length = 4f;
    GameObject bow;
    Vector3 targetOrigin;
    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        bow = transform.GetChild(2).gameObject;
        bow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            Actions.Hold.Invoke();
        if (Input.GetMouseButton(0))
        {
            bow.SetActive(true);
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
            screenPos = Input.mousePosition - screenPos;
            float angle = Mathf.Atan2(screenPos.y, screenPos.x);
            screenPos = new Vector3(Mathf.Cos(angle) * length, Mathf.Sin(angle) * length, 0);
            bow.transform.position = transform.position + screenPos;
            bow.transform.localPosition = new Vector3(bow.transform.localPosition.x, bow.transform.localPosition.y, -1);
            bow.transform.up = screenPos.normalized;
        }
        if (Input.GetMouseButtonUp(0))
        {
            bow.SetActive(false);
            Actions.Release.Invoke();
        }
    }
}
