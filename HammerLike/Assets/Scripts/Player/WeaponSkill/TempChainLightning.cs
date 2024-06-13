using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TempChainLightning : MonoBehaviour, ISkill
{
    public GameObject SparkBall;
    public GameObject Lightning;
    bool alreadyLightning = false;
    List<GameObject> hitMonsters = new List<GameObject>();
    public GameObject Thunder;
    private VisualEffect visualEffect;
    public float maxDistance = 10f;

    public void ChargeSkill(Vector3 position, Transform parent, SO_Skill skillData)
    {
        /*this.transform.position = position;
        StartCoroutine(DoLightning());*/
        ChargeAttack();
    }

    private void Start()
    {
        visualEffect = Thunder.GetComponent<VisualEffect>();
    }

    

    private void ChargeAttack()
    {
        Vector3 thunderPosition = GetMouseWorldPosition();
        thunderPosition = ClampPositionToMaxDistance(thunderPosition);
        thunderPosition.y += 5; // y 좌표를 5만큼 증가시킴

        GameObject thunderInstance = Instantiate(Thunder, thunderPosition, Quaternion.identity);
        visualEffect = thunderInstance.GetComponent<VisualEffect>();
        visualEffect.Play();
        Destroy(thunderInstance, 3); // Thunder 오브젝트를 3초 후에 제거
    }


    private Vector3 GetMouseWorldPosition()
    {
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitdist;

        if (playerPlane.Raycast(ray, out hitdist))
        {
            return ray.GetPoint(hitdist);
        }

        return transform.position;
    }

    private Vector3 ClampPositionToMaxDistance(Vector3 targetPosition)
    {
        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance > maxDistance)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            targetPosition = transform.position + direction * maxDistance;
        }

        return targetPosition;
    }

    IEnumerator DoLightning()
    {
        Vector3 startLocation = transform.position;
        Quaternion quater = transform.rotation * Quaternion.Euler(90, 0, 0);

        GameObject attack = Instantiate(Lightning, startLocation, quater);

        Vector3 previousLocation = Vector3.zero;
        {
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitdist;

            if (playerPlane.Raycast(ray, out hitdist))
            {
                Vector3 targetPoint = ray.GetPoint(hitdist);

                GameObject firstTarget = FindNearestMonster(targetPoint);
                if (firstTarget != null)
                {
                    targetPoint = firstTarget.transform.position;
                    hitMonsters.Add(firstTarget);
                }

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
        for (int i = 0; i < 4; ++i)
        {
            GameObject nextTarget = FindNearestMonster(previousLocation);
            if (nextTarget == null)
                break;

            Vector3 targetPoint = nextTarget.transform.position;
            hitMonsters.Add(nextTarget);

            GameObject attack2 = Instantiate(Lightning, previousLocation, quater);
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

        hitMonsters.Clear();
    }

    GameObject FindNearestMonster(Vector3 position)
    {
        Collider[] hitColliders = Physics.OverlapBox(position, new Vector3(10, 10f, 10));
        GameObject nearestMonster = null;
        float shortestDistance = float.MaxValue;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster") && !hitMonsters.Contains(hitCollider.gameObject))
            {
                float distance = Vector3.Distance(position, hitCollider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestMonster = hitCollider.gameObject;
                }
            }
        }

        return nearestMonster;
    }

    void WaitForLightning()
    {
        alreadyLightning = false;
    }

    private void FixedUpdate()
    {
        /*if (Input.GetMouseButtonDown(0) && !alreadyLightning)
        {
            alreadyLightning = true;
            Invoke("WaitForLightning", 2);
            StartCoroutine(DoLightning());
        }*/
    }
}
