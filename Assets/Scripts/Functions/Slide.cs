using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Slide : MonoBehaviour
{
    public float speed = 1.0f;
    bool shouldMove = true;
    bool moveVertical = false;
    float sizeHorizontal = 0;
    float sizeVertical = 0;
    float initialX;
    float initialY;
    float PosX;
    float PosY;

    float time = 0;
    float distance = 27;

    void Awake()
    {
        initialX = transform.position.x;
        initialY = transform.position.y;
        if (sizeHorizontal == 0)
        {
            if (GetComponent<SpriteRenderer>())
                sizeHorizontal = GetComponent<SpriteRenderer>().sprite.texture.width * transform.localScale.x;
            else
            {
                if (transform.childCount > 0)
                {
                    float totalSizes = 0;
                    float totalPosX = 0;
                    foreach (Transform child in transform)
                    {
                        if (child.GetComponent<SpriteRenderer>())
                        {
                            Sprite sprite = child.GetComponent<SpriteRenderer>().sprite;
                            int width = 0;
                            int height = 0;
                            float ppu = 0;
                            //Functions.GetImageInfo(sprite.texture, out width, out height, out ppu);
                            totalSizes += (width / ppu) * child.transform.localScale.x;
                            totalPosX += child.localPosition.x;
                        }
                    }
                    sizeHorizontal = totalSizes;
                    if (transform.childCount > 1) sizeHorizontal = totalSizes - totalPosX;
                    PosX = initialX - sizeHorizontal;
                }
            }
        }
        if (sizeVertical == 0) 
        {
            if (GetComponent<SpriteRenderer>())
                sizeHorizontal = GetComponent<SpriteRenderer>().sprite.texture.height * transform.localScale.y;
            else
            {
                if (transform.childCount > 0)
                {
                    float totalSizes = 0;
                    float totalPosY = 0;
                    foreach (Transform child in transform)
                    {
                        if (child.GetComponent<SpriteRenderer>())
                        {
                            Sprite sprite = child.GetComponent<SpriteRenderer>().sprite;
                            int width = 0;
                            int height = 0;
                            float ppu = 0;
                            //Functions.GetImageInfo(sprite.texture, out width, out height, out ppu);
                            totalSizes += (height / ppu) * child.transform.localScale.y;
                            totalPosY += child.localPosition.y;
                        }
                    }
                    sizeVertical = totalSizes;
                    if (transform.childCount > 1) sizeVertical = totalSizes - totalPosY;
                    PosY = initialY - sizeVertical;
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        print(initialX);
        print(PosX);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time+= 0.01f;
        if (shouldMove)
        {
            if (!moveVertical)
                transform.position = new Vector3(Functions.Loop2(time * speed, initialX, initialX - distance), transform.position.y, transform.position.z);
            else
                transform.position = new Vector3(transform.position.x, Functions.Loop2(time * speed, initialY, initialY - distance), transform.position.z);
        }
    }
}
