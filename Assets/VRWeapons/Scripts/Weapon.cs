using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRWeapons
{
    [RequireComponent(typeof(AudioSource))]

    [System.Serializable]
    public class Weapon : MonoBehaviour
    {
        public delegate void WeaponFiredEvent(Weapon thisWeap, IBulletBehavior roundFired);
        public delegate void WeaponStopFiringEvent(Weapon thisWeap);
        public delegate void WeaponPickedUpEvent(Weapon pickedUpWeapon);
        public delegate void WeaponDroppedEvent(Weapon droppedWeapon);
        public delegate void MagazineRemovedEvent(Weapon thisWeap, IMagazine droppedMag);
        public delegate void MagazineInsertedEvent(Weapon thisWeap, IMagazine newMag);
        public delegate void FireModeChangedEvent(int newFireMode);

        public event WeaponFiredEvent OnWeaponFired;
        public event WeaponStopFiringEvent OnWeaponStopFiring;
        public event WeaponPickedUpEvent OnWeaponPickedUp;
        public event WeaponDroppedEvent OnWeaponDropped;
        public event MagazineRemovedEvent OnMagRemoved;
        public event MagazineInsertedEvent OnMagInserted;
        public event FireModeChangedEvent OnFireModeChanged;

        IMuzzleActions Muzzle;
        IEjectorActions Ejector;
        IKickActions Kick;
        IBoltActions Bolt;
        IObjectPool shellPool;
        public IBulletBehavior chamberedRound;
        public IMagazine Magazine;
        
        public event VRWControl.TriggerHaptics shotHaptics;

        AudioSource audioSource;
        AudioClip soundToPlay;

        bool isFiring, justFired, stopFiring;
        int burstCount, currentFireMode;

        [HideInInspector]
        public bool secondHandGripped, isHeld;

        [HideInInspector]
        public GameObject holdingDevice, secondHandDevice;

        [HideInInspector]
        public List<Collider> colliderList;

        float nextFire;

        Vector3 triggerAngleStart, triggerPositionStart;

        [HideInInspector]
        public Transform grabPoint;

        //// Shown in inspector ////
        [Tooltip("Bolt will rack forward after racking backward, unless magazine is inserted and empty."), SerializeField]
        public bool autoRackForward;


        [Tooltip("Bolt moves back on firing. Disable for bolt/pump action weapons."), SerializeField]
        bool boltMovesOnFiring;

        public ImpactProfile impactProfile;

        [BitMask(typeof(FireMode)), SerializeField]
        FireMode fireMode;

        [Tooltip("Fire rate in seconds"), SerializeField]        
        float fireRate = 0.1f;

        [Tooltip("Only applies to burst fire mode."), SerializeField]
        int burstAmount = 3;

        [Tooltip("Trigger GameObject. Used to accurately rotate weapon's trigger on controller trigger pull."), SerializeField]
        Transform trigger;

        [Tooltip("End rotation of trigger, when fully pressed down.\n\nIf trigger does not rotate, leave this at (0, 0, 0)."), SerializeField]
        Vector3 triggerEndRotation;

        [Tooltip("End position of trigger, when fully pressed down.\n\nIf trigger does not change position, leave this at (0, 0, 0)."), SerializeField]
        Vector3 triggerEndPosition;

        [Tooltip("Main collider of the weapon, used for grabbing. Assign collider to disable it on pickup.\n\nIf this collider is not assigned, bolt manipulation " +
            "may not function correctly."), SerializeField]
        public Collider weaponBodyCollider;

        [Tooltip("Secondary collider of the weapon, used for second-hand grabbing. Collider will be disabled until weapon is picked up, then it will enable.\n\nIf " +
            "this collider is not assigned (on a two-handed weapon), physics may act strangely."), SerializeField]
        public Collider secondHandGripCollider;

        [Tooltip("Sound effect played when magazine is inserted."), SerializeField]
        AudioClip MagIn;

        [Tooltip("Sound effect played when magazine is removed."), SerializeField]
        AudioClip MagOut;

        [Tooltip("Sound effect played when bolt is moved back."), SerializeField]
        AudioClip SlideBack;

        [Tooltip("Sound effect played when bolt is moved forward."), SerializeField]
        AudioClip SlideForward;

        [Tooltip("Sound effect played when attempting to fire an empty weapon."), SerializeField]
        AudioClip DryFire;
        
        //// End shown in inspector ////

        [System.Serializable]
        public enum FireMode
        {
            Safe = (1 << 0),
            SemiAuto = (1 << 1),
            Automatic = (1 << 2),
            Burst = (1 << 3)
        }

        public enum AudioClips
        {
            MagIn = 0,
            MagOut = 1,
            SlideForward = 2,
            SlideBack = 3,
            DryFire = 4
        }
                
        [System.Serializable]
        public struct Attack
        {
            public float damage;
            public Vector3 origin;
            public RaycastHit hitInfo;
            public Weapon originWeapon;
        }

        private void Awake()
        {
            colliderList.Add(weaponBodyCollider);
            if (secondHandGripCollider != null)
            {
                colliderList.Add(secondHandGripCollider);
            }
        }

        private void Start()
        {
            Muzzle = GetComponentInChildren<IMuzzleActions>();
            Bolt = GetComponentInChildren<IBoltActions>();
            Ejector = GetComponentInChildren<IEjectorActions>();
            Kick = GetComponent<IKickActions>();
            shellPool = GetComponent<IObjectPool>();
            Magazine = GetComponentInChildren<IMagazine>();

            currentFireMode = GetStartingFireMode();

            if (OnFireModeChanged != null)
            {
                OnFireModeChanged.Invoke(currentFireMode);
            }

            if (Magazine != null)
            {
                MonoBehaviour mag = (MonoBehaviour)Magazine as MonoBehaviour;
                if (!mag.enabled)
                {
                    Magazine = null;
                    
                }
                else
                {
                    InsertMagazine(Magazine);
                    if (mag.GetComponent<Rigidbody>() != null && mag.gameObject != this.gameObject)
                    {
                        mag.GetComponent<Rigidbody>().isKinematic = true;
                    }
                    IgnoreCollision(mag.GetComponent<Collider>(), true);
                }
            }
            
            if (grabPoint == null)
            {
                grabPoint = transform.Find("Grab Point");
            }
            if (Bolt != null && Ejector != null)
            {
                Bolt.SetEjector(Ejector);
            }
            if (weaponBodyCollider != null)
            {
                IgnoreCollision(weaponBodyCollider, true);
            }
            
            if (secondHandGripCollider != null)
            {
                secondHandGripCollider.enabled = false;
            }

            audioSource = GetComponent<AudioSource>();
            if (trigger != null) {
                triggerAngleStart = trigger.localEulerAngles;
                triggerPositionStart = trigger.localPosition;
                if (triggerEndPosition == Vector3.zero)
                {
                    triggerEndPosition = triggerPositionStart;
                }
                if (triggerEndRotation == Vector3.zero)
                {
                    triggerEndRotation = triggerAngleStart;
                }
            }
        }

        int GetStartingFireMode()
        {
            int i = 0, j = 1;            

            if ((int)fireMode == 0 || (int)fireMode == 1)
            {
                Debug.Log("Weapon " + this + " does not have any fire modes capable of firing. Check fire mode flags.");
                return 1;
            }

            while (i == 0 && j < (int)fireMode)
            {
                i = j << 1 & (int)fireMode;
                j = j << 1;
            }
            return i;
        }

        public void SlideRelease()
        {
            if (Bolt != null)
            {
                Bolt.BoltRelease();
            }
        }

        public int GetRoundCount()
        {
            int count = 0;
            if (IsChambered())
            {
                count++;
            }
            if (Magazine != null)
            {
                count += Magazine.GetCurrentRoundCount();
            }
            return count;
        }

        public void IgnoreCollision(Collider collider, bool isIgnored = true)
        {
            if (collider == null)
            {
                Debug.LogWarning("Cannot ignore null collider");
                return;
            }
            
            foreach(Collider col in colliderList)
            {
                if (col != collider)
                {
                    Physics.IgnoreCollision(col, collider, isIgnored);
                }
            }

        }
                
        public void StartFiring(GameObject usingObject)
        {
            isFiring = true;
        }

        public void StopFiring(GameObject previousUsingObject)
        {
            if (OnWeaponStopFiring != null)
            {
                OnWeaponStopFiring.Invoke(this);
            }
            stopFiring = true;
            isFiring = false;
        }

        public void ChamberNewRound(IBulletBehavior round)
        {
            chamberedRound = round;
        }

        public void PlaySound(AudioClips clip)
        {
            switch (clip)
            {
                case AudioClips.MagIn:
                    soundToPlay = MagIn;
                    break;
                case AudioClips.MagOut:
                    soundToPlay = MagOut;
                    break;
                case AudioClips.SlideForward:
                    soundToPlay = SlideForward;
                    break;
                case AudioClips.SlideBack:
                    soundToPlay = SlideBack;
                    break;
                case AudioClips.DryFire:
                    soundToPlay = DryFire;
                    break;
            }
            if (soundToPlay != null && audioSource != null)
            {
                audioSource.pitch = Time.timeScale;
                audioSource.clip = soundToPlay;
                audioSource.Play();
            }

        }

        public void ChangeFireMode()
        {
            currentFireMode = currentFireMode << 1;

            int i = 0;

            if ((int)fireMode == 0 || (int)fireMode == 1)
            {
                Debug.Log("Weapon " + this + " does not have any fire modes capable of firing. Check fire mode flags.");
            }
            else
            {
                while ((currentFireMode & (int)fireMode) == 0 && i < 2)
                {
                    currentFireMode = currentFireMode << 1;
                    if (currentFireMode > (int)fireMode)
                    {
                        currentFireMode = 1;
                        i++;
                    }
                }
            }
            if (OnFireModeChanged != null)
            {
                OnFireModeChanged.Invoke(currentFireMode);
            }
        }        

        public bool IsLoaded()
        {
            bool ret = false;
            if (Magazine != null)
            {
                ret = true;
            }
            return ret;
        }

        public Attack NewAttack(float newDamage, Vector3 newOrigin, RaycastHit newHit)
        {
            return new Attack
            {
                damage = newDamage,
                origin = newOrigin,
                hitInfo = newHit
            };
        }

        private void FixedUpdate()
        {
            if (isFiring)
            {
                if (((FireMode)currentFireMode != FireMode.Safe && !justFired) || (FireMode)currentFireMode == FireMode.Automatic || ((FireMode)currentFireMode == FireMode.Burst && burstCount < burstAmount))
                {
                    if ((Time.time - nextFire >= fireRate) && IsChambered())
                    {
                        if (Muzzle != null)
                        {
                            Muzzle.StartFiring(chamberedRound);
                        }
                        else
                        {
                            Debug.LogError("No muzzle found on " + this.gameObject + ", please add one using the Weapon Builder.");
                        }

                        chamberedRound = null;
                        DoOnFireActions();
                        nextFire = Time.time;
                        justFired = true;
                        burstCount++;
                        if (shotHaptics != null)
                        {
                            shotHaptics.Invoke();
                        }
                        if (OnWeaponFired != null)
                        {
                            OnWeaponFired.Invoke(this, chamberedRound);
                        }
                    }
                    else if (!justFired && Time.time - nextFire >= fireRate)
                    {
                        PlaySound(AudioClips.DryFire);
                        nextFire = Time.time;
                        justFired = true;
                    }
                }
            }
            else if (stopFiring)
            {
                stopFiring = false;
                justFired = false;
                burstCount = 0;
                if (Muzzle != null)
                {
                    Muzzle.StopFiring();
                }
                else
                {
                    Debug.LogError("No muzzle found on " + this.gameObject + ", please add one using the Weapon Builder.");
                }
            }
        }

        #region Attachment-Friendly functions
        public void SetMuzzle(IMuzzleActions newMuzzle)
        {
            Muzzle = newMuzzle;
        }

        public void SetEjector(IEjectorActions newEjector)
        {
            Ejector = newEjector;
        }

        public bool IsChambered()
        {
            bool val = false;
            if (chamberedRound != null)
            {
                val = true;
            }
            return val;
        }

        #endregion

        void DoOnFireActions()
        {
            if (Bolt != null)
            {
                if (shellPool != null)
                {
                    GameObject go = shellPool.GetNewObj();
                    if (go != null)
                    {
                        Bolt.ReplaceRoundWithEmptyShell(go);
                    }
                }
                if (boltMovesOnFiring)
                {
                    Bolt.BoltBack();
                }
            }
            if (Kick != null)
            {
                Kick.Kick();
            }            
        }

        public void InsertMagazine(IMagazine newMag)
        {
            if (OnMagInserted != null)
            {
                OnMagInserted.Invoke(this, newMag);
            }
        }

        public void DropMagazine()
        {
            if (OnMagRemoved != null)
            {
                OnMagRemoved.Invoke(this, Magazine);
            }
            if (Magazine != null)
            {
                Magazine.MagOut(this);
            }
        }

        public void WeaponPickedUp()
        {
            if (OnWeaponPickedUp != null)
            {
                OnWeaponPickedUp.Invoke(this);
            }
        }

        public void WeaponDropped()
        {
            if (OnWeaponDropped != null)
            {
                OnWeaponDropped.Invoke(this);
            }
        }

        public void SetTriggerAngle(float angle)
        {
            if (trigger != null)
            {
                trigger.localRotation = Quaternion.Lerp(Quaternion.Euler(triggerAngleStart), Quaternion.Euler(triggerEndRotation), angle);
                trigger.localPosition = Vector3.Lerp(triggerPositionStart, triggerEndPosition, angle);
            }
            if (Bolt != null)
            {
                Bolt.OnTriggerPullActions(angle);
            }
        }

        public bool IsWeaponFiring()
        {
            return isFiring;
        }

        public void SetColliderEnabled(Collider col, bool isEnabled)
        {
            if (col != null)
            {
                col.enabled = isEnabled;
            }
        }
    }
    public class BitMaskAttribute : PropertyAttribute
    {
        public System.Type propType;
        public BitMaskAttribute(System.Type aType)
        {
            propType = aType;
        }
    }
}