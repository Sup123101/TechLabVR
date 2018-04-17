using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons.InteractionSystems.Generic
{
    public class VRW_GenericIS_Controller : MonoBehaviour
    {
        public event GrabButtonPressedEvent OnGrabButtonDown;
        public event GrabButtonUnpressedEvent OnGrabButtonUp;
        public event TriggerPressedEvent OnTriggerPressDown;
        public event TriggerUnpressedEvent OnTriggerPressUp;
        public event DropMagPressedEvent OnDropMagButtonPressed;
        public event SlideReleasePressedEvent OnSlideReleasePressed;
        public event FireModePressedEvent OnFireModePressed;

        public delegate void GrabButtonPressedEvent();
        public delegate void GrabButtonUnpressedEvent();
        public delegate void TriggerPressedEvent();
        public delegate void TriggerUnpressedEvent();
        public delegate void DropMagPressedEvent();
        public delegate void SlideReleasePressedEvent();
        public delegate void FireModePressedEvent();

        delegate void TouchpadUpPressedEvent();
        delegate void TouchpadRightPressedEvent();
        delegate void TouchpadDownPressedEvent();
        delegate void TouchpadLeftPressedEvent();

        event TouchpadUpPressedEvent OnTouchpadUpPressed;
        event TouchpadRightPressedEvent OnTouchpadRightPressed;
        event TouchpadDownPressedEvent OnTouchpadDownPressed;
        event TouchpadLeftPressedEvent OnTouchpadLeftPressed;


        enum ButtonActions
        {
            None = 0,
            DropMagazine = 1,
            ChangeFireMode = 2,
            SlideRelease = 3
        }

        [SerializeField]
        ButtonActions TouchpadLeft = ButtonActions.ChangeFireMode, TouchpadUp = ButtonActions.SlideRelease, TouchpadRight, TouchpadDown = ButtonActions.DropMagazine;


        [SerializeField] public Valve.VR.EVRButtonId grabButton = Valve.VR.EVRButtonId.k_EButton_Grip;
        [SerializeField] Valve.VR.EVRButtonId fireButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
        Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;

        bool currentlyHoldingObject;
        SteamVR_Controller.Device thisDevice;
        SteamVR_TrackedObject thisTrackedObj;

        Vector2 touchpadAxis;

        GameObject controllerModel;

        private void Start()
        {
            controllerModel = transform.GetComponentInChildren<SteamVR_RenderModel>().gameObject;
            thisTrackedObj = GetComponent<SteamVR_TrackedObject>();
            thisDevice = SteamVR_Controller.Input((int)thisTrackedObj.index);

            AssignButtons();
        }

        void AssignButtons()
        {

            // This is a messy code-vomit - but until I can figure out a better way to do it, this works

            if (TouchpadLeft != ButtonActions.None)
            {
                switch (TouchpadLeft)
                {
                    case ButtonActions.ChangeFireMode:
                        OnTouchpadLeftPressed += ChangeFireMode;
                        break;
                    case ButtonActions.DropMagazine:
                        OnTouchpadLeftPressed += DropMagazine;
                        break;
                    case ButtonActions.SlideRelease:
                        OnTouchpadLeftPressed += SlideRelease;
                        break;
                }
            }

            if (TouchpadUp != ButtonActions.None)
            {
                switch (TouchpadUp)
                {
                    case ButtonActions.ChangeFireMode:
                        OnTouchpadUpPressed += ChangeFireMode;
                        break;
                    case ButtonActions.DropMagazine:
                        OnTouchpadUpPressed += DropMagazine;
                        break;
                    case ButtonActions.SlideRelease:
                        OnTouchpadUpPressed += SlideRelease;
                        break;
                }
            }

            if (TouchpadRight != ButtonActions.None)
            {
                switch (TouchpadRight)
                {
                    case ButtonActions.ChangeFireMode:
                        OnTouchpadRightPressed += ChangeFireMode;
                        break;
                    case ButtonActions.DropMagazine:
                        OnTouchpadRightPressed += DropMagazine;
                        break;
                    case ButtonActions.SlideRelease:
                        OnTouchpadRightPressed += SlideRelease;
                        break;
                }
            }

            if (TouchpadDown != ButtonActions.None)
            {
                switch (TouchpadDown)
                {
                    case ButtonActions.ChangeFireMode:
                        OnTouchpadDownPressed += ChangeFireMode;
                        break;
                    case ButtonActions.DropMagazine:
                        OnTouchpadDownPressed += DropMagazine;
                        break;
                    case ButtonActions.SlideRelease:
                        OnTouchpadDownPressed += SlideRelease;
                        break;
                }
            }
        }

        private void ChangeFireMode()
        {
            if (OnFireModePressed != null)
            {
                OnFireModePressed.Invoke();
            }
        }

        private void DropMagazine()
        {
            if (OnDropMagButtonPressed != null)
            {
                OnDropMagButtonPressed.Invoke();
            }
        }

        private void SlideRelease()
        {
            if (OnSlideReleasePressed != null)
            {
                OnSlideReleasePressed.Invoke();
            }
        }

        private void Update()
        {
            if (thisDevice.GetPressDown(fireButton))
            {
                if (OnTriggerPressDown != null)
                {
                    OnTriggerPressDown.Invoke();
                }
            }
            if (thisDevice.GetPressUp(fireButton))
            {
                if (OnTriggerPressUp != null)
                {
                    OnTriggerPressUp.Invoke();
                }
            }               
            if (thisDevice.GetPressDown(grabButton))
            {
                if (OnGrabButtonDown != null)
                {
                    OnGrabButtonDown.Invoke();
                }
            }
            if (thisDevice.GetPressUp(grabButton))
            {
                if (OnGrabButtonUp != null)
                {
                    OnGrabButtonUp.Invoke();
                }
            }
            if (thisDevice.GetPressDown(touchpad))
            {
                touchpadAxis = thisDevice.GetAxis(touchpad);
                float angle;
                if (touchpadAxis.x > 0)
                {
                    angle = Vector2.Angle(Vector2.up, touchpadAxis);
                }
                else
                {
                    angle = Vector2.Angle(Vector2.down, touchpadAxis) + 180;
                }
                if (angle > 315 || angle < 45)
                {
                    if (OnTouchpadUpPressed != null)
                    {
                        OnTouchpadUpPressed.Invoke();
                    }
                }
                else if (angle > 45 && angle < 135)
                {
                    if (OnTouchpadRightPressed != null)
                    {
                        OnTouchpadRightPressed.Invoke();
                    }
                }
                else if (angle > 135 && angle < 225)
                {
                    if (OnTouchpadDownPressed != null)
                    {
                        OnTouchpadDownPressed.Invoke();
                    }
                }
                else if (angle > 225 && angle < 315)
                {
                    if (OnTouchpadLeftPressed != null)
                    {
                        OnTouchpadLeftPressed.Invoke();
                    }
                }

            }

        }

        public SteamVR_TrackedObject TrackedObj
        {
            get { return thisTrackedObj; }
        }

        public SteamVR_Controller.Device Device
        {
            get { return thisDevice; }
        }

        public bool IsCurrentlyHoldingObject
        {
            get { return currentlyHoldingObject; }
            set { currentlyHoldingObject = value; }
        }

        public GameObject Model
        {
            get { return controllerModel; }
        }
    }
}