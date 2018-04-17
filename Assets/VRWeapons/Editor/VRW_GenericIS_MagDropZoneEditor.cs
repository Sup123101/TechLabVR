using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VRWeapons.InteractionSystems.Generic
{
    [CustomEditor(typeof(VRW_GenericIS_MagDropZone)), System.Serializable]

    public class VRW_GenericIS_MagDropZoneEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            VRW_GenericIS_MagDropZone dz = (VRW_GenericIS_MagDropZone)target;

            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Set Magazine location", "Clicking this button will set the drop zone's inserted magazine location to where " +
                "the \"Starting Mag\" is currently located.\n\nPlace the magazine appropriately, then click this button.\nIf the weapon does " +
                "not start with a magazine inserted, the Starting Mag field can be cleared after pressing this button.")))
            {
                if (dz.startingMag != null)
                {
                    Transform magLocation;
                    if (dz.magPosition != null && dz.magPosition != dz.startingMag)
                    {
                        magLocation = dz.magPosition;
                    }
                    else
                    {
                        magLocation = new GameObject("Mag Location").transform;                        
                    }
                    magLocation.parent = dz.transform.parent;
                    magLocation.position = dz.startingMag.position;
                    magLocation.rotation = dz.startingMag.rotation;
                    dz.magPosition = magLocation;
                    magLocation.gameObject.SetActive(false);
                }
            }
        }
    }
}