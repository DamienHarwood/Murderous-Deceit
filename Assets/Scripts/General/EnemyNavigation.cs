using System;
using System.Collections;
using System.Collections.Generic;
using Damien;
using Knife.RealBlood;
using Knife.RealBlood.SimpleController;
using UnityEngine;
using Random = UnityEngine.Random;
using RileyMcGowan;

namespace RileyMcGowan
{
    public class EnemyNavigation : MonoBehaviour
    {
        /// <summary>
        /// Private Variables / Declaration
        /// </summary>
        private DelegateStateManager stateManager = new DelegateStateManager();
        private DelegateState standing = new DelegateState();
        private DelegateState walking = new DelegateState();
        private DelegateState death = new DelegateState();
        private float safeDistance = 1f;
        private GameObject player;
        private GameObject currentTarget;
        private Vector3 playerTransform;
        private Vector3 playerTransformNoHeight;
        private Vector3 currentTargetNoHeight;
        private Rigidbody rb;
        private float distanceToPlayer;
        private Ray ray;
        private Ray distanceRay;
        private FOV fovRef;
        private bool inSight;
        private Target[] targetList;
        private Health heathComp;
        private bool isDead;
        private bool isShooting;
        private float defaultSpeedModifier;
        public Rigidbody enemyRB;

        /// <summary>
        /// Public Variables / Declaration
        /// </summary>
        public LayerMask raycastLayerMask;
        public float enemySpeed;
        public GameObject bulletPrefab;
        public bool hasBeenShot;
        public bool canShoot;
        public enum ListOfTargets
        {
            level1Target,
            level2Target,
            level3Target
        }
        public ListOfTargets listOfTargets;
        
        /// <summary>
        /// Core Code
        /// </summary>
        private void Start()
        {
            if (isDead != true)
            {
                if (listOfTargets == ListOfTargets.level1Target)
                {
                    targetList = FindObjectsOfType<Target1>();
                }
                if (listOfTargets == ListOfTargets.level2Target)
                {
                    targetList = FindObjectsOfType<Target2>();
                }
                if (listOfTargets == ListOfTargets.level3Target)
                {
                    targetList = FindObjectsOfType<Target3>();
                }
            
                if (GetComponent<Rigidbody>() != null)
                {
                    rb = GetComponent<Rigidbody>();
                }
                else
                {
                    if (GetComponentInChildren<Rigidbody>() != null)
                    {
                        rb = GetComponentInChildren<Rigidbody>();
                    }
                    else
                    {
                        if (GetComponentInParent<Rigidbody>() != null)
                        {
                            rb = GetComponentInParent<Rigidbody>();
                        }
                        else
                        {
                            Debug.Log("No Rigidbody assigned to rb");
                            return;
                        }
                    }
                }
            
                if (GetComponent<FOV>() != null)
                {
                    fovRef = GetComponent<FOV>();
                }
                else
                {
                    if (GetComponentInChildren<FOV>() != null)
                    {
                        fovRef = GetComponentInChildren<FOV>();
                    }
                    else
                    {
                        if (GetComponentInParent<FOV>() != null)
                        {
                            fovRef = GetComponentInParent<FOV>();
                        }
                        else
                        {
                            Debug.Log("No FOV assigned to fovRef");
                            return;
                        }
                    }
                }
                if (GetComponent<Health>() != null)
                {
                    heathComp = GetComponent<Health>();
                    heathComp.deathEvent += Death;
                }
                else
                {
                    if (GetComponentInChildren<Health>() != null)
                    {
                        heathComp = GetComponentInChildren<Health>();
                        heathComp.deathEvent += Death;
                    }
                    else
                    {
                        if (GetComponentInParent<Health>() != null)
                        {
                            heathComp = GetComponentInParent<Health>();
                            heathComp.deathEvent += Death;
                        }
                        else
                        {
                            Debug.Log("No Health assigned to healthComp");
                            return;
                        }
                    }
                }
            

                //FindObjectsOfType<>();
                if (FindObjectOfType<PlayerController>() != null)
                {
                    player = FindObjectOfType<PlayerController>().gameObject;
                }
            
                standing.Enter = standingStart;
                standing.Update = standingUpdate;
                standing.Exit = standingExit;
                walking.Enter = walkingStart;
                walking.Update = walkingUpdate;
                walking.Exit = walkingExit;
                death.Enter = deathStart;
                death.Update = deathUpdate;
                death.Exit = deathExit;
                stateManager.ChangeState(standing);
            }
        }

        private void Update()
        {
            if (enemySpeed != null)
            {
                if (enemySpeed < 0)
                {
                    if (enemySpeed < -1)
                    {
                        defaultSpeedModifier = 0.01f + 0.05f;
                    }
                    else
                    {
                        defaultSpeedModifier = 0.01f + -enemySpeed/20;
                    }
                }
                else
                {
                    if (enemySpeed > 1)
                    {
                        defaultSpeedModifier = 0.01f + 0.05f;
                    }
                    else
                    {
                        defaultSpeedModifier = 0.01f + enemySpeed/20;
                    }
                }
            }
            
           // if(enemyRB.velocity != (0, 0, 0))
            
            if (player == null && FindObjectOfType<PlayerController>() != null)
            {
                player = FindObjectOfType<PlayerController>().gameObject;
            }
            if (listOfTargets != ListOfTargets.level1Target || hasBeenShot != false)
            {
                canShoot = true;
            }
            else
            {
                canShoot = false;
            }
            
            if (isDead != true)
            {
                stateManager.UpdateCurrentState();
                if (player != null)
                {
                    playerTransform = player.transform.position;
                    playerTransformNoHeight = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
                }
                if (playerTransform != null && fovRef != null)
                {
                    if (fovRef.listOfTargets.Count > 0)
                    {
                        inSight = true;
                    }

                    if (fovRef.listOfTargets.Count < 1)
                    {
                        inSight = false;
                    }
                }
            }
        }

        /// <summary>
        /// Standing State
        /// </summary>
        private void standingStart()
        {
            Debug.Log("Standing State");
        }

        private void standingUpdate()
        {
            if (isDead != false)
            {
                return;
            }
            if (inSight != false)
            {
                transform.LookAt(playerTransformNoHeight);
                if (canShoot != false && isShooting != true)
                {
                    float bulletDelay = 5;
                    float decayDelay = 2;
                    StartCoroutine(Shoot(bulletDelay, decayDelay));
                }
            }
            
            if (targetList != null && inSight != true)
            {
                stateManager.ChangeState(walking);
            }
        }

        private void standingExit()
        {

        }

        /// <summary>
        /// Walking State
        /// </summary>
        private void walkingStart()
        {
            Debug.Log("Walking State");
            currentTarget = targetList[Random.Range(0, targetList.Length)].gameObject;
            currentTargetNoHeight = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
        }

        private void walkingUpdate()
        {
            if (isDead != false)
            {
                return;
            }
            if (fovRef.listOfTargets.Count < 1)
            {
                if (inSight == true)
                {
                    rb.isKinematic = true;
                    stateManager.ChangeState(standing);
                }
                else
                {
                    transform.LookAt(currentTargetNoHeight);
                    transform.position += transform.forward * defaultSpeedModifier;
                }

                if (Vector3.Distance(transform.position, currentTargetNoHeight) < safeDistance && inSight == false)
                {
                    rb.isKinematic = true;
                    currentTarget = targetList[Random.Range(0, targetList.Length)].gameObject;
                    currentTargetNoHeight = new Vector3(currentTarget.transform.position.x, transform.position.y, currentTarget.transform.position.z);
                }
            }
            else
            {
                stateManager.ChangeState(standing);
            }
        }

        private void walkingExit()
        {

        }

        /// <summary>
        /// Death State
        /// </summary>
        private void deathStart()
        {
            Debug.Log("Death State");
            isDead = true;
            //Death event called here
        }

        private void deathUpdate()
        {
            //Death event called here
        }

        private void deathExit()
        {

        }


        /// <summary>
        /// Function Calls
        /// </summary>
        private void Death(Health health)
        {
            stateManager.ChangeState(death);
        }

        IEnumerator Shoot(float shotDelay, float destroyDelay)
        {
            isShooting = true;
            var instance = Instantiate(bulletPrefab, transform.position, transform.rotation);
            Destroy(instance, destroyDelay);
            yield return new WaitForSeconds(shotDelay);
            isShooting = false;
        }

        
        /// <summary>
        /// Disable Junk
        /// </summary>
        private void OnDestroy()
        {
            heathComp.deathEvent -= Death;
        }

        private void OnDisable()
        {
            heathComp.deathEvent -= Death;
        }
    }
}
