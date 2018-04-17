using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons.InteractionSystems.Generic
{
    [RequireComponent(typeof(Rigidbody))]
    public class VRW_GenericIS_Interactable : MonoBehaviour
    {

        VRW_GenericIS_Controller controller;

        VRWControl control;

        float dropTime;
        bool isHeld, wasPreviouslyKinematic;

        Rigidbody thisRB;

        Transform previousParent, currentParent;        

        [SerializeField] Vector3 grabbedPosition, grabbedRotation;

        [Tooltip("If checked, will not adjust position when grabbed."), SerializeField]
        bool precisionGrab;

        [Tooltip("If checked, user must hold down grip button to continue holding object. Otherwise, grabbing is toggled with each press."), SerializeField]
        bool holdButtonToGrab;

        private void Start()
        {
            thisRB = GetComponent<Rigidbody>();
            control = FindObjectOfType<VRWControl>();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!isHeld)
            {
                if (controller == null)
                {
                    if ((controller = other.GetComponent<VRW_GenericIS_Controller>()) != null && !controller.IsCurrentlyHoldingObject)
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
                if (control.disableControllersOnPickup)
                {
                    controller.Model.SetActive(false);
                }
                previousParent = transform.parent;
                wasPreviouslyKinematic = thisRB.isKinematic;

                transform.parent = controller.transform;
                currentParent = transform.parent;
                
                if (!precisionGrab)
                {
                    transform.localPosition = grabbedPosition;
                    transform.localEulerAngles = grabbedRotation;
                }

                thisRB.isKinematic = true;

                isHeld = true;

                controller.IsCurrentlyHoldingObject = true;
                controller.OnGrabButtonDown -= AttemptGrab;
                controller.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(Ungrab);
                if (holdButtonToGrab)
                {
                    controller.OnGrabButtonUp += new VRW_GenericIS_Controller.GrabButtonUnpressedEvent(Ungrab);
                }
            }
        }

        public void Ungrab()
        {
            if (control.disableControllersOnPickup)
            {
                controller.Model.SetActive(true);
            }
            transform.parent = previousParent;
            thisRB.isKinematic = wasPreviouslyKinematic;
            tossObject(thisRB);

            controller.IsCurrentlyHoldingObject = false;
            controller.OnGrabButtonDown -= AttemptGrab;
            controller.OnGrabButtonDown -= Ungrab;
            controller.OnGrabButtonUp -= Ungrab;
            controller = null;
            isHeld = false;
            dropTime = Time.time;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!isHeld && controller != null)
            {
                controller.OnGrabButtonDown -= AttemptGrab;
                controller.OnGrabButtonDown -= Ungrab;
                controller = null;
            }
        }

        private void Update()
        {
            if (isHeld)
            {
                if (transform.parent != currentParent)
                {
                    Ungrab();
                }
            }            
        }

        void tossObject(Rigidbody rigidbody)
        {
            Transform origin = controller.TrackedObj.origin ? controller.TrackedObj.origin : controller.TrackedObj.transform.parent;

            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(controller.Device.velocity);
                rigidbody.angularVelocity = origin.TransformVector(controller.Device.angularVelocity);
            }
            else
            {
                rigidbody.velocity = controller.Device.velocity;
                rigidbody.angularVelocity = controller.Device.angularVelocity;
            }
        }

    }
}