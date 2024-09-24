using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    float speed = 0.05f;
    float direction = -1;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Functions.Loop(Time.timeSinceLevelLoad * speed, 360, 0);

        rect.localRotation = Quaternion.Euler(0, 0, angle * direction);
        //rect.localRotation = Functions.Quat(angle * direction, 2);
    }
}
