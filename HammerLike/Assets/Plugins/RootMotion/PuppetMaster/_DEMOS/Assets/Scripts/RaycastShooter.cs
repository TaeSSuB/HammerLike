using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;
using System.Threading;

namespace RootMotion.Demos {

    public class RaycastShooter : MonoBehaviour
    {

        public LayerMask layers;
        public float unpin = 10f;
        public float force = 10f;
        //public ParticleSystem blood;
        public GameObject[] targetBones;
        public int indexRay=0;
        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                // Raycast to find a ragdoll collider
                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, 100f, layers))
                {
                    var broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

                    if (broadcaster != null)
                    {
                        broadcaster.Hit(unpin, ray.direction * force, hit.point);


                        //blood.transform.position = hit.point;
                        //blood.transform.rotation = Quaternion.LookRotation(-ray.direction);
                        //blood.Emit(5);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (targetBones.Length > 0)
                {
                    ShootAtBone(targetBones[0]); // Example: Shoot at the first bone in the list
                }
            }
            else if (Input.GetKeyDown(KeyCode.H))
            {
                if (targetBones.Length > 1)
                {
                    ShootAtBone(targetBones[1]); // Example: Shoot at the first bone in the list
                }
            }
            else if (Input.GetKeyDown(KeyCode.K))
            {
                if (targetBones.Length > 2)
                {
                    ShootAtBone(targetBones[2]); // Example: Shoot at the first bone in the list
                }
            }

        }
        private void OnTriggerEnter(Collider other)
        {
            // Check if the object is in the layer mask
            /*if (layers == (layers | (1 << other.gameObject.layer)))
            {
                var broadcaster = other.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

                if (broadcaster != null)
                {
                    Vector3 hitDirection = (other.transform.position - transform.position).normalized;
                    broadcaster.Hit(unpin, hitDirection * force, other.ClosestPoint(transform.position));

                    blood.transform.position = other.ClosestPoint(transform.position);
                    blood.transform.rotation = Quaternion.LookRotation(-hitDirection);
                    blood.Emit(5);
                }
            }*/
            //ShootAtBone(targetBones[0]);
        }

        public void ShootAtBone(GameObject targetBone)
        {
            
                Vector3 direction = (targetBone.transform.position - transform.position).normalized;
                Ray ray = new Ray(transform.position, direction);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f, layers))
                {
                    var broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

                    if (broadcaster != null)
                    {
                        broadcaster.Hit(unpin, direction * force, hit.point);
                        indexRay++;
                        //blood.transform.position = hit.point;
                        //blood.transform.rotation = Quaternion.LookRotation(-direction);
                        //blood.Emit(5);
                    }
                }
            
        }
        public void ShootAtBoneWithForce(GameObject targetBone, float customForce)
        {

            Vector3 direction = (targetBone.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, layers))
            {
                var broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

                if (broadcaster != null)
                {
                    broadcaster.Hit(unpin, direction * customForce, hit.point);
                    indexRay++;
                    //blood.transform.position = hit.point;
                    //blood.transform.rotation = Quaternion.LookRotation(-direction);
                    //blood.Emit(5);
                }
            }

        }
        public void IndexRay()
        {
            Debug.Log(indexRay);
        }



    }
}
