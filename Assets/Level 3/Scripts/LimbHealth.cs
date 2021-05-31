using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Namespace use cause why not keep it out of the way of other components
namespace RileyMcGowan
{
    public class LimbHealth : MonoBehaviour
    {
        [Tooltip("Starting Health between 1-100 is reasonable.")]
        public float startingHealth;

        [Tooltip("Expected to be the same as startingHealth on start. If no value set default 100.")]
        public float maxHealth;
        
        [Tooltip("This is current health, do not edit unless for testing.")]
        public float currentHealth;
        
        [Tooltip("Toggle on to make object invincible (not die).")]
        public bool invincible;

        private Health parentHealthComp;
        private EnemyNavigation parentEnemyNav;
        
        public enum DamageType
        {
            Normal,
            Poison,
            Energy
        }
        
        void Start()
        {
            ResetHealth();
            if (GetComponentInParent<Health>() != null)
            {
                parentHealthComp = GetComponentInParent<Health>();
                if (parentHealthComp.GetComponent<EnemyNavigation>() != null)
                {
                    parentEnemyNav = parentHealthComp.gameObject.GetComponent<EnemyNavigation>();
                }
            }
            else
            {
                Debug.Log("parentHealthComp Cannot find reference of health in parent");
            }
        }

        void FixedUpdate()
        {
            //ENABLE / DISABLE
            if (parentEnemyNav != null)
            {
                if (parentEnemyNav.listOfTargets != EnemyNavigation.ListOfTargets.level3Target)
                {
                    enabled = false;
                }
                else
                {
                    enabled = true;
                }
            }
            
            //If we take damage that makes the objects health 0 then invoke death
            if (currentHealth <= 0 && invincible != true)
            {
                currentHealth = 0;
                DestroyObject();
            }
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        
        /// DO DAMAGE AND HEALING ///
        //DoDamage function to record damage then apply it
        public void DoDamage(float damageDealt, DamageType damageType)
        {
            if (invincible != true)
            {
                currentHealth -= damageDealt;
                parentHealthComp.DoDamage((-damageDealt/4)*3, Health.DamageType.Normal);
            }
        }
        
        public void ResetHealth()
        {
            if (maxHealth <= 0)
            {
                maxHealth = 100;
            }
            if (startingHealth == 0)
            {
                startingHealth = 100;
            }
            currentHealth = startingHealth;
        }
        //Separated death function for editor
        public void DestroyObject()
        {
            ////////////////////BREAK LIMB////////////////////
            Destroy(gameObject);
            ////////////////////BREAK LIMB////////////////////
        }
    }
}