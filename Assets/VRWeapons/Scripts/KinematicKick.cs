using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons
{
    public class KinematicKick : MonoBehaviour, IKickActions
    {
        Weapon thisWeap;
        Vector3 originalPos, targetPos, currentPos;
        float lerpVal;
        bool fixPosition, isKicking, originalPosSet, originalRotSet, wasGrippedWhenFired;
        int shotsFiredSinceReset;

        Quaternion startRot, endRot, originalRot;

        MonoBehaviour muzzleGO;

        [Tooltip("How much to kick back when the weapon is fired. Logarithmically tapers down with each shot."), SerializeField, Range(0, 1)]
        float positionalKickStrength = 0.05f;
        
        [Tooltip("Amount (and direction) the weapon rotates when fired. Logarithmically tapers down with each shot."), SerializeField]
        Vector3 amountToRotate = new Vector3(-5, 0, 0);

        [Tooltip("How quickly the weapon recoils. 1 is instant, 0 is no movement."), SerializeField, Range(0f, 1f)]
        float recoilLerpSpeed = 0.5f;

        [Tooltip("How quickly the weapon recovers. 1 is instant, 0 is no movement."), SerializeField, Range(0f, 1f)]
        float recoverLerpSpeed = 0.2f;

        [Tooltip("Decreases the amount of kick when 2-hand gripped by multiplication. 0 = no kick, 1 = full kick."), SerializeField, Range(0f, 1f)]
        float twoHandGripKickReduction = 0.5f;
        
        private void Start()
        {
            thisWeap = GetComponent<Weapon>();
            thisWeap.OnWeaponStopFiring += new Weapon.WeaponStopFiringEvent(ResetShots);

            muzzleGO = thisWeap.gameObject.GetComponentInChildren<IMuzzleActions>() as MonoBehaviour;            
        }

        void SetOriginalPos(Weapon thisWeap)
        {
            originalPos = transform.localPosition;
            originalRot = transform.localRotation;
        }

        public void Kick()
        {
            float tmpKickReduction = 1;
            if (!originalPosSet)
            {
                originalPos = transform.localPosition;
                originalPosSet = true;
            }

            shotsFiredSinceReset++;

            if (!originalRotSet && !thisWeap.secondHandGripped)
            {
                startRot = transform.localRotation;
                endRot = Quaternion.Euler((transform.localEulerAngles.x + (amountToRotate.x / shotsFiredSinceReset)), (transform.localEulerAngles.y + (amountToRotate.y / shotsFiredSinceReset)), (transform.localEulerAngles.z + (amountToRotate.z / shotsFiredSinceReset)));
                originalRotSet = true;
            }

            if (thisWeap.secondHandGripped)
            {
                tmpKickReduction *= twoHandGripKickReduction;
            }
            
            currentPos = transform.localPosition;
            
            targetPos = ((transform.localPosition - muzzleGO.transform.forward) * positionalKickStrength) / shotsFiredSinceReset;

            if (!thisWeap.secondHandGripped)
            {
                
            }
            isKicking = true;
        }

        void ResetShots(Weapon thisWeap)
        {
            shotsFiredSinceReset = 0;
        }

        private void Update()
        {
            if (isKicking && thisWeap.isHeld)
            {
                DoPositionalRecoil(thisWeap.secondHandGripped);
                if (!thisWeap.secondHandGripped)
                {
                    wasGrippedWhenFired = true;
                    DoRotationalRecoil();
                }
            }
            else if (fixPosition && thisWeap.isHeld)
            {
                DoPositionalRecovery();
                if (!thisWeap.secondHandGripped && wasGrippedWhenFired)
                {
                    DoRotationalRecovery();
                }
                if (lerpVal <= 0)
                {
                    fixPosition = false;
                }
            }
        }

        void DoPositionalRecoil(bool is2HandGripped)
        {
            float val = 1;
            if (is2HandGripped)
            {
                val *= twoHandGripKickReduction;
            }
            if (lerpVal >= 1)
            {
                lerpVal = 1;
                fixPosition = true;
                isKicking = false;
                wasGrippedWhenFired = false;
            }
            transform.Translate(transform.InverseTransformDirection(targetPos) * recoilLerpSpeed * val);
            lerpVal += recoilLerpSpeed;
        }

        void DoPositionalRecovery()
        { 
            if (lerpVal <= 0)
            {
                lerpVal = 0;
                targetPos = originalPos;
            }
            transform.localPosition = Vector3.Lerp(originalPos, transform.localPosition, lerpVal);
            lerpVal -= recoverLerpSpeed;
        }

        void DoRotationalRecoil()
        {
            transform.localRotation = Quaternion.Lerp(startRot, endRot, lerpVal);
        }

        void DoRotationalRecovery()
        {
            transform.localRotation = Quaternion.Lerp(startRot, endRot, lerpVal);
        }
    }
}