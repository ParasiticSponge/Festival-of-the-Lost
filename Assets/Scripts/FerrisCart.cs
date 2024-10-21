using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FerrisCart : MonoBehaviour
{
    public GameObject target;
    Vector3 targetOrigin;

    float speed = 8;
    float rockSpeed = 10;
    float direction = -1;
    public float length = 170;
    public float startingAngle;
    public float startingTime;
    public Vector2 offset = new Vector3(0, 0);
    Transform rect;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<RectTransform>())
            rect = GetComponent<RectTransform>().transform;
        else
            rect = transform;
        //length = target.GetComponent<RectTransform>().rect.height / 2;
        if (target.GetComponent<RectTransform>())
            targetOrigin = target.GetComponent<RectTransform>().transform.position;
        else
            targetOrigin = target.transform.position;
        startingAngle *= Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Functions.Oscillate((Time.timeSinceLevelLoad * rockSpeed) + startingTime, -25, 25);
        rect.localRotation = Quaternion.Euler(0, 0, angle);
        //rect.localRotation = Functions.Quat(angle * direction, 2);

        angle = Mathf.Deg2Rad * Time.timeSinceLevelLoad * speed * direction;
        float x = (Mathf.Cos(startingAngle + angle) * length) + offset.x;
        float y = (Mathf.Sin(startingAngle + angle) * length) + offset.y;
        Vector3 vector = new Vector3(x, y, rect.position.z);
        Vector3 origin = new Vector3(targetOrigin.x, targetOrigin.y, 0);
        transform.position = origin + vector;

    }
}
