using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRWeapons {

    public class VRW_Attachment: MonoBehaviour
    { 
        public delegate void AttachmentUsedEvent(GameObject objectAttached);
        public delegate void AttachedToWeapEvent(Weapon weapon);
        public delegate void DetachedFromWeapEvent(Weapon prevWeapon);
        
        public event AttachmentUsedEvent OnAttachmentUsed;
        public event AttachedToWeapEvent OnAttachedToWeap;
        public event DetachedFromWeapEvent OnDetachedFromWeap;

        Weapon currentWeap;

        public void AttachToWeapon(Weapon newWeap)
        {
            currentWeap = newWeap;
            
            if (OnAttachedToWeap != null)
            {
                OnAttachedToWeap.Invoke(currentWeap);
            }
        }

        public void UseAttachment(GameObject objectAttached)
        {
            if (OnAttachmentUsed != null)
            {
                OnAttachmentUsed.Invoke(this.gameObject);
            }
        }

        public void DetachFromWeapon(Weapon prevWeapon)
        {
            if (OnDetachedFromWeap != null)
            {
                OnDetachedFromWeap.Invoke(currentWeap);
            }

            currentWeap = null;
        }
    }
}