using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FerrisCart : MonoBehaviour
{
    float speed = 8;
    float rockSpeed = 10;
    float direction = -1;
    RectTransform rect;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Functions.Loop(Time.timeSinceLevelLoad * rockSpeed, 0, 360);
        float rockAngle = Functions.Oscillate(Time.timeSinceLevelLoad * rockSpeed, -25, 25);
        //print(angle);
        rect.localRotation = Quaternion.Euler(0, 0, rockAngle);
        rect.RotateAround(target.position, new Vector3(0, 0, -1), Time.deltaTime * speed);
        //rect.localRotation = Functions.Quat(angle * direction, 2);
    }
}
