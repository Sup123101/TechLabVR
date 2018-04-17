using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRWeapons.InteractionSystems.VRTK
{

    public class VRW_ControllerActions_VRTK : MonoBehaviour
    {
        [HideInInspector]
        public Weapon CurrentHeldWeapon;

        enum ButtonActions
        {
            None = 0,
            DropMagazine = 1,
            ChangeFireMode = 2,
            SlideRelease = 3
        }

        delegate void TouchpadUpPressedEvent(object sender, ControllerInteractionEventArgs e);
        delegate void TouchpadRightPressedEvent(object sender, ControllerInteractionEventArgs e);
        delegate void TouchpadDownPressedEvent(object sender, ControllerInteractionEventArgs e);
        delegate void TouchpadLeftPressedEvent(object sender, ControllerInteractionEventArgs e);
        delegate void Button1PressedEvent(object sender, ControllerInteractionEventArgs e);
        delegate void Button2PressedEvent(object sender, ControllerInteractionEventArgs e);

        event TouchpadUpPressedEvent OnTouchpadUpPressed;
        event TouchpadRightPressedEvent OnTouchpadRightPressed;
        event TouchpadDownPressedEvent OnTouchpadDownPressed;
        event TouchpadLeftPressedEvent OnTouchpadLeftPressed;
        event Button1PressedEvent OnButton1Pressed;
        event Button2PressedEvent OnButton2Pressed;

        [SerializeField]
        ButtonActions TouchpadLeft = ButtonActions.ChangeFireMode, TouchpadUp = ButtonActions.SlideRelease, TouchpadRight, TouchpadDown = ButtonActions.DropMagazine, Button1, Button2;
                
        private void Start()
        {
            VRTK_ControllerEvents events = GetComponent<VRTK_ControllerEvents>();

            events.TriggerAxisChanged += new ControllerInteractionEventHandler(TriggerAxisChanged);
            events.TouchpadPressed += new ControllerInteractionEventHandler(TouchpadPressed);
            events.ButtonOnePressed += new ControllerInteractionEventHandler(Button1Pressed);
            events.ButtonTwoPressed += new ControllerInteractionEventHandler(Button2Pressed);

            AssignButtons(events);
        }

        void AssignButtons(VRTK_ControllerEvents events)
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

            if (Button1 != ButtonActions.None)
            {
                switch (Button1)
                {
                    case ButtonActions.ChangeFireMode:
                        OnButton1Pressed += ChangeFireMode;
                        break;
                    case ButtonActions.DropMagazine:
                        OnButton1Pressed += DropMagazine;
                        break;
                    case ButtonActions.SlideRelease:
                        OnButton1Pressed += SlideRelease;
                        break;
                }
            }

            if (Button2 != ButtonActions.None)
            {
                switch (Button2)
                {
                    case ButtonActions.ChangeFireMode:
                        OnButton2Pressed += ChangeFireMode;
                        break;
                    case ButtonActions.DropMagazine:
                        OnButton2Pressed += DropMagazine;
                        break;
                    case ButtonActions.SlideRelease:
                        OnButton2Pressed += SlideRelease;
                        break;
                }
            }
        }
        
        private void TriggerAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (CurrentHeldWeapon != null && e.controllerReference.scriptAlias == CurrentHeldWeapon.holdingDevice)
            {
                CurrentHeldWeapon.SetTriggerAngle(e.buttonPressure);
            }
        }

        private void TouchpadPressed(object sender, ControllerInteractionEventArgs e)
        {
            if (e.touchpadAngle > 315 || e.touchpadAngle < 45)
            {
                if (OnTouchpadUpPressed != null)
                {
                    OnTouchpadUpPressed.Invoke(sender, e);
                }
            }
            else if (e.touchpadAngle > 45 && e.touchpadAngle < 135)
            {
                if (OnTouchpadRightPressed != null)
                {
                    OnTouchpadRightPressed.Invoke(sender, e);
                }
            }
            else if (e.touchpadAngle > 135 && e.touchpadAngle < 225)
            {
                if (OnTouchpadDownPressed != null)
                {
                    OnTouchpadDownPressed.Invoke(sender, e);
                }
            }
            else if (e.touchpadAngle > 225 && e.touchpadAngle < 315)
            {
                if (OnTouchpadLeftPressed != null)
                {
                    OnTouchpadLeftPressed.Invoke(sender, e);
                }
            }
        }

        private void Button1Pressed(object sender, ControllerInteractionEventArgs e)
        {
            if (OnButton1Pressed != null)
            {
                OnButton1Pressed.Invoke(sender, e);
            }
        }

        private void Button2Pressed(object sender, ControllerInteractionEventArgs e)
        {
            if (OnButton2Pressed != null)
            {
                OnButton2Pressed.Invoke(sender, e);
            }
        }

        private void ChangeFireMode(object sender, ControllerInteractionEventArgs e)
        {
            if (CurrentHeldWeapon != null && e.controllerReference.scriptAlias == CurrentHeldWeapon.holdingDevice)
            {
                CurrentHeldWeapon.ChangeFireMode();
            }
        }

        private void DropMagazine(object sender, ControllerInteractionEventArgs e)
        {
            if (CurrentHeldWeapon != null && e.controllerReference.scriptAlias == CurrentHeldWeapon.holdingDevice)
            {
                CurrentHeldWeapon.DropMagazine();
            }
        }

        private void SlideRelease(object sender, ControllerInteractionEventArgs e)
        {
            if (CurrentHeldWeapon != null && e.controllerReference.scriptAlias == CurrentHeldWeapon.holdingDevice)
            {
                CurrentHeldWeapon.SlideRelease();
            }
        }
    }
}