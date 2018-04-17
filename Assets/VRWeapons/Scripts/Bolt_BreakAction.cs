using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRWeapons
{
    [RequireComponent(typeof(HingeJoint))]

    /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
     *                                                                           *
     *      Break action script is complete, HOWEVER: Builder is not complete.   *
     *      Building a break-action weapon turned out to be more complicated     *
     *      than I expected, so I'm still working on a way to use the builder    *
     *      to set one up. In the meantime, if you need a break-action weapon    *
     *      set up, send me an email at Slayd7@gmail.com and I can help out.     *
     *                                                                           *
     * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

    public class Bolt_BreakAction : MonoBehaviour, IBoltActions
    {
        float lerpVal;

        float maxVal, minVal, previousHingeMax;

        bool fullyClosed = true, needsToLock = true, isCurrentlyOpening, isBeingManipulated;

        IMagazine[] chambers;
        Transform[] roundTransforms;
        Rigidbody[] roundRigidBodies;
        Collider[] roundColliders;

        [SerializeField]
        GameObject[] MuzzleObjects;

        IMuzzleActions[] muzzles;

        int chamberIndex, chamberCount;

        Rigidbody thisRB;

        Weapon thisWeap;
        HingeJoint hinge;

        JointLimits lockedJLim, unlockedJLim;

        IEjectorActions ejector;

        private void Start()
        {
            thisRB = GetComponent<Rigidbody>();
            thisWeap = GetComponentInParent<Weapon>();
            hinge = GetComponent<HingeJoint>();
            ejector = thisWeap.gameObject.GetComponentInChildren<IEjectorActions>();


            unlockedJLim = hinge.limits;
            lockedJLim = new JointLimits();     // Setting up "locked" limit, for when chambers are closed.
            lockedJLim.max = 0;

            previousHingeMax = hinge.limits.max;

            maxVal = hinge.limits.max + transform.localEulerAngles.x;
            minVal = 0 + transform.localEulerAngles.x;            
            
            SetChambers();              // Gets a count of the chambers and gets indices set up
        }        

        void SetChambers()
        {
            chamberCount = 0;
            foreach(IMagazine mag in GetComponentsInChildren<IMagazine>())
            {
                chamberCount++;
            }
            chambers = new IMagazine[chamberCount];
            roundTransforms = new Transform[chamberCount];
            roundRigidBodies = new Rigidbody[chamberCount];
            roundColliders = new Collider[chamberCount];

            chamberCount = 0;
            
            foreach (IMagazine mag in GetComponentsInChildren<IMagazine>())
            {
                chambers[chamberCount] = mag;
                chamberCount++;
            }


            int i = 0;
            foreach(GameObject obj in MuzzleObjects)
            {
                i++;
            }

            muzzles = new IMuzzleActions[i];
            i = 0;
            foreach(GameObject obj in MuzzleObjects)
            {
                muzzles[i] = obj.GetComponent<IMuzzleActions>();
                i++;
            }

            thisWeap.SetMuzzle(muzzles[0]);
            StartCoroutine(LoadFirstRound());
        }

        IEnumerator LoadFirstRound()
        {
            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < chamberCount; i++)
            {
                roundTransforms[i] = chambers[i].GetRoundTransform();
                roundRigidBodies[i] = chambers[i].GetRoundRigidBody();
                if (roundColliders[i] != null && roundTransforms[i].GetComponent<Collider>() != null)
                {
                    roundColliders[i] = roundTransforms[i].GetComponent<Collider>();
                }
            }

            foreach (Collider col in roundColliders)
            {
                foreach (Collider otherCol in roundColliders)
                {
                    if (col != null && otherCol != null)
                    {
                        Physics.IgnoreCollision(col, otherCol);
                    }
                }
            }

            thisWeap.ChamberNewRound(ChamberNewRound());


        }

        private void FixedUpdate()
        {
            if (!fullyClosed && !isBeingManipulated)
            {
                float oldLerpVal = lerpVal;
                lerpVal = Mathf.InverseLerp(minVal, maxVal, transform.localEulerAngles.x);

                if (isCurrentlyOpening && lerpVal > 0.95f)
                {
                    lerpVal = 1;
                    isCurrentlyOpening = false;
                    DoOnOpenActions();
                }
                else if (!isCurrentlyOpening && lerpVal < 0.05f)
                {
                    lerpVal = 0;
                    fullyClosed = true;
                    DoOnClosedActions();
                }
                else if (thisWeap.IsChambered())
                {
                    thisWeap.ChamberNewRound(null);
                }

                if (lerpVal != oldLerpVal)
                {
                    transform.localEulerAngles = new Vector3(Mathf.Lerp(minVal, maxVal, lerpVal), transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
            }
            else
            {
                if (needsToLock && !isBeingManipulated)
                {
                    SetHingeLocked(hinge);
                }
                if (isBeingManipulated)
                {
                    transform.localEulerAngles = new Vector3(Mathf.Lerp(minVal, maxVal, lerpVal), transform.localEulerAngles.y, transform.localEulerAngles.z);
                    if (isCurrentlyOpening && lerpVal > 0.95f)
                    {
                        lerpVal = 1;
                        isCurrentlyOpening = false;
                        DoOnOpenActions();
                    }
                    else if (lerpVal < 0.05f)
                    {
                        lerpVal = 0;
                        fullyClosed = true;
                        DoOnClosedActions();
                    }
                }
            }
        }
        
        void SetHingeLocked(HingeJoint hj, bool locked = true)
        {
            if (locked)
            {
                hj.limits = lockedJLim;
                needsToLock = false;
            }
            else
            {
                hj.limits = unlockedJLim;
                needsToLock = true;
            }
        }

        void DoOnClosedActions()
        {
            int i = SetCurrentChamber(true);

            if (i != -1)
            {
                chamberIndex = i;
                for (int j = 0; j < chamberCount; j++)
                {
                    if (chambers[j].GetRoundTransform() != null)
                    {
                        roundTransforms[j] = chambers[j].GetRoundTransform();
                        roundRigidBodies[j] = chambers[j].GetRoundRigidBody();
                        roundColliders[j] = roundTransforms[j].GetComponent<Collider>();
                    }
                }

                foreach(Collider col in roundColliders)
                {
                    foreach(Collider otherCol in roundColliders)
                    {
                        if (col != null && otherCol != null)
                        {
                            Physics.IgnoreCollision(col, otherCol);
                        }
                    }
                }

                thisWeap.ChamberNewRound(ChamberNewRound());
            }
            else
            {
                thisWeap.ChamberNewRound(null);
            }

            thisWeap.PlaySound(Weapon.AudioClips.SlideForward);

            isCurrentlyOpening = false;

            hinge.useSpring = true;
        }

        void DoOnOpenActions()
        {
            if (ejector != null)
            {
                for (int i = 0; i < chamberCount; i++)
                {
                    if (roundTransforms[i] != null)
                    {
                        ejector.Eject(roundTransforms[i], roundRigidBodies[i]);
                    }

                    chambers[i].PopBullet();

                    roundTransforms[i] = null;
                    roundRigidBodies[i] = null;
                }
            }
            thisWeap.ChamberNewRound(null);

            hinge.useSpring = false;
        }

        int SetCurrentChamber(bool reset = false)
        {
            int i = 0;
            if (reset)
            {
                return i;
            }

            else if (chamberCount > 0)
            {
                if (chamberIndex < chamberCount)
                {
                    i = chamberIndex + 1;
                    return i;
                }
            }
            return -1;
        }

        public IBulletBehavior ChamberNewRound()
        {
            if (chamberIndex < chamberCount && chamberIndex >= 0)
            {
                thisWeap.SetMuzzle(muzzles[chamberIndex]);
                IBulletBehavior tmp = chambers[chamberIndex].FeedRound();                
                return tmp;
            }
            
            return null;
        }

        public void BoltBack()
        {
            /* Nothing to do here, for this bolt type */
        }

        public void OnTriggerPullActions(float angle)
        {
            /* Nothing to do here, for this bolt type */
        }

        public void SetEjector (IEjectorActions newEjector)
        {
            ejector = newEjector;
        }

        public void BoltRelease()
        {
            if (!isCurrentlyOpening)
            {
                thisWeap.PlaySound(Weapon.AudioClips.SlideBack);
            }
            fullyClosed = false;
            isCurrentlyOpening = true;
            SetHingeLocked(hinge, false);
        }

        public void IsCurrentlyBeingManipulated(bool val)
        {
            bool tmp = false;
            if (val)
            {
                SetHingeLocked(hinge, false);
            }
            needsToLock = false;

            if (!tmp && val)
            {
                tmp = true;
                isCurrentlyOpening = true;
            }
            else if (tmp && !val)
            {
                tmp = false;
            }

            if (!val && lerpVal < 0.05f)
            {
                needsToLock = true;
            }
            thisRB.isKinematic = val;
            isBeingManipulated = val;
        }

        public void ReplaceRoundWithEmptyShell(GameObject go)
        {
            Vector3 pos = roundTransforms[chamberIndex].position;
            Vector3 rot = roundTransforms[chamberIndex].eulerAngles;

            go.transform.position = pos;
            go.transform.eulerAngles = rot;

            go.transform.parent = transform;

            Destroy(roundTransforms[chamberIndex].gameObject);

            roundTransforms[chamberIndex] = go.transform;
            roundRigidBodies[chamberIndex] = go.GetComponent<Rigidbody>();

            roundRigidBodies[chamberIndex].isKinematic = true;
            
            chamberIndex = SetCurrentChamber();

            if (chamberIndex != -1)
            {
                thisWeap.ChamberNewRound(ChamberNewRound());
            }
        }

        public Vector3 GetMinValue()
        {
            return Vector3.zero;
        }

        public Vector3 GetMaxValue()
        {
            return Vector3.zero;
        }

        public float boltLerpVal { set { lerpVal = value; } get { return lerpVal; } }
    }
}