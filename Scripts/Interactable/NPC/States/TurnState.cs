using System.Collections;
using Extensions.FSM;
using UnityEngine;

namespace Interactable.NPC.States
{
    public class TurnState : IState
    {
        private NPCReferenceManager _npcReferenceManager;
        private Animator _animator;
        private Transform _lookAtTransform;
        private Coroutine _turnCoroutine;
        private bool _hasTurnedToPlayer;
        private bool _isEvent;

        public TurnState(NPCReferenceManager npcReferenceManager, Transform LookAtTransform, bool isEvent = false)
        {
            _npcReferenceManager = npcReferenceManager;
            _animator = _npcReferenceManager.Animator;
            _lookAtTransform = LookAtTransform;
            _isEvent = isEvent;
        }
        
        public void OnEnter()
        {
            //TODO: Doesnt need to be a seperate MonoBehaviour
            //CAMERA SETUP
            _turnCoroutine = _npcReferenceManager.StartCoroutine(TurnToPlayer(_lookAtTransform));
            
            
            //Camera Setup is different on Turn to Player than on LookAtEvent, we use two Cameras for that
            if (!_isEvent)
            {
                _npcReferenceManager.SetupDialogueCamera(_lookAtTransform);
            }
            else
            {
                _npcReferenceManager.SetupEventCamera(_lookAtTransform);
            }
        }

        public void Tick()
        {

        }

        public void OnExit()
        {
            if (_turnCoroutine != null)
            {
                _npcReferenceManager.StopCoroutine(_turnCoroutine);
                _turnCoroutine = null;
            }

            _animator.SetBool(NPCReferenceManager.ANIM_TURN, false);
            _animator.SetFloat(NPCReferenceManager.ANIM_TURNSPEED, 0f);
            
            _hasTurnedToPlayer = false;
        }

        public Color GizmoState()
        {
            return new Color(0.1f, 0.6f, 0.1f);
        }
        private IEnumerator TurnToPlayer(Transform playerHeadTransform)
        {
            Vector3 playerPosition = new Vector3(playerHeadTransform.position.x, _npcReferenceManager.transform.position.y, playerHeadTransform.position.z);
            Vector3 directionToPlayer = (playerPosition - _npcReferenceManager.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            float angle = Vector3.SignedAngle(_npcReferenceManager.transform.forward, directionToPlayer, Vector3.up);
            float forceCorrectTime = Mathf.Abs(angle) / 180f * 2f;

            _animator.SetBool(NPCReferenceManager.ANIM_TURN, true);
            _animator.SetFloat(NPCReferenceManager.ANIM_TURNSPEED, angle);

            float rotationSpeed = 1.75f;
            float elapsedTime = 0f;

            while (Quaternion.Angle(_npcReferenceManager.transform.rotation, targetRotation) > 0.1f)
            {
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= forceCorrectTime)
                {
                    break;
                }

                _npcReferenceManager.transform.rotation = Quaternion.Slerp(_npcReferenceManager.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            _animator.SetBool(NPCReferenceManager.ANIM_TURN, false);
            _animator.SetFloat(NPCReferenceManager.ANIM_TURNSPEED, 0f);
            
            _hasTurnedToPlayer = true;
            _npcReferenceManager.Interact = false;
        }
        public bool HasFinishedTurning()
        {
            return _hasTurnedToPlayer;
        }
    }
}