using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishReeler : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(Test());
    }

    IEnumerator Test()
    {
        yield return new WaitForSeconds(5);
        Actions.missedReel.Invoke();
        StartCoroutine(Test());
    }
}
