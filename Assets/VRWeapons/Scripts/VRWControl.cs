using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Valve.VR.InteractionSystem;


namespace VRWeapons
{
    public class VRWControl : MonoBehaviour
    {
        float VRWVersion = 2.02f;

        [HideInInspector]
        public List<Weapon> WeaponsInScene;

        [Tooltip("Check this to disable controller on weapon pickup.\n\nNOTE: If using VRTK, this box will do nothing. Instead, hide controllers by adding the VRTK_InteractControllerAppearance script to your weapon and checking the \"Hide Controller On Grab\" box."), SerializeField]
        public bool disableControllersOnPickup;

        [Header("Gunshot layer mask")]
        public LayerMask shotMask;

        public delegate void TriggerHaptics();

        private void Start()
        {
            Debug.Log("VRWeapons Version " + VRWVersion.ToString("F2"));
        }

        int CountWeapons()
        {
            int i = 0;
            foreach(Weapon tmp in FindObjectsOfType<Weapon>())
            {
                WeaponsInScene.Add(tmp);                
                i++;
            }
            return i;
        }
        
        public static float V3InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Mathf.Clamp(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB), 0, 1);
        }

        public static Vector3 V3Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            Vector3 tmp = value;
            tmp = new Vector3(Mathf.Clamp(tmp.x, min.x, max.x), Mathf.Clamp(tmp.y, min.y, max.y), Mathf.Clamp(tmp.z, min.z, max.z));
            return tmp;
        }

        public static Quaternion ZLockedAim(Transform mainHand, Transform secondHand, Transform thisObj)                     // BORROWED FROM VRTK'S VRTK_ControlDirectionGrabAction SCRIPT, THANKS STONE FOX//
        {
            Vector3 forward = (secondHand.position - mainHand.position).normalized;

            // calculate rightLocked rotation
            Quaternion rightLocked = Quaternion.LookRotation(forward, Vector3.Cross(-mainHand.right, forward).normalized);

            // delta from current rotation to the rightLocked rotation
            Quaternion rightLockedDelta = Quaternion.Inverse(thisObj.rotation) * rightLocked;

            float rightLockedAngle;
            Vector3 rightLockedAxis;

            // forward direction and roll
            rightLockedDelta.ToAngleAxis(out rightLockedAngle, out rightLockedAxis);

            if (rightLockedAngle > 180f)
            {
                // remap ranges from 0-360 to -180 to 180
                rightLockedAngle -= 360f;
            }

            // make any negative values into positive values;
            rightLockedAngle = Mathf.Abs(rightLockedAngle);

            // calculate upLocked rotation
            Quaternion upLocked = Quaternion.LookRotation(forward, mainHand.forward);

            // delta from current rotation to the upLocked rotation
            Quaternion upLockedDelta = Quaternion.Inverse(thisObj.rotation) * upLocked;

            float upLockedAngle;
            Vector3 upLockedAxis;

            // forward direction and roll
            upLockedDelta.ToAngleAxis(out upLockedAngle, out upLockedAxis);

            // remap ranges from 0-360 to -180 to 180
            if (upLockedAngle > 180f)
            {
                upLockedAngle -= 360f;
            }

            // make any negative values into positive values;
            upLockedAngle = Mathf.Abs(upLockedAngle);

            // assign the one that involves less change to roll
            return (upLockedAngle < rightLockedAngle ? upLocked : rightLocked);
        }
    }
}