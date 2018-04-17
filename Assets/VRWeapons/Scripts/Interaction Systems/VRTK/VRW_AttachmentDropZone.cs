using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

namespace VRWeapons.InteractionSystems.VRTK
{

    [RequireComponent(typeof(VRTK_SnapDropZone))]

    public class VRW_AttachmentDropZone : MonoBehaviour
    {

        VRTK_SnapDropZone dropZone;
        Weapon thisWeap;
        VRW_Attachment attachment;

        private void Start()
        {
            dropZone = GetComponent<VRTK_SnapDropZone>();
            dropZone.ObjectSnappedToDropZone += new SnapDropZoneEventHandler(ObjectSnapped);
            dropZone.ObjectUnsnappedFromDropZone += new SnapDropZoneEventHandler(ObjectUnsnapped);
            thisWeap = GetComponentInParent<Weapon>();
        }

        void ObjectSnapped(object sender, SnapDropZoneEventArgs e)
        {
            thisWeap.IgnoreCollision(e.snappedObject.GetComponentInChildren<Collider>(), true);

            attachment = e.snappedObject.GetComponent<VRW_Attachment>();

            if (attachment != null)
            {
                attachment.AttachToWeapon(thisWeap);
            }
        }

        void ObjectUnsnapped(object sender, SnapDropZoneEventArgs e)
        {
            thisWeap.IgnoreCollision(e.snappedObject.GetComponentInChildren<Collider>(), false);
            if (attachment != null)
            {
                attachment.DetachFromWeapon(thisWeap);
            }

            attachment = null;
        }
    }
}