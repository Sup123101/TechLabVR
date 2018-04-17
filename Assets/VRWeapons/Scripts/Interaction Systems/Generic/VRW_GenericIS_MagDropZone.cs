﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons.InteractionSystems.Generic
{
    [RequireComponent(typeof(Rigidbody))]
    public class VRW_GenericIS_MagDropZone : MonoBehaviour
    {
        Weapon thisWeap;
        Collider thisCol;
        IMagazine collidingMagazine;

        IEnumerator magIn;

        bool isLerping, magMatchFound;

        float dropTime, lerpVal, lerpValDelta;

        [SerializeField]
        public Transform magPosition;

        [SerializeField]
        public Transform startingMag;

        [Tooltip("How long in seconds it takes for magazine to reach mag well, once inserted."), SerializeField]
        float timeToMagInsert;

        [Tooltip("List of valid tags accepted by this weapon.\n\nIf this is empty, weapon will accept any magazine. Use with caution."), SerializeField]
        string[] validTags;

        private void Start()
        {
            if (timeToMagInsert != 0)
            {
                lerpValDelta = 1 / timeToMagInsert;
            }
            else
            {
                lerpValDelta = 1;
            }
            thisWeap = GetComponentInParent<Weapon>();

            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

            thisWeap.OnMagRemoved += new Weapon.MagazineRemovedEvent(MagDropped);

            thisCol = GetComponent<Collider>();
            if (magPosition != null)
            {
                magPosition.gameObject.SetActive(false);
            }
            if (thisCol != null)
            {
                Physics.IgnoreCollision(thisCol, thisWeap.weaponBodyCollider);
                if (thisWeap.secondHandGripCollider != null)
                {
                    Physics.IgnoreCollision(thisCol, thisWeap.weaponBodyCollider);
                }
            }
            else
            {
                Debug.LogError("No collider found on mag drop zone (Child of " + thisWeap + "). Please add a collider.");
            }
            thisCol.isTrigger = true;

            if (startingMag != null)
            {
                if (startingMag.GetComponent<IMagazine>() != null)
                {
                    InsertMag(startingMag.GetComponent<IMagazine>(), startingMag, true);
                }
                else
                {
                    Debug.LogError("No script implementing interface IMagazine found on " + startingMag + ". Assign a correctly configured magazine.");
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if ((collidingMagazine = other.transform.GetComponent<IMagazine>()) == null)
            {
                Physics.IgnoreCollision(thisCol, other.GetComponent<Collider>());           // If it finds a non-magazine collider, ignore physics between the drop zone and that collider.
            }                                                                               // This prevents excessive use of GetComponent.
            else
            {
                if (!magMatchFound && !thisWeap.IsLoaded() && Time.time - dropTime > 0.3f)
                {
                    if (IsValidMag(other.transform.tag))
                    {
                        magMatchFound = true;
                        other.transform.parent = thisWeap.transform;                        
                        other.GetComponent<Rigidbody>().isKinematic = true;
                        if (magPosition != null && !isLerping)
                        {
                            if (timeToMagInsert > 0)
                            {
                                magIn = LerpMovement(collidingMagazine, other.transform);
                                StartCoroutine(magIn);
                                isLerping = true;
                            }
                            else
                            {
                                InsertMag(collidingMagazine, other.transform);
                            }
                        }
                    }
                }
            }
        }
        
        IEnumerator LerpMovement(IMagazine mag, Transform t)
        {
            float startTime = Time.time;

            Collider col = t.GetComponent<Collider>();
            Physics.IgnoreCollision(col, thisWeap.weaponBodyCollider);

            if (thisWeap.secondHandGripCollider != null)
            {
                Physics.IgnoreCollision(col, thisWeap.secondHandGripCollider);
            }
            
            Quaternion startRotation = t.rotation;
            Vector3 startPos = t.position;

            while (lerpVal < 1)
            {
                t.position = Vector3.Lerp(startPos, magPosition.position, lerpVal);
                t.rotation = Quaternion.Lerp(startRotation, magPosition.rotation, lerpVal);
                lerpVal += lerpValDelta * Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            lerpVal = 0;
            InsertMag(mag, t);
            isLerping = false;
        }

        void InsertMag(IMagazine newMag, Transform t, bool onStart = false)
        {
            magMatchFound = false;
            VRW_GenericIS_BulletDropZone dz = null;
            if ((dz = t.GetComponentInChildren<VRW_GenericIS_BulletDropZone>()) != null)
            {
                dz.thisCol.enabled = false;
            }

            if (!onStart)
            {
                t.GetComponent<VRW_GenericIS_Interactable>().Ungrab();
            }

            magIn = null;
            newMag.MagIn(thisWeap);

            Collider col = t.GetComponent<Collider>();
            Physics.IgnoreCollision(col, thisWeap.weaponBodyCollider);

            if (thisWeap.secondHandGripCollider != null)
            {
                Physics.IgnoreCollision(col, thisWeap.secondHandGripCollider);
            }

            thisWeap.InsertMagazine(newMag);

            col.enabled = false;
            
            t.localPosition = magPosition.localPosition;
            t.localEulerAngles = magPosition.localEulerAngles;
        }

        bool IsValidMag(string tag)
        {
            bool ret = false;

            if (validTags.Length == 0)
            {
                ret = true;                             // In case no list is set up, allow all magazines.
            }

            for (int i = 0; i < validTags.Length; i++)
            {
                if (tag == validTags[i])
                {
                    ret = true;
                }
            }
            return ret;
        }

        void MagDropped(Weapon thisWeap, IMagazine currentMag)
        {
            if (currentMag != null)
            {
                MonoBehaviour go = currentMag as MonoBehaviour;
                go.GetComponent<Collider>().enabled = true;
                dropTime = Time.time;
            }
        }
    }
}