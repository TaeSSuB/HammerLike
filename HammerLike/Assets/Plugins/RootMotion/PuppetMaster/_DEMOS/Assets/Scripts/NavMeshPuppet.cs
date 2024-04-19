using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos
{
    public class NavMeshPuppet : MonoBehaviour
    {
        public BehaviourPuppet puppet;
        public UnityEngine.AI.NavMeshAgent agent;
        public Transform target;
        public Animator animator;
        public bool isActive=true;
        private void Start()
        {
            if (target == null)
            {
                //GameObject player = GameObject.Find("Player"); // "Player"라는 이름을 가진 GameObject를 찾습니다.

                GameObject player = GameObject.FindWithTag("Player");

                if (player != null) // GameObject가 존재하는지 확인합니다.
                {
                    target = player.transform; // 찾은 GameObject의 Transform 컴포넌트를 target에 할당합니다.
                }
                else
                {
                    Debug.LogError("Player 오브젝트를 찾을 수 없습니다. 'Player'라는 이름의 오브젝트가 씬에 존재하는지 확인해주세요.");
                }
            }
        }

        void Update()
        {
            // Keep the agent disabled while the puppet is unbalanced.
            agent.enabled = puppet.state == BehaviourPuppet.State.Puppet;

            // Update agent destination and Animator
            if (agent.enabled)
            {
                agent.SetDestination(target.position);

                animator.SetFloat("Forward", agent.velocity.magnitude * 0.25f);
            }
        }
    }
}
