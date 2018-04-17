using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRWeapons {
    public class FireSelectRotator : MonoBehaviour {

        [HideInInspector, SerializeField]
        public Vector3 safePosition, semiPosition, burstPosition, autoPosition;
        Weapon thisWeap;

        [SerializeField, Range(0, 1)]
        float rotationSpeed;

        private void Awake()
        {
            thisWeap = GetComponentInParent<Weapon>();
            
            thisWeap.OnFireModeChanged += SetPosition;
        }

        void Start() {

            if (rotationSpeed == 0)
            {
                rotationSpeed = 0.1f;
            }
        }

        void SetPosition(int currentFireMode)
        {
            bool validFireMode = false;

            Vector3 finalPosition = new Vector3();

            switch (currentFireMode)
            {
                case 0:
                    break;
                case 1:
                    validFireMode = true;
                    finalPosition = safePosition;
                    break;
                case 2:
                    validFireMode = true;
                    finalPosition = semiPosition;
                    break;
                case 4:
                    validFireMode = true;
                    finalPosition = autoPosition;
                    break;
                case 8:
                    validFireMode = true;
                    finalPosition = burstPosition;
                    break;
            }            

            if (validFireMode)
            {
                IEnumerator tmp = MoveToPosition(finalPosition);
                StartCoroutine(tmp);
            }

        }        

        IEnumerator MoveToPosition(Vector3 finalPosition)
        {
            Vector3 startingPos = transform.localEulerAngles;

            float lerpVal = 0;

            while (lerpVal < 1)
            {
                transform.localRotation = Quaternion.Lerp(Quaternion.Euler(startingPos), Quaternion.Euler(finalPosition), lerpVal);
                lerpVal += rotationSpeed;
                yield return new WaitForFixedUpdate();
            }
            transform.localEulerAngles = finalPosition;
        }
    }
}