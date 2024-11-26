using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBar : MonoBehaviour
{
    FishReeler reel;
    private void Awake()
    {
        reel = transform.parent.GetComponent<FishReeler>();
    }
    private void OnTriggerEnter(Collider other)
    {
        reel.inRange = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!reel.correct)
        {
            reel.Missed();
        }
        reel.inRange = false;
        reel.correct = false;
    }
}
