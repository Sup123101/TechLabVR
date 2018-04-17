using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons.InteractionSystems.Generic
{
    [RequireComponent(typeof(Rigidbody))]
    public class VRW_GenericIS_BoltInteractable : MonoBehaviour
    {
        IBoltActions bolt;

        Collider col;

        SteamVR_Controller.Device device;
        SteamVR_TrackedObject trackedObj;
        VRW_GenericIS_Controller controller;

        VRW_GenericIS_InteractableWeapon weaponInteractable;

        Weapon thisWeap;

        Vector3 offset, lastGoodPosition;

        bool thisObjectIsGrabbed, hasMoved;

        float lerpValue, dropTime;

        [SerializeField]
        public Vector3 boltOpenPosition, boltClosedPosition;

        [SerializeField]
        bool mustHoldGrabButton;

        private void Awake()
        {
            thisWeap = GetComponentInParent<Weapon>();
            if ((col = GetComponent<Collider>()) != null)
            {
                thisWeap.colliderList.Add(col);
            }
        }

        private void Start()
        {
            thisWeap.IgnoreCollision(col);
            bolt = transform.parent.GetComponentInChildren<IBoltActions>();
            GetComponent<Rigidbody>().isKinematic = true;
            weaponInteractable = GetComponentInParent<VRW_GenericIS_InteractableWeapon>();
        }

        public Collider GetInteractableCollider()
        {
            return col;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!thisObjectIsGrabbed)
            {                
                if (controller == null)
                {
                    if (other.gameObject != thisWeap.holdingDevice && (controller = other.GetComponent<VRW_GenericIS_Controller>()) != null && !controller.IsCurrentlyHoldingObject)
                    {
                        controller.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(AttemptGrab);
                    }
                }
            }
        }

        void AttemptGrab()
        {
            if (!controller.IsCurrentlyHoldingObject)
            {
                offset = controller.TrackedObj.transform.position - transform.position;
                thisObjectIsGrabbed = true;
                controller.OnGrabButtonDown -= AttemptGrab;
                if (!mustHoldGrabButton)
                {
                    controller.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(Ungrab);
                }
                else
                {
                    controller.OnGrabButtonUp += new VRW_GenericIS_Controller.GrabButtonUnpressedEvent(Ungrab);
                }
            }
        }

        void Ungrab()
        {
            thisObjectIsGrabbed = false;
            controller.OnGrabButtonDown -= Ungrab;
            controller.OnGrabButtonUp -= Ungrab;
            controller = null;            
        }

        private void OnTriggerExit(Collider other)
        {
            if (!thisObjectIsGrabbed && controller != null)
            {
                controller.OnGrabButtonDown -= AttemptGrab;
                controller.OnGrabButtonDown -= Ungrab;
                controller.OnGrabButtonUp -= Ungrab;
                controller = null;
            }
        }

        private void Update()
        {
            float oldLerpValue = lerpValue;

            if (thisObjectIsGrabbed)
            {
                {
                    bolt.IsCurrentlyBeingManipulated(true);
                    hasMoved = true;

                    transform.position = controller.TrackedObj.transform.position - offset;

                    lerpValue = VRWControl.V3InverseLerp(boltClosedPosition, boltOpenPosition, transform.localPosition);
                    ClampControllerToTrack();
                    bolt.boltLerpVal = lerpValue;
                }
            }
            else if (hasMoved)
            {
                bolt.IsCurrentlyBeingManipulated(false);
                hasMoved = false;
                transform.localPosition = lastGoodPosition;
            }
            else
            {
                lerpValue = bolt.boltLerpVal;
            }
            if (lerpValue != oldLerpValue)                                                                                      // Moves the manipulator to be in the correct position (with the bolt object). Only 
            {                                                                                                                   // moves it if the lerp value has changed - doesn't need adjustment otherwise.
                transform.localPosition = Vector3.Lerp(boltClosedPosition, boltOpenPosition, lerpValue);
            }
        }

        void ClampControllerToTrack()
        {
            transform.localPosition = Vector3.Lerp(boltClosedPosition, boltOpenPosition, lerpValue);
            lastGoodPosition = transform.localPosition;
        }

    }
}