﻿using UnityEngine;
using UnityEditor;
using VRTK;
using VRTK.GrabAttachMechanics;

namespace VRWeapons.InteractionSystems.VRTK
{
    public class VRW_ObjectSetupVRTK : EditorWindow
    {        
        enum MagazineType
        {
            Simple = 0,
            Complex = 1
        }

        enum BoltType
        {
            StraightBack = 0,
            RevolverStyle = 1
        }

        enum BulletType
        {
            RaycastBullet = 0,
            ShotgunShell = 1,
            Projectile = 2
        }

        BulletType bulletTypeForMag, bulletType;

        MagazineType magType;

        BoltType boltType;

        bool twoHanded, isInteractable;

        GameObject target, slide, projectile;

        [MenuItem("Window/VRWeapons/Set up new Weapon (VRTK)")]
        private static void Init()
        {
            VRW_ObjectSetupVRTK window = (VRW_ObjectSetupVRTK)GetWindow(typeof(VRW_ObjectSetupVRTK));
            window.minSize = new Vector2(300f, 332f);
            window.maxSize = new Vector2(300f, 375f);

            window.autoRepaintOnSceneChange = true;
            window.titleContent.text = "Weapon setup VRTK";
            window.Show();
        }

        private void OnGUI()
        {
            target = Selection.activeGameObject;
            twoHanded = EditorGUILayout.Toggle("2-handed weapon", twoHanded);
            if (GUILayout.Button(new GUIContent("Add base weapon script to selected object", "")))
            {
                if (target != null)
                {
                    if (target.GetComponent<Weapon>() == null)
                    {
                        target.AddComponent<Weapon>();
                    }
                    else
                    {
                        Debug.LogWarning("Weapon already found on " + target + ". No Weapon added.");
                    }
                    target.GetComponent<AudioSource>().playOnAwake = false;
                    if (target.GetComponent<IKickActions>() == null)
                    {
                        target.AddComponent<KinematicKick>();
                    }
                    else
                    {
                        Debug.LogWarning("KinematicKick already found on " + target + ". No KinematicKick added.");
                    }
                    if (target.GetComponent<IObjectPool>() == null)
                    {
                        target.AddComponent<ObjectPool>();
                    }
                    else
                    {
                        Debug.LogWarning("ObjectPool already found on " + target + ". No ObjectPool added.");
                    }
                    
                    if (target.GetComponent<Weapon_VRTK_InteractableObject>() == null)
                    {
                        Weapon_VRTK_InteractableObject tmp = target.AddComponent<Weapon_VRTK_InteractableObject>();
                        tmp.isUsable = true;
                        tmp.isGrabbable = true;
                        tmp.holdButtonToGrab = false;
                        tmp.holdButtonToUse = true;
                        tmp.useOnlyIfGrabbed = true;
                        if (target.GetComponentInChildren<Collider>() != null)
                        {
                            target.GetComponent<Weapon>().weaponBodyCollider = target.GetComponentInChildren<Collider>();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Weapon_VRTK_InteractableObject already found on " + target + ". No Weapon_VRTK_InteractableObject added.");
                    }
                    if (target.GetComponent<VRTK_ChildOfControllerGrabAttach>() == null)
                    {
                        target.AddComponent<VRTK_ChildOfControllerGrabAttach>();
                    }
                    else
                    {
                        Debug.LogWarning("VRTK_ChildOfControllerGrabAttach already found on " + target + ". No VRTK_ChildOfControllerGrabAttach added.");
                    }
                    target.GetComponent<Weapon_VRTK_InteractableObject>().grabAttachMechanicScript = target.GetComponent<VRTK_ChildOfControllerGrabAttach>();

                    if (twoHanded)
                    {
                        if (target.GetComponent<Weapon_VRTK_ControlDirectionGrabAction>() == null)
                        {
                            target.AddComponent<Weapon_VRTK_ControlDirectionGrabAction>();
                        }
                        else
                        {
                            Debug.LogWarning("Weapon_VRTK_ControlDirectionGrabAction already found on " + target + ". No Weapon_VRTK_ControlDirectionGrabAction added.");
                        }
                        target.GetComponent<Weapon_VRTK_InteractableObject>().secondaryGrabActionScript = target.GetComponent<Weapon_VRTK_ControlDirectionGrabAction>();
                    }
                    if (FindObjectOfType<VRTK_SDKManager>() != null)
                    {
                        VRTK_SDKManager tmp = FindObjectOfType<VRTK_SDKManager>();
                        if (tmp.scriptAliasLeftController != null)
                        {
                            if (tmp.scriptAliasLeftController.GetComponent<VRW_ControllerActions_VRTK>() == null)
                            {
                                tmp.scriptAliasLeftController.AddComponent<VRW_ControllerActions_VRTK>();
                            }
                            if (tmp.scriptAliasLeftController.GetComponent<VRTK_ControllerEvents>() == null)
                            {
                                tmp.scriptAliasLeftController.AddComponent<VRTK_ControllerEvents>();
                            }
                            if (tmp.scriptAliasLeftController.GetComponent<VRTK_InteractTouch>() == null)
                            {
                                tmp.scriptAliasLeftController.AddComponent<VRTK_InteractTouch>();
                            }
                            if (tmp.scriptAliasLeftController.GetComponent<VRTK_InteractGrab>() == null)
                            {
                                tmp.scriptAliasLeftController.AddComponent<VRTK_InteractGrab>();
                                tmp.scriptAliasLeftController.GetComponent<VRTK_InteractGrab>().controllerEvents = tmp.scriptAliasLeftController.GetComponent<VRTK_ControllerEvents>();
                            }
                            if (tmp.scriptAliasLeftController.GetComponent<VRTK_InteractUse>() == null)
                            {
                                tmp.scriptAliasLeftController.AddComponent<VRTK_InteractUse>();
                                tmp.scriptAliasLeftController.GetComponent<VRTK_InteractUse>().controllerEvents = tmp.scriptAliasLeftController.GetComponent<VRTK_ControllerEvents>();
                                tmp.scriptAliasLeftController.GetComponent<VRTK_InteractUse>().interactGrab = tmp.scriptAliasLeftController.GetComponent<VRTK_InteractGrab>();
                            }
                        }
                        else
                        {
                            Debug.LogError("No VRTK left controller alias found. Please add a controller alias for the left controller.");
                        }
                        if (tmp.scriptAliasRightController != null)
                        {
                            if (tmp.scriptAliasRightController.GetComponent<VRW_ControllerActions_VRTK>() == null)
                            {
                                tmp.scriptAliasRightController.AddComponent<VRW_ControllerActions_VRTK>();
                            }
                            if (tmp.scriptAliasRightController.GetComponent<VRTK_ControllerEvents>() == null)
                            {
                                tmp.scriptAliasRightController.AddComponent<VRTK_ControllerEvents>();
                            }
                            if (tmp.scriptAliasRightController.GetComponent<VRTK_InteractTouch>() == null)
                            {
                                tmp.scriptAliasRightController.AddComponent<VRTK_InteractTouch>();
                            }
                            if (tmp.scriptAliasRightController.GetComponent<VRTK_InteractGrab>() == null)
                            {
                                tmp.scriptAliasRightController.AddComponent<VRTK_InteractGrab>();
                                tmp.scriptAliasRightController.GetComponent<VRTK_InteractGrab>().controllerEvents = tmp.scriptAliasRightController.GetComponent<VRTK_ControllerEvents>();
                            }
                            if (tmp.scriptAliasRightController.GetComponent<VRTK_InteractUse>() == null)
                            {
                                tmp.scriptAliasRightController.AddComponent<VRTK_InteractUse>();
                                tmp.scriptAliasRightController.GetComponent<VRTK_InteractUse>().controllerEvents = tmp.scriptAliasRightController.GetComponent<VRTK_ControllerEvents>();
                                tmp.scriptAliasRightController.GetComponent<VRTK_InteractUse>().interactGrab = tmp.scriptAliasRightController.GetComponent<VRTK_InteractGrab>();
                            }
                        }
                        else
                        {
                            Debug.LogError("No VRTK right controller alias found. Please add a controller alias for the right controller.");
                        }
                    }
                    GameObject grabPoint;
                    if (target.transform.Find("Grab Point") == null)
                    {
                        grabPoint = new GameObject("Grab Point");
                    }
                    else
                    {
                        grabPoint = target.transform.Find("Grab Point").gameObject;
                    }
                    grabPoint.transform.parent = target.transform;
                    grabPoint.transform.localPosition = Vector3.zero;
                    target.GetComponent<VRTK_ChildOfControllerGrabAttach>().rightSnapHandle = grabPoint.transform;
                    
                }
                if (FindObjectOfType<VRWControl>() == null)
                {
                    Debug.Log("No VRWControl found. Adding new VRWControl object to scene.");
                    GameObject tmp = new GameObject("VRWControl");
                    tmp.AddComponent<VRWControl>();
                }
            }
            if (GUILayout.Button(new GUIContent("Create new Muzzle object", "Align muzzle with weapon as desired, with the muzzle object's forward direction pointing " +
                "where the gun should fire.")))
            {
                if (target.GetComponent<Weapon>() == null)
                {
                    Debug.LogError("No Weapon script found on " + target + ". Please select an object with a valid Weapon script.");
                }
                else if (target != null && target.GetComponentInChildren<IMuzzleActions>() == null)
                {
                    GameObject muzzle = new GameObject();
                    muzzle.transform.parent = target.transform;
                    muzzle.transform.localPosition = Vector3.zero;
                    muzzle.AddComponent<Muzzle>();
                    muzzle.name = "Muzzle";
                    if (muzzle.GetComponent<AudioSource>() == null)
                    {
                        muzzle.AddComponent<AudioSource>();
                    }
                    muzzle.GetComponent<AudioSource>().playOnAwake = false;
                    Selection.activeGameObject = muzzle;
                }
                else
                {
                    Debug.LogWarning("Class implementing IMuzzleActions already found on " + target + ". No Muzzle added.");
                }
            }

            boltType = (BoltType)EditorGUILayout.EnumPopup("Bolt Type", boltType);
            if (boltType == BoltType.RevolverStyle)
            {
                EditorGUILayout.LabelField("Revolver functionality not complete.");
                EditorGUILayout.LabelField("Check back later.");
            }

            if (GUILayout.Button(new GUIContent("Assign bolt to selected object", "Bolt should be a separate object - the part that moves when the weapon fires, " +
                "that the player is able to pull back to charge the weapon. \n\nIf no bolt exists on your model, then assign to an empty GameObject and adjust slide time " +
                "as required for fire rate.")))
            {
                slide = (GameObject)Selection.activeObject;
                target = slide.GetComponentInParent<Weapon>().gameObject;
                if (slide == null)
                {
                    Debug.LogError("No GameObject selected. Please select a GameObject to add a bolt.");
                }
                else if (slide.GetComponentInParent<Weapon>() == null)
                {
                    Debug.LogError("No Weapon script found on " + slide + "'s parent objects. Please select an object that is a child of a valid Weapon script.");
                }
                else if (slide == target)
                {
                    Debug.LogError("Attempted to set up the weapon object as the bolt object. Please select desired bolt, instead of the weapon itself.");
                }
                else 
                {
                    if (boltType == BoltType.StraightBack)
                    {
                        if (slide.GetComponent<IBoltActions>() == null)
                        {
                            Bolt tmp = slide.AddComponent<Bolt>();
                            tmp.BoltEndPosition = tmp.transform.localPosition;
                            tmp.BoltStartPosition = tmp.transform.localPosition;
                            tmp.BoltRotationStart = tmp.transform.localEulerAngles;
                            tmp.BoltRotationEnd = tmp.transform.localEulerAngles;
                            tmp.bolt = slide.transform;
                        }
                        else
                        {
                            Debug.LogWarning("Class implementing IBoltActions already found on " + slide + ". No bolt added.");
                        }
                        
                        if (target.GetComponentInChildren<Bolt_InteractableObject>() == null)
                        {
                            GameObject slideControl = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            slideControl.name = "Bolt Grab Point";
                            slideControl.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                            slideControl.transform.parent = target.transform;
                            slideControl.transform.localPosition = Vector3.zero;
                            var tempMaterial = new Material(slideControl.GetComponent<Renderer>().sharedMaterial);
                            tempMaterial.color = Color.red;
                            slideControl.GetComponent<MeshRenderer>().sharedMaterial = tempMaterial;

                            Rigidbody rb = slideControl.AddComponent<Rigidbody>();
                            rb.isKinematic = true;

                            Bolt_InteractableObject tmp = slideControl.AddComponent<Bolt_InteractableObject>();
                            tmp.isGrabbable = true;
                            tmp.holdButtonToGrab = true;
                            tmp.isUsable = false;
                            tmp.disableWhenIdle = false;

                            VRTK_TrackObjectGrabAttach tmpTrack = slideControl.AddComponent<VRTK_TrackObjectGrabAttach>();
                            tmpTrack.precisionGrab = true;
                            tmp.grabAttachMechanicScript = tmpTrack;
                            Selection.activeGameObject = slideControl;
                        }
                        
                    }
                    else if (boltType == BoltType.RevolverStyle)
                    {
                        if (slide.GetComponent<IBoltActions>() == null)
                        {
                            slide.AddComponent<Bolt_Rotating>();
                        }
                        else
                        {
                            Debug.LogWarning("Class implementing IBoltActions already found on " + slide + ". No bolt added.");
                        }
                    }
                }
            }
            if (GUILayout.Button(new GUIContent("Create new Ejector", "Location of the ejector does not matter, but face ejector so that forward is ejection direction.")))
            {
                bool weaponFound = false;
                GameObject parent = null;
                if (target.GetComponent<Weapon>() == null)
                {
                    if (target.GetComponentInParent<Weapon>() == null)
                    {
                        Debug.LogError("No Weapon script found on " + target.name + ". Add a Weapon script first.");
                    }
                    else
                    {
                        parent = target.GetComponentInParent<Weapon>().gameObject;
                        weaponFound = true;
                    }
                }
                else
                {
                    parent = target.GetComponent<Weapon>().gameObject;
                    weaponFound = true;
                }
                if (weaponFound && parent.GetComponentInChildren<IEjectorActions>() == null)
                {
                    GameObject ejector = new GameObject();
                    ejector.name = "Ejector";
                    ejector.transform.parent = parent.transform;
                    ejector.transform.localPosition = Vector3.zero;
                    ejector.AddComponent<Ejector>();
                    ejector.transform.localEulerAngles = new Vector3(-15, 90, 0);
                    Selection.activeGameObject = ejector;
                }
                
            }
            magType = (MagazineType)EditorGUILayout.EnumPopup("Magazine Type", magType);

            if (magType == MagazineType.Simple)
            {
                bulletTypeForMag = (BulletType)EditorGUILayout.EnumPopup("Bullet Type", bulletTypeForMag);
            }

            if (GUILayout.Button(new GUIContent("Set up selected object as Magazine", "Simple magazines should have a single bullet type added as a component to the magazine. \n\nComplex " +
                "magazines are capable of individual bullet behavior, and require rounds to be set up in the magazine. \n\nIf weapon has an internal magazine, select the weapon and press this.")))
            {
                GameObject newMag = (GameObject)Selection.activeObject;
                if (magType == MagazineType.Simple)
                {
                    if (newMag.GetComponent<IMagazine>() == null)
                    {
                        newMag.AddComponent<SimpleMagazine>();
                        if (newMag.GetComponent<Weapon>() != null)
                        {
                            newMag.GetComponent<IMagazine>().CanMagBeDetached = false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Class implementing IMagazine already found on " + newMag + ". No magazine added.");
                    }
                    if (newMag.GetComponent<IBulletBehavior>() == null && newMag.GetComponent<IMagazine>() != null)
                    {
                        switch (bulletTypeForMag)
                        {
                            case BulletType.Projectile:
                                newMag.AddComponent<BulletTypes.ProjectileBullet>();
                                break;
                            case BulletType.RaycastBullet:
                                newMag.AddComponent<BulletTypes.RaycastBullet>();
                                break;
                            case BulletType.ShotgunShell:
                                newMag.AddComponent<BulletTypes.ShotgunBullet>();
                                break;
                        }
                    }
                }
                else if (magType == MagazineType.Complex)
                {
                    if (newMag.GetComponent<IMagazine>() == null)
                    {
                        newMag.AddComponent<Magazine>();
                        if (newMag.GetComponent<Weapon>() != null)
                        {
                            newMag.GetComponent<IMagazine>().CanMagBeDetached = false;
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Class implementing IMagazine already found on " + newMag + ". No magazine added.");
                    }
                }

                if (newMag.GetComponent<Collider>() == null && newMag.GetComponent<Weapon>() == null)
                {
                    newMag.AddComponent<BoxCollider>();
                }

                if (newMag.GetComponent<Rigidbody>() == null)
                {
                    newMag.AddComponent<Rigidbody>();
                }

                
                if (newMag.GetComponent<Magazine_VRTK_InteractableObject>() == null && newMag.GetComponent<Weapon>() == null)
                {
                    newMag.AddComponent<Magazine_VRTK_InteractableObject>();
                    newMag.GetComponent<Magazine_VRTK_InteractableObject>().isGrabbable = true;
                    newMag.GetComponent<Magazine_VRTK_InteractableObject>().holdButtonToGrab = false;
                }
                
            }
                        
            if (GUILayout.Button(new GUIContent("Set up Magazine DropZone", "For VRTK, use a dropzone for magazine insertion. Use BulletDropZone for loading individual rounds into " +
                "magazines.")))
            {
                if (target.GetComponent<Weapon>() == null)
                {
                    Debug.LogError("No Weapon script found on " + target + ". Please select an object with a valid Weapon script.");
                }
                else if (target.GetComponentInChildren<MagDropZone>() != null)
                {
                    Debug.LogWarning("Magazine drop zone already found on " + target + ". No drop zone created.");
                }
                else
                {
                    GameObject dz = new GameObject();
                    Undo.RecordObject(dz, "Set up Magazine Drop Zone");

                    dz.name = "Mag Drop Zone";
                    dz.AddComponent<MagDropZone>();

                    dz.transform.parent = target.transform;
                    dz.transform.localPosition = Vector3.zero;
                        
                    if (dz.GetComponent<VRTK_SnapDropZone>() == null)
                    {
                        dz.AddComponent<VRTK_SnapDropZone>();
                    }
                    if (dz.GetComponent<VRTK_PolicyList>() == null)
                    {
                        dz.AddComponent<VRTK_PolicyList>();
                    }
                    dz.GetComponent<VRTK_SnapDropZone>().validObjectListPolicy = dz.GetComponent<VRTK_PolicyList>();
                    dz.GetComponent<VRTK_PolicyList>().operation = VRTK_PolicyList.OperationTypes.Include;         
                    dz.GetComponent<VRTK_SnapDropZone>().snapType = VRTK_SnapDropZone.SnapTypes.UseParenting;

                    if (dz.GetComponent<Collider>() == null)
                    {
                        dz.AddComponent<SphereCollider>();
                        dz.GetComponent<SphereCollider>().radius = 0.05f;
                        dz.GetComponent<SphereCollider>().isTrigger = true;
                    }
                    if (dz.GetComponent<Rigidbody>() == null)
                    {
                        dz.AddComponent<Rigidbody>();
                    }
                    dz.GetComponent<Rigidbody>().isKinematic = true;
                    if (target.GetComponentInChildren<IMagazine>() != null)
                    {
                        MonoBehaviour e = target.GetComponentInChildren<IMagazine>() as MonoBehaviour;
                        dz.transform.localPosition = e.transform.localPosition;
                        dz.transform.localEulerAngles = e.transform.localEulerAngles;
                        dz.GetComponent<VRTK_SnapDropZone>().highlightObjectPrefab = e.gameObject;
                        dz.GetComponent<VRTK_SnapDropZone>().defaultSnappedObject = e.gameObject;
                        e.transform.parent = null;
                    }
                    Selection.activeGameObject = dz;
                }
            }
            if (GUILayout.Button(new GUIContent("Set up Bullet DropZone", "For VRTK, this drop zone is to add individual rounds to a complex magazine.\n\n" +
                "If the weapon has an internal magazine, use this to load rounds into it.")))
            {
                GameObject mag = (GameObject)Selection.activeObject;
                if (mag.GetComponent<IMagazine>() == null)
                {
                    Debug.LogError("No IMagazine script found on " + mag + ". Please set up magazine before setting up Bullet DropZone.");
                }
                else if (mag.GetComponentInChildren<BulletDropZone>() != null)
                {
                    Debug.LogWarning("Bullet Drop Zone already found on " + mag + ". No drop zone added.");
                }
                else
                {
                    GameObject dz = new GameObject();
                    Undo.RecordObject(dz, "Set up Bullet Drop Zone");
                    dz.name = "Bullet Drop Zone";
                    dz.AddComponent<BulletDropZone>();
                    if (dz.GetComponent<VRTK_SnapDropZone>() == null)
                    {
                        dz.AddComponent<VRTK_SnapDropZone>();
                    }
                    if (dz.GetComponent<VRTK_PolicyList>() == null)
                    {
                        dz.AddComponent<VRTK_PolicyList>();
                    }
                    dz.GetComponent<VRTK_SnapDropZone>().validObjectListPolicy = dz.GetComponent<VRTK_PolicyList>();
                    dz.GetComponent<VRTK_PolicyList>().operation = VRTK_PolicyList.OperationTypes.Include;
                    dz.GetComponent<VRTK_SnapDropZone>().snapType = VRTK_SnapDropZone.SnapTypes.UseParenting;
                    if (dz.GetComponent<Collider>() == null)
                    {
                        dz.AddComponent<BoxCollider>();
                        dz.GetComponent<BoxCollider>().size = new Vector3(0.025f, 0.035f, 0.065f);
                    }
                    dz.GetComponent<Collider>().isTrigger = true;
                    dz.transform.parent = mag.transform;
                    dz.transform.localPosition = Vector3.zero;
                    Selection.activeGameObject = dz;
                }
            }
            
            EditorGUILayout.TextArea("", GUI.skin.horizontalSlider);

            GUIStyle centeredStyle = GUI.skin.GetStyle("Label");
            centeredStyle.alignment = TextAnchor.UpperCenter;

            EditorGUILayout.LabelField("Bullet Builder", centeredStyle);

            bulletType = (BulletType)EditorGUILayout.EnumPopup("Bullet Type", bulletType);

            if (bulletType == BulletType.Projectile)
            {
                projectile = (GameObject)EditorGUILayout.ObjectField("Projectile Object", projectile, typeof(Object), true);
            }

            isInteractable = EditorGUILayout.Toggle("Bullet can be picked up", isInteractable);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(new GUIContent("Set Up\nBullet", "Press this button to set up a bullet for use in complex magazines.")))
            {
                if (target.GetComponent<IBulletBehavior>() == null)
                {
                    switch (bulletType)
                    {
                        case BulletType.RaycastBullet:
                            target.AddComponent<BulletTypes.RaycastBullet>();
                            break;
                        case BulletType.ShotgunShell:
                            target.AddComponent<BulletTypes.ShotgunBullet>();
                            break;
                        case BulletType.Projectile:
                            {
                                if (projectile != null)
                                {
                                    if (projectile.GetComponent<Rigidbody>() == null)
                                    {
                                        projectile.AddComponent<Rigidbody>();
                                    }
                                    target.AddComponent<BulletTypes.ProjectileBullet>();
                                    target.GetComponent<BulletTypes.ProjectileBullet>().projectile = projectile;
                                    projectile.transform.parent = target.transform;
                                    projectile.SetActive(false);
                                }
                                else
                                {
                                    Debug.LogError("Projectile round requires a projectile object. Please assign a projectile object above.", target);
                                }
                                break;
                            }
                    }
                }
                if (target.GetComponent<Rigidbody>() == null)
                {
                    target.AddComponent<Rigidbody>();
                    target.GetComponent<Rigidbody>().mass = 0.05f;
                }
                if (target.GetComponent<Collider>() == null)
                {
                    target.AddComponent<BoxCollider>();
                }
                if (isInteractable && target.GetComponent<VRTK_InteractableObject>() == null)
                {
                    target.AddComponent<VRTK_InteractableObject>();
                }
                if (isInteractable)
                {
                    target.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
                    target.GetComponent<VRTK_InteractableObject>().holdButtonToGrab = false;
                }
            }

            if (GUILayout.Button(new GUIContent("Set Up\nEmpty Shell", "Press this button to set up an empty bullet shell, for use in ejecting spent rounds.")))
            {
                if (target.GetComponent<Rigidbody>() == null)
                {
                    target.AddComponent<Rigidbody>();
                }
                if (target.GetComponent<Collider>() == null)
                {
                    target.AddComponent<BoxCollider>();
                }
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}