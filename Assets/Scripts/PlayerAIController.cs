using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TinyWar
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerAIController : MonoBehaviour
    {
        private Animator animator;

        private NavMeshAgent agent;
        private Transform tr;

        public Transform target;

        public float stopDistance = .5f;

        private PlayerController playerController;

        private void OnEnable()
        {
            agent = GetComponent<NavMeshAgent>();
            tr = GetComponent<Transform>();
            animator = GetComponentInChildren<Animator>();
            playerController = GetComponentInChildren<PlayerController>();
        }

     

        void Update()
        {

            if(target != null) 
            {
                agent.destination = target.position;
                agent.isStopped = false;
            }


            // Calculate the velocity relative to this transform's orientation
            Vector3 relVelocity = tr.InverseTransformDirection(agent.velocity);
            relVelocity.y = 0;

            // Speed relative to the character size
            animator.SetFloat("NormalizedSpeed", relVelocity.magnitude / animator.transform.lossyScale.x);


        }

        public void StopAgent()
        {
            target = null;
            agent.isStopped = true;
        }

     
    }
}
