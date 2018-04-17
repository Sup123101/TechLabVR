using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons
{
    public class JointKick : MonoBehaviour, IKickActions
    {
        Weapon thisWeap;

        MonoBehaviour muzzleGO;

        Rigidbody thisRB;

        ConfigurableJoint thisJoint;

        bool isKicking;

        [SerializeField]
        float kickStrength;

        float startValue, currentValue, kickTime;

        private void Start()
        {
            thisWeap = GetComponent<Weapon>();
            muzzleGO = thisWeap.gameObject.GetComponentInChildren<IMuzzleActions>() as MonoBehaviour;
            thisRB = GetComponent<Rigidbody>();
        }

        public void Kick()
        {
            if (thisJoint == null)
            {
                thisJoint = GetComponent<ConfigurableJoint>();
            }

            if (!isKicking)
            {
                SetMotionFree();
                isKicking = true;
                startValue = transform.localPosition.z;
                kickTime = Time.time;
            }

            thisRB.AddForce(-muzzleGO.transform.forward * kickStrength, ForceMode.Impulse);
        }

        private void FixedUpdate()
        {
            if (isKicking)
            {
                currentValue = transform.localPosition.z;
                if (Time.time - kickTime > 0.1f)
                {
                    if (Mathf.Abs(currentValue - startValue) < 0.005f)
                    {
                        SetMotionLocked();
                        isKicking = false;
                    }
                }

            }
        }

        void SetMotionFree()
        {
            thisJoint.zMotion = ConfigurableJointMotion.Limited;
        }

        void SetMotionLocked()
        {
            thisJoint.zMotion = ConfigurableJointMotion.Locked;
        }
    }
}