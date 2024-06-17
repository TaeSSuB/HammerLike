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
    private float damage = 10f;
    private Vector3 playerTrs;
    [SerializeField] private float overlapSize = 3f;
    public void ChargeSkill(Vector3 position, Transform parent, SO_Skill skillData)
    {
        /*this.transform.position = position;
        StartCoroutine(DoLightning());*/
        ChargeAttack(position);
    }

    private void Start()
    {
        visualEffect = Thunder.GetComponent<VisualEffect>();

    }



    private void ChargeAttack(Vector3 position)
    {
        Vector3 thunderPosition;
        thunderPosition = ClampPositionToMaxDistance(position);
        thunderPosition.y += 5; // y 좌표를 5만큼 증가시킴

        GameObject thunderInstance = Instantiate(Thunder, thunderPosition, Quaternion.identity);
        visualEffect = thunderInstance.GetComponent<VisualEffect>();
        visualEffect.Play();
        Destroy(thunderInstance, 3); // Thunder 오브젝트를 3초 후에 제거

        StartCoroutine(DelayedDamage(thunderPosition));
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
        playerTrs = GameManager.Instance.Player.transform.position;
        Debug.Log(targetPosition);
        // z축 보정
        Vector3 correctedTargetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z * 2);

        float distance = Vector3.Distance(playerTrs, correctedTargetPosition);

        if (distance > maxDistance)
        {
            Vector3 direction = (correctedTargetPosition - playerTrs).normalized;
            correctedTargetPosition = playerTrs + direction * maxDistance;
        }

        // z축 보정 해제
        correctedTargetPosition.z /= 2;
        //correctedTargetPosition.z -= 2;

        return correctedTargetPosition;
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

                    B_Enemy enemy = firstTarget.GetComponent<B_Enemy>();
                    if (enemy != null)
                    {
                        Vector3 hitDir = (firstTarget.transform.position - startLocation).normalized;
                        enemy.TakeDamage(hitDir, (int)damage, 0, false);
                    }
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

            B_Enemy enemy = nextTarget.GetComponent<B_Enemy>();
            if (enemy != null)
            {
                Vector3 hitDir = (nextTarget.transform.position - previousLocation).normalized;
                enemy.TakeDamage(hitDir, (int)damage, 0, false);
            }

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
            if ((hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Monster")) && !hitMonsters.Contains(hitCollider.gameObject))
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

    private void WeaponAttack()
    {
        alreadyLightning = true;
        Invoke("WaitForLightning", 2);
        StartCoroutine(DoLightning());
    }
    public void WeaponAttack(Vector3 position)
    {
        alreadyLightning = true;
        Invoke("WaitForLightning", 2);
        StartCoroutine(DoLightning());
    }

    IEnumerator DelayedDamage(Vector3 position)
    {
        yield return new WaitForSeconds(0.1f);
        position.y = 0f;

        Collider[] hitColliders = Physics.OverlapSphere(position, overlapSize);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") || hitCollider.CompareTag("Monster"))
            {
                B_Enemy enemy = hitCollider.GetComponent<B_Enemy>();
                if (enemy != null && !hitMonsters.Contains(hitCollider.gameObject))
                {
                    Vector3 hitDir = (hitCollider.transform.position - position).normalized;
                    enemy.TakeDamage(hitDir, (int)damage, 0, false);
                    hitMonsters.Add(hitCollider.gameObject);
                }
            }
        }
    }
}
