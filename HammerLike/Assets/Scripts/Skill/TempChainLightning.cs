using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempChainLightning : MonoBehaviour
{
    public GameObject SparkBall;
    public GameObject Lightning;
    bool alreadyLightninbg = false;
    IEnumerator DoLightning()
    {
        Vector3 startLocation = transform.forward;
        Quaternion quater = transform.rotation * Quaternion.Euler(90, 0, 0);

        GameObject attack = Instantiate(Lightning, startLocation, quater);

        Vector3 previousLocation = Vector3.zero;
        {
            Plane playerPlan = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitdist;

            if (playerPlan.Raycast(ray, out hitdist))
            {
                Vector3 targetPoint = ray.GetPoint(hitdist);

                float distance = Vector3.Distance(targetPoint, startLocation);

                Vector3 angleOfNextHit = targetPoint - startLocation;
                attack.transform.rotation = Quaternion.LookRotation(angleOfNextHit, Vector3.up) * Quaternion.Euler(90, 0, 0);

                attack.transform.position = attack.transform.position + ((targetPoint - attack.transform.position) / 2);
                attack.transform.localScale = new Vector3(attack.transform.localScale.x, distance, attack.transform.localScale.z);

                GameObject.Destroy(attack, 1);
                previousLocation = targetPoint;
                GameObject.Destroy(Instantiate(SparkBall, targetPoint, quater), 3);
            }
        }
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < 5; ++i)
        {
            GameObject attack2 = Instantiate(Lightning, previousLocation, quater);

            Plane playerPlan = new Plane(Vector3.up, transform.position);
            Vector3 targetPoint = new Vector3(Random.Range(-15, 15), 3, Random.Range(-15,15));
            float distance = Vector3.Distance(targetPoint, previousLocation);

            Vector3 angleOfNextHit = targetPoint - previousLocation;
            attack2.transform.rotation = Quaternion.LookRotation(angleOfNextHit, Vector3.up) * Quaternion.Euler(90, 0, 0);
            attack2.transform.position = attack2.transform.position + ((targetPoint - attack2.transform.position) / 2);
            attack2.transform.localScale = new Vector3(attack2.transform.localScale.x, distance, attack2.transform.localScale.z);

            GameObject.Destroy(Instantiate(SparkBall, targetPoint, quater), 3);
            GameObject.Destroy(attack2, 1);
            previousLocation = targetPoint;
            yield return new WaitForSeconds(.2f);
        }

        
    }

    void WaitForLightning()
    {
        alreadyLightninbg = false;
    }
    private void FixedUpdate()
    {
        if(Input.GetMouseButtonDown(0)&& !alreadyLightninbg)
        {
            alreadyLightninbg = true;
            Invoke("WaitForLightning", 2);
            StartCoroutine(DoLightning());
        }
    }
}
