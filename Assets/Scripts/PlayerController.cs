using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyWar
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float playerHealth = 100;
        [SerializeField] private float playerAttackDamage = 10;
        [SerializeField] private Team teamType;
        private Animator animator;
        private CharacterController characterController;
        private PlayerAIController playerAIController;
        private PlayerUIManager playerUIManager;
        private float minAttackDistance = 1;
        private float updateLoopWaitTime = .2f;
        public string playerName;
        public Action<PlayerController,PlayerController> hitTaken;
        public Action<PlayerController,PlayerController> playerDied;
        public PlayerController currFightingEnemy;
        public bool isAttacking;
        public bool isDead;

        public float PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; }
        }

        public Team TeamType
        {
            get { return teamType; }
            set { teamType = value; }
        }

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            characterController = GetComponent<CharacterController>();
            playerAIController = GetComponent<PlayerAIController>();
            playerUIManager = GetComponent<PlayerUIManager>();
        }

        void OnEnable()
        {
            hitTaken += HitTakenHandler;
            playerDied += PlayerDiedHandler;
            StartCoroutine(UpdateCoro());
        }


        // Update is called once per frame
        IEnumerator UpdateCoro()
        {
            while (true)
            {
                if (!isDead)
                {
                    if (!isAttacking)
                    {
                        SearchForEnemy();
                    }
                    else
                    {
                        if (currFightingEnemy != null)
                        {
                            if (!currFightingEnemy.isDead)
                                transform.forward = currFightingEnemy.transform.position - transform.position;
                            else
                            {
                                currFightingEnemy = null;
                                StopFighting();
                            }

                        }
                    }
                }
                yield return new WaitForSeconds(updateLoopWaitTime);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.name == "w_sword")
            {
                PlayerController attackingPlayerController = other.GetComponentInParent<PlayerController>();
                if (attackingPlayerController != null)
                {
                    if(attackingPlayerController.teamType != this.teamType && attackingPlayerController.currFightingEnemy == this)
                    {
                        hitTaken?.Invoke(this,attackingPlayerController);
                    }
                }
            }           
        }

        private void SearchForEnemy()
        {
            PlayerController nearestEnemy = null;
            float nearestEnemyDist = float.MaxValue;
            foreach (PlayerController player in  TinyWarManager.Instance.ActivePlayers)
            {
                if(player.teamType != teamType && !player.isDead)
                {
                    float playerDist = Vector3.Distance(player.transform.position, transform.position);
                    if (playerDist < nearestEnemyDist)
                    {
                        nearestEnemyDist = playerDist;
                        nearestEnemy = player;
                    }
                }
            }
            if(nearestEnemy != null)
            {

                playerAIController.target = nearestEnemy.transform;
                currFightingEnemy = nearestEnemy;
                if(nearestEnemyDist<minAttackDistance)
                {
                    playerAIController.StopAgent();
                    StartFighting();
                }
            }
        }

        private void StartFighting()
        {
            isAttacking = true;
            animator.SetLayerWeight(1, 1);
        }

        private void StopFighting()
        {
            isAttacking = false;
            animator.SetLayerWeight(1, 0);
        }

        private void PlayDead()
        {
            animator.SetTrigger("PlayerDead");
        }

        private void HitTakenHandler(PlayerController hitTakenPlayer,PlayerController hitDealtPlayer)
        {
            playerHealth -= playerAttackDamage;
            playerUIManager.UpdateHealthBar(playerHealth);
            if(playerHealth <= 0)
                playerDied?.Invoke(hitTakenPlayer,hitDealtPlayer);
        }

        private void PlayerDiedHandler(PlayerController diedPlayer,PlayerController killedPlayer)
        {
            hitTaken -= HitTakenHandler;
            playerAIController.target = null;
            playerUIManager.DestroyHealthBar();
            isDead = true;
            StopFighting();
            PlayDead();
            characterController.enabled = false;
            Destroy(gameObject, 5);
        }

        private void OnDisable()
        {
            hitTaken = null;
            playerDied = null;
            StopAllCoroutines();
        }

    }

    public enum Team {Ally,Enemy} 
}