using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    float speed = 8;
    float direction = -1;
    Transform rect;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<RectTransform>())
            rect = GetComponent<RectTransform>();
        else
            rect = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Functions.Loop(Time.timeSinceLevelLoad * speed, 0, 360);

        rect.localRotation = Quaternion.Euler(0, 0, angle * direction);
        //rect.localRotation = Functions.Quat(angle * direction, 2);
    }
}
