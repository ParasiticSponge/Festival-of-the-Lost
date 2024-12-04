using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float shakeDuration = 1f;
    public bool isShaking = false;
    [SerializeField] AnimationCurve shakeCurve;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isShaking)
        {
            isShaking = false;
            StartCoroutine(ShakeCamera());
        }
    }

    IEnumerator ShakeCamera()
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float shakeStrength = shakeCurve.Evaluate(elapsedTime / shakeDuration);
            transform.position = startPos + (Random.insideUnitSphere * shakeStrength);
            yield return null;
        }
        transform.position = startPos;
    }
}