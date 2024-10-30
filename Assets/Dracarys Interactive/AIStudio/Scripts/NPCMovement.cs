using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace DracarysInteractive.AIStudio
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(DialogueCharacter))]
    public class NPCMovement : MonoBehaviour
    {
        [SerializeField] private Transform _destination;
        private NavMeshAgent _agent;
        private DialogueCharacter _character;
        private bool _walkingAnimationTriggered = false;

        public UnityEvent<bool> OnMovement;

        public Transform Destination
        {
            get { return _destination; }

            set
            {
                if (_destination != value)
                {
                    transform.LookAt(_destination = value);
                    _agent.SetDestination(_destination.position);
                }
            }
        }

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _character = GetComponent<DialogueCharacter>();
        }

        private void Update()
        {
            if (_destination != null && !_agent.pathPending)
            {
                if (_agent.remainingDistance <= _agent.stoppingDistance)
                {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                    {
                        OnMovement.Invoke(_walkingAnimationTriggered = false);
                        _destination = null;
                    }
                } else if (!_walkingAnimationTriggered)
                {
                    OnMovement.Invoke(_walkingAnimationTriggered = true);
                }
            }
        }
    }
}