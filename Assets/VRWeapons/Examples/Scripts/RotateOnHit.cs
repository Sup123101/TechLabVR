using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnHit : MonoBehaviour {
    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 endRotation;

    Vector3 startRotation;

    [SerializeField]
    float rotationSpeed, timeTilReturn;

    bool isHit;

    float lerpVal;

    private void Start()
    {
        startRotation = target.localEulerAngles;
    }

    public void OnHit()
    {
        if (!isHit)
        {
            StartCoroutine(Rotate());
            isHit = true;
        }
    }

    IEnumerator Rotate()
    {
        while (lerpVal < 1)
        {
            target.localEulerAngles = Vector3.Lerp(startRotation, endRotation, lerpVal);
            
            lerpVal += rotationSpeed;
            yield return new WaitForFixedUpdate();
        }

        target.localEulerAngles = endRotation;
        lerpVal = 1;

        float curTime = Time.time;

        while (Time.time < curTime + timeTilReturn)
        {
            yield return null;
        }

        while (lerpVal > 0)
        {
            target.localEulerAngles = Vector3.Lerp(startRotation, endRotation, lerpVal);
            lerpVal -= rotationSpeed;
            yield return new WaitForFixedUpdate();
        }

        lerpVal = 0;

        target.localEulerAngles = startRotation;
        isHit = false;
    }
}
