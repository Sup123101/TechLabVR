using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRWeapons.InteractionSystems.VRTK
{
    public class Magazine_VRTK_InteractableObject : VRTK_InteractableObject
    {
        ////////////////////
        /// Script is effectively empty - Just have this here in case I want to do something with it later.
        /// That way, people will already be using the script, so I don't need to disrupt anyone's work more than necessary.
        ////////////////////


        public override void OnInteractableObjectGrabbed(InteractableObjectEventArgs e)
        {            
            base.OnInteractableObjectGrabbed(e);
        }

        public override void OnInteractableObjectUngrabbed(InteractableObjectEventArgs e)
        {
            base.OnInteractableObjectUngrabbed(e);
        }
    }
}