using UnityEngine;
using System.Collections;
using RootMotion.Dynamics;

namespace RootMotion.Demos {

	public class RaycastShooter : MonoBehaviour {

		public LayerMask layers;
		public float unpin = 10f;
		public float force = 10f;
		public ParticleSystem blood;

        // Update is called once per frame
        void Update () {
			if (Input.GetMouseButtonDown(0)) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				// Raycast to find a ragdoll collider
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(ray, out hit, 100f, layers)) {
					var broadcaster = hit.collider.attachedRigidbody.GetComponent<MuscleCollisionBroadcaster>();

					if (broadcaster != null) {
						broadcaster.Hit(unpin, ray.direction * force, hit.point);
                        

						blood.transform.position = hit.point;
						blood.transform.rotation = Quaternion.LookRotation(-ray.direction);
						blood.Emit(5);
					}
				}
			}
		}
        private void OnTriggerEnter(Collider other)
        {
            // Check if the object is in the layer mask
            if (layers == (layers | (1 << other.gameObject.layer)))
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
            }
        }
    }
}
