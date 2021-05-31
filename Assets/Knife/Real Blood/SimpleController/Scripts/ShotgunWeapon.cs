using System;
using System.Collections;
using System.Collections.Generic;
using RileyMcGowan;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Knife.RealBlood.SimpleController
{
    /// <summary>
    /// Shotgun weapon
    /// </summary>
    public class ShotgunWeapon : Weapon
    {
        public int Bullets = 10;
        public float RandomAngle = 5f;
        private Health damageRef;
        private LimbHealth limbRef;
        public float Damage = 2f;

        Dictionary<IHittable, List<DamageData>> damages = new Dictionary<IHittable, List<DamageData>>();
        //Dictionary<IDamageable, List<Damage>> damages2 = new Dictionary<IDamageable, List<Damage>>();
        private void FixedUpdate()
        {
            if (damageRef != null)
            {
                damageRef.DoDamage(Damage, Health.DamageType.Normal);
                damageRef = null;
            }
            if (limbRef != null)
            {
                limbRef.DoDamage(Damage, LimbHealth.DamageType.Normal);
                limbRef = null;
            }
        }

        protected override void Shot()
        {

            handsAnimator.Play("Shot", 0, 0);
            shootSound.Play();
            damages.Clear();
            //damages2.Clear();
            for (int i = 0; i < Bullets; i++)
            {
                Vector3 direction = Camera.main.transform.forward;
                direction = Quaternion.AngleAxis(Random.Range(-RandomAngle, RandomAngle), Camera.main.transform.up) * direction;
                direction = Quaternion.AngleAxis(Random.Range(-RandomAngle, RandomAngle), Camera.main.transform.right) * direction;
                //Debug.DrawLine(Camera.main.transform.position, Camera.main.transform.position + direction * 155f, Color.red, 5f);

                Ray r = new Ray(Camera.main.transform.position, direction);
                RaycastHit hitInfo;

                if (Physics.Raycast(r, out hitInfo, 1000, ShotMask, QueryTriggerInteraction.Ignore))
                {

                    var hittable = hitInfo.collider.GetComponent<IHittable>();
                    if (hitInfo.collider.GetComponentInParent<Health>() != null)
                    {
                        damageRef = hitInfo.collider.GetComponentInParent<Health>();
                    }
                    else
                    {
                        if (hitInfo.collider.GetComponent<Health>() != null)
                        {
                            damageRef = hitInfo.collider.GetComponent<Health>();
                        }
                    }
                    if (hitInfo.collider.GetComponent<LimbHealth>() != null)
                    {
                        limbRef = hitInfo.collider.GetComponent<LimbHealth>();
                    }
                    else
                    {
                        if (hitInfo.collider.GetComponentInChildren<LimbHealth>() != null)
                        {
                            limbRef = hitInfo.collider.GetComponentInChildren<LimbHealth>();
                        }
                        else
                        {
                            if (hitInfo.collider.GetComponentInParent<LimbHealth>() != null)
                            {
                                limbRef = hitInfo.collider.GetComponentInParent<LimbHealth>();
                            }
                        }
                    }
                    if (hittable != null)
                    {
                        if (hitInfo.collider.GetComponent<Rigidbody>() != null)
                        {
                            Rigidbody rb = hitInfo.collider.GetComponent<Rigidbody>();
                            rb.AddRelativeForce(-Vector3.forward * 40);
                        }
                        
                        DamageData damage = new DamageData()
                        {
                            amount = Damage,
                            direction = r.direction,
                            normal = hitInfo.normal,
                            point = hitInfo.point
                        };

                        List<DamageData> damageDatasPerHittable;
                        if (!damages.TryGetValue(hittable, out damageDatasPerHittable))
                        {
                            damageDatasPerHittable = new List<DamageData>();
                            damages.Add(hittable, damageDatasPerHittable);
                        }
                        damageDatasPerHittable.Add(damage);
                        
                    }

                }

                DebugShot(r, hitInfo);
            }

            foreach (var kv in damages)
            {
                kv.Key.TakeDamage(kv.Value.ToArray());
            }

            /* you should not call take damage with that method

            foreach (var kv in damages2)
            {
                kv.Key.StartTakeDamage();

                foreach(var d in kv.Value)
                {
                    kv.Key.TakeDamage(d);
                }

                kv.Key.EndTakeDamage();
            }
            */

            // you should use DamageHelper
            //DamageHelper.TakeDamage(damages2);
        }
    }
}