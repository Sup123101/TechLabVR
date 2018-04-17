using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons.InteractionSystems.Generic
{
    [RequireComponent(typeof(Rigidbody))]

    public class VRW_GenericIS_InteractableWeapon : MonoBehaviour
    {
        Weapon thisWeap;

        VRWControl control;

        VRW_GenericIS_Controller mainHandController, secondHandController;
        SteamVR_Controller.Device device, secondHandDevice;
        [HideInInspector]
        public SteamVR_TrackedObject trackedObj, secondHandTrackedObj;

        [SerializeField] public Valve.VR.EVRButtonId grabButton = Valve.VR.EVRButtonId.k_EButton_Grip;
        [SerializeField] Valve.VR.EVRButtonId fireButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
        [SerializeField] Valve.VR.EVRButtonId dropMagButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

        [Tooltip("If checked, second hand grip button must be held down to remain gripping."), SerializeField]
        public bool holdButtonTo2HandGrip;

        [Tooltip("How long in frames haptics will fire when weapon is fired."), SerializeField]
        int hapticTime = 6;

        [Tooltip("Haptic strength per pulse"), SerializeField, Range(0, 3999)]
        ushort hapticStrength = 3999;

        Rigidbody thisRB;

        bool isHeld, secondHandGripped, wasPreviouslyKinematic, isColliding;

        Transform previousParent;

        float dropTime;

        private void Start()
        {
            thisWeap = GetComponent<Weapon>();
            thisWeap.shotHaptics += new VRWControl.TriggerHaptics(ShotHaptics);
            thisRB = GetComponent<Rigidbody>();
            DisableWeaponColliders(isHeld);
            control = FindObjectOfType<VRWControl>();
        }

        void ShotHaptics()
        {
            StartCoroutine(TriggerHaptics());
        }

        IEnumerator TriggerHaptics()
        {
            if (device != null)
            {
                for (int i = 0; i < hapticTime; i++)
                {
                    if (secondHandGripped && secondHandDevice != null)
                    {
                        secondHandDevice.TriggerHapticPulse(hapticStrength);
                    }
                    if (device != null)
                    {
                        device.TriggerHapticPulse(hapticStrength);
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        
        private void OnTriggerStay(Collider other)
        {
            if (!isHeld)
            {
                if (mainHandController == null)
                {
                    if ((mainHandController = other.GetComponent<VRW_GenericIS_Controller>()) != null && !mainHandController.IsCurrentlyHoldingObject)
                    {
                        device = mainHandController.Device;
                        trackedObj = mainHandController.TrackedObj;
                        mainHandController.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(GrabObject);
                    }
                    isColliding = true;
                }
            }
            else
            {
                if (secondHandController == null)
                {
                    if (other.gameObject != mainHandController.gameObject && (secondHandController = other.GetComponent<VRW_GenericIS_Controller>()) != null && !secondHandController.IsCurrentlyHoldingObject)
                    {
                        secondHandTrackedObj = secondHandController.TrackedObj;
                        secondHandDevice = secondHandController.Device;
                        secondHandController.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(SecondHandGrab);
                    }
                    isColliding = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            isColliding = false;
            if (!isHeld && mainHandController != null)
            {
                mainHandController.OnGrabButtonDown -= GrabObject;
                mainHandController.OnTriggerPressDown -= StartFiring;
                mainHandController.OnTriggerPressUp -= StopFiring;
                mainHandController.OnDropMagButtonPressed -= DropMagButtonPressed;
                mainHandController.OnFireModePressed -= FireModePressed;
                mainHandController.OnSlideReleasePressed -= SlideReleasePressed;
                mainHandController = null;
            }
            if (secondHandController != null && !secondHandGripped)
            {
                secondHandController.OnGrabButtonDown -= SecondHandGrab;
                secondHandController.OnGrabButtonDown -= SecondHandUngrab;
                secondHandController = null;
                thisWeap.secondHandGripped = false;
                secondHandGripped = false;
            }
        }
        
        void SecondHandGrab()
        {
            if (control.disableControllersOnPickup)
            {
                secondHandController.Model.SetActive(false);
            }
            thisWeap.secondHandDevice = secondHandController.gameObject;
            thisWeap.secondHandGripped = true;
            secondHandGripped = true;
            dropTime = Time.time;
            secondHandController.OnGrabButtonDown -= SecondHandGrab;
            secondHandController.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(SecondHandUngrab);
        }

        void SecondHandUngrab()
        {
            if (control.disableControllersOnPickup)
            {
                secondHandController.Model.SetActive(true);
            }
            thisWeap.secondHandDevice = null;
            thisWeap.secondHandGripped = false;
            secondHandGripped = false;
            dropTime = Time.time;
            secondHandController.OnGrabButtonDown -= SecondHandUngrab;
            secondHandController = null;
            Realign();
        }

        void GrabObject()
        {            
            if (!mainHandController.IsCurrentlyHoldingObject)
            {
                if (control.disableControllersOnPickup)
                {
                    mainHandController.Model.SetActive(false);
                }
                mainHandController.IsCurrentlyHoldingObject = true;
                wasPreviouslyKinematic = thisRB.isKinematic;
                thisRB.isKinematic = true;
                previousParent = transform.parent;

                Realign();

                transform.parent = trackedObj.transform;

                thisWeap.WeaponPickedUp();

                thisWeap.holdingDevice = mainHandController.gameObject;

                isHeld = true;
                thisWeap.isHeld = true;
                dropTime = Time.time;

                DisableWeaponColliders(isHeld);
                mainHandController.OnGrabButtonDown -= GrabObject;
                mainHandController.OnTriggerPressDown += new VRW_GenericIS_Controller.TriggerPressedEvent(StartFiring);
                mainHandController.OnTriggerPressUp += new VRW_GenericIS_Controller.TriggerUnpressedEvent(StopFiring);
                mainHandController.OnDropMagButtonPressed += new VRW_GenericIS_Controller.DropMagPressedEvent(DropMagButtonPressed);
                mainHandController.OnSlideReleasePressed += new VRW_GenericIS_Controller.SlideReleasePressedEvent(SlideReleasePressed);
                mainHandController.OnFireModePressed += new VRW_GenericIS_Controller.FireModePressedEvent(FireModePressed);
                mainHandController.OnGrabButtonDown += new VRW_GenericIS_Controller.GrabButtonPressedEvent(UngrabObject);
            }
        }

        void UngrabObject()
        {
            if (control.disableControllersOnPickup)
            {
                mainHandController.Model.SetActive(true);
                if (secondHandController != null)
                {
                    secondHandController.Model.SetActive(true);
                }
            }
            mainHandController.IsCurrentlyHoldingObject = false;
            thisRB.isKinematic = wasPreviouslyKinematic;
            transform.parent = previousParent;

            tossObject(thisRB);

            isHeld = false;
            thisWeap.isHeld = false;

            DisableWeaponColliders(isHeld);

            thisWeap.WeaponDropped();

            dropTime = Time.time;
            mainHandController.OnGrabButtonDown -= UngrabObject;
            mainHandController.OnTriggerPressDown -= StartFiring;
            mainHandController.OnTriggerPressUp -= StopFiring;
            mainHandController.OnDropMagButtonPressed -= DropMagButtonPressed;
            mainHandController.OnSlideReleasePressed -= SlideReleasePressed;
            mainHandController.OnFireModePressed -= FireModePressed;
            mainHandController = null;
            device = null;
            trackedObj = null;
            thisWeap.secondHandGripped = false;
            secondHandGripped = false;
        }

        void StartFiring()
        {
            thisWeap.StartFiring(trackedObj.gameObject);
        }

        void StopFiring()
        {
            thisWeap.StopFiring(trackedObj.gameObject);
        }

        void DropMagButtonPressed()
        {
            thisWeap.DropMagazine();
        }

        void SlideReleasePressed()
        {
            thisWeap.SlideRelease();
        }

        void FireModePressed()
        {
            thisWeap.ChangeFireMode();
        }

        private void Update()
        {
            if (isHeld)
            {
                if (device.GetTouch(fireButton))
                {
                    thisWeap.SetTriggerAngle(device.GetAxis(fireButton).x);
                }
                if (secondHandGripped)
                {
                    transform.rotation = VRWControl.ZLockedAim(mainHandController.transform, secondHandController.transform, transform);
                }
            }
            else if (!isColliding && !isHeld)
            {
                mainHandController = null;
                device = null;
                trackedObj = null;
                secondHandDevice = null;
                secondHandTrackedObj = null;
            }
        }

        void tossObject(Rigidbody rigidbody)
        {
            Transform origin = trackedObj.origin ? trackedObj.origin : trackedObj.transform.parent;

            if (origin != null)
            {
                rigidbody.velocity = origin.TransformVector(device.velocity);
                rigidbody.angularVelocity = origin.TransformVector(device.angularVelocity);
            }
            else
            {
                rigidbody.velocity = device.velocity;
                rigidbody.angularVelocity = device.angularVelocity;
            }
        }

        void DisableWeaponColliders(bool isGripped)
        {
            thisWeap.weaponBodyCollider.enabled = !isGripped;
            if (thisWeap.secondHandGripCollider != null)
            {
                thisWeap.secondHandGripCollider.enabled = isGripped;
            }
        }

        void Realign()
        {
            transform.rotation = trackedObj.transform.rotation * Quaternion.Euler(thisWeap.grabPoint.localEulerAngles);
            transform.position = trackedObj.transform.position - (thisWeap.grabPoint.position - transform.position);
        }
    }
}
