using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 7, -10);
    Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPos = target.position + offset;

        //smooth movement
        //Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        //Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, smoothSpeed);

        transform.position = desiredPos;
        //transform.LookAt(target);
    }
}
