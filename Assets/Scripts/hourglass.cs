using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hourglass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quat(50, 0);
        //transform.Rotate(0, 10 * Time.deltaTime, 0);
        //transform.Rotate(transform.forward, 50 * Time.deltaTime);
        transform.GetComponent<Rigidbody>().AddTorque(new Vector3(0, 5 * Time.deltaTime, 0));
    }

    Quaternion Quat(float angle, Int32 axis)
    {
        float w = Mathf.Cos(angle / 2 * Mathf.Deg2Rad);
        float component = Mathf.Sin(angle / 2 * Mathf.Deg2Rad);

        axis = (int)(100 / Mathf.Pow(10, axis));
        float x = component * (axis / 100);
        float y = component * (axis % 100 / 10);
        float z = component * (axis % 10);

        return new Quaternion(x, y, z, w);
    }

    float DegToRad(float angle)
    {
        return Mathf.PI / 180 * angle;
    }
}
