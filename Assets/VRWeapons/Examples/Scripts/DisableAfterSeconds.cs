using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterSeconds : MonoBehaviour {
    [SerializeField]
    float secondsUntilDisable;

    float elapsedTime;

    private void OnEnable()
    {
        elapsedTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - elapsedTime >= secondsUntilDisable)
        {
            gameObject.SetActive(false);
        }
    }
}
