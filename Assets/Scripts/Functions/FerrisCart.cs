using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class FerrisCart : MonoBehaviour
{
    public GameObject target;
    Vector3 targetOrigin;
    public Vector3 TargetOrigin { get { return targetOrigin; } set { targetOrigin = value; } }

    float speed = 8;
    float rockSpeed = 10;
    float direction = -1;
    public Vector2 rotation = new Vector2(-25, 25);
    public float length = 170;
    public float startingAngle;
    public float startingTime;
    public Vector2 offset = new Vector3(0, 0);
    public bool swing = true;
    public bool rotate = true;
    public bool blink = false;
    public float startingTimeBlink = 0;
    public bool invokeObject;

    Transform rect;
    Color spriteColour;
    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<RectTransform>())
            rect = GetComponent<RectTransform>().transform;
        else
            rect = transform;
        //length = target.GetComponent<RectTransform>().rect.height / 2;
        if (target != null)
        {
            if (target.GetComponent<RectTransform>())
                targetOrigin = target.GetComponent<RectTransform>().transform.position;
            else
                targetOrigin = target.transform.position;
        }
        if (GetComponent<Image>())
            spriteColour = GetComponent<Image>().color;
        else if (GetComponent<SpriteRenderer>())
            spriteColour = GetComponent<SpriteRenderer>().color;
        else if (GetComponent<Text>())
            spriteColour = GetComponent<Text>().color;
        else 
            spriteColour = GetComponent<Light2D>().color;
        startingAngle *= Mathf.Deg2Rad;
    }

    // Update is called once per frame
    void Update()
    {
        float angle;
        if (swing)
        {
            angle = Functions.Oscillate((Time.timeSinceLevelLoad * rockSpeed) + startingTime, rotation.x, rotation.y);
            rect.localRotation = Quaternion.Euler(0, 0, angle);
        }
        //rect.localRotation = Functions.Quat(angle * direction, 2);

        if (rotate)
        {
            angle = Mathf.Deg2Rad * Time.timeSinceLevelLoad * speed * direction;
            float x = (Mathf.Cos(startingAngle + angle) * length) + offset.x;
            float y = (Mathf.Sin(startingAngle + angle) * length) + offset.y;
            Vector3 vector = new Vector3(x, y, rect.position.z);
            Vector3 origin = new Vector3(targetOrigin.x, targetOrigin.y, 0);
            transform.position = origin + vector;
        }
        if (blink)
        {
            Color color = spriteColour;
            float loop = Functions.Oscillate((Time.timeSinceLevelLoad * rockSpeed) + startingTimeBlink, 0, 10);
            color.a = loop;
            GetComponent<Light2D>().intensity = loop;
        }
        if (invokeObject)
        {
            if (transform.localPosition.x >= 0 && transform.localPosition.x <= 1 && transform.localPosition.y < 0)
                Actions.RideCart.Invoke(gameObject);
        }
    }
}
