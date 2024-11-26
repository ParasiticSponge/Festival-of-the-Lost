using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishReeler : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    float[,,] boxInfo = { {{108.93f, 0.5f, 0},{43.4f, 63.4f, 2}},
                          {{108.93f, -1.5f, 0},{43.4f, 32.57f, 2}},
                          {{108.93f, -1.5f, 0},{43.4f, 22.2f, 2}} };
    public int sprite = 0;
    int direction = 1;
    float magnitude = 110;
    GameObject mask;
    GameObject bar;
    public float speed = 1;
    float barAngle;
    float angle;
    public bool inRange;
    public bool correct;
    Vector2 position;
    private void Awake()
    {
        mask = transform.GetChild(1).gameObject;
        bar = transform.GetChild(3).gameObject;
    }
    private void OnEnable()
    {
        RandomPosition();
        mask.GetComponent<Image>().sprite = sprites[sprite];
    }
    private void OnDisable()
    {
        
    }

    private void Update()
    {
        position = bar.GetComponent<RectTransform>().anchoredPosition;
        angle += Time.deltaTime * direction;
        bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Cos((barAngle + angle) * speed), Mathf.Sin((barAngle + angle) * speed)) * magnitude;
        float positionalAngle = Mathf.Atan2(position.y, position.x) - 67.5f;
        bar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, positionalAngle * Mathf.Rad2Deg);

        if (Input.GetMouseButtonDown(0) && inRange)
        {
            correct = true;
            direction *= -1;
            RandomMask(Mathf.Atan2(position.y, position.x));
        }
        else if (Input.GetMouseButtonDown(0) && !inRange)
        {
            Missed();
        }
    }

    void RandomPosition()
    {
        angle = 0;

        barAngle = (float)Functions.random.Next(361);
        bar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Cos(barAngle * Mathf.Deg2Rad), Mathf.Sin(barAngle * Mathf.Deg2Rad)) * magnitude;
        bar.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, barAngle - 90);

        float randomBarAngle = (float)Functions.random.Next((int)barAngle + 90, (int)barAngle + 90 + 180);
        mask.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, randomBarAngle);
    }
    void RandomMask(float angle)
    {
        float randomBarAngle = (float)Functions.random.Next((int)angle + 90, (int)angle + 90 + 180);
        mask.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, randomBarAngle);
    }

    public void Missed()
    {
        Actions.missedReel.Invoke();
    }
}
