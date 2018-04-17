using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace VRWeapons
{
    [CustomEditor(typeof(FireSelectRotator)), System.Serializable]

    public class FireSelectRotatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FireSelectRotator rotator = (FireSelectRotator)target;

            base.OnInspectorGUI();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Set \"Safe\" rotation", "Set fire selector in \"Safe\" position, and press this button."), GUILayout.Width(125)))
            {
                rotator.safePosition = rotator.transform.localEulerAngles;
            }
            if (GUILayout.Button(new GUIContent("Toggle position", "Toggle to Safe position."), GUILayout.Width(125)))
            {
                rotator.transform.localEulerAngles = rotator.safePosition;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Set \"Semi\" rotation", "Set fire selector in \"Semi\" position, and press this button."), GUILayout.Width(125)))
            {
                rotator.semiPosition = rotator.transform.localEulerAngles;
            }
            if (GUILayout.Button(new GUIContent("Toggle position", "Toggle to Semi position."), GUILayout.Width(125)))
            {
                rotator.transform.localEulerAngles = rotator.semiPosition;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Set \"Burst\" rotation", "Set fire selector in \"Burst\" position, and press this button."), GUILayout.Width(125)))
            {
                rotator.burstPosition = rotator.transform.localEulerAngles;
            }
            if (GUILayout.Button(new GUIContent("Toggle position", "Toggle to Burst position."), GUILayout.Width(125)))
            {
                rotator.transform.localEulerAngles = rotator.burstPosition;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Set \"Auto\" rotation", "Set fire selector in \"Auto\" position, and press this button."), GUILayout.Width(125)))
            {
                rotator.autoPosition = rotator.transform.localEulerAngles;
            }
            if (GUILayout.Button(new GUIContent("Toggle position", "Toggle to Auto position."), GUILayout.Width(125)))
            {
                rotator.transform.localEulerAngles = rotator.autoPosition;
            }
            GUILayout.EndHorizontal();
        } 
    }
}