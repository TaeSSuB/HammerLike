using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class B_Boss_SkeletonTorturer : B_Boss
{
    [SerializeField] private GameObject weaponObj;

    protected override void Start()
    {
        base.Start();

        weaponObj.SetActive(false);

        // Add Boss Patterns
        bossPatterns.Add(new B_BossPattern("Test_FirstPattern", 1, 3f, 3f, Pattern1, this));
        bossPatterns.Add(new B_BossPattern("", 2, 5f, 3f, Pattern2, this));
        bossPatterns.Add(new B_BossPattern("", 3, 7f, 3f, Pattern3, this));
    }

    protected override void Update()
    {
        base.Update();

        if (!bossPatterns.Any(p => p.isCurrentlyActive))
        {
            B_BossPattern nextPattern = bossPatterns
                .Where(p => p.IsReadyToActivatePattern(lastPatternActivatedTime))
                .OrderByDescending(p => p.priority)
                .FirstOrDefault();

            if (nextPattern != null)
            {
                StartCoroutine(nextPattern.ActivatePattern()); // Coroutine 실행
            }
        }
    }

    private IEnumerator Pattern1(B_BossPattern pattern)
    {
        // Pattern 1
        Debug.Log(this.gameObject.name + " : Pattern 1");

        var player = GameManager.Instance.Player;

        while (pattern.isCurrentlyActive)  // 지속적인 추적 로직
        {
            Anim.SetBool("IsChasing", true);
            agent.SetDestination(player.gameObject.transform.position);
            yield return null; // 매 프레임마다 위치 업데이트
        }

        Anim.SetBool("IsChasing", false);
        yield return null;
    }

    private IEnumerator Pattern2(B_BossPattern pattern)
    {
        // Pattern 2
        Debug.Log(this.gameObject.name + " : Pattern 2");
        
        // stop move agent
        agent.SetDestination(transform.position);
        weaponObj.SetActive(true);
        weaponObj.transform.position = transform.position + Vector3.up * 1f;

        var player = GameManager.Instance.Player;

        while (pattern.isCurrentlyActive)  // 지속적인 추적 로직
        {
            if(weaponObj.activeSelf == false)
                weaponObj.SetActive(true);
            // dir to player
            var dir = player.transform.position - transform.position;
            dir = GameManager.Instance.ApplyCoordDivideAfterNormalize(dir);
            weaponObj.transform.rotation = Quaternion.LookRotation(dir);
            
            //agent.SetDestination(player.gameObject.transform.position);
            yield return null; // 매 프레임마다 위치 업데이트
        }        
        
        weaponObj.SetActive(false);
        //Anim.SetBool("IsAttacking", false);
        yield return null;
    }

    private IEnumerator Pattern3(B_BossPattern pattern)
    {
        // Pattern 3
        Debug.Log(this.gameObject.name + " : Pattern 3");

        // stop move agent
        agent.SetDestination(transform.position);
        weaponObj.SetActive(true);
        weaponObj.transform.position = transform.position + Vector3.up * 1f;

        while (pattern.isCurrentlyActive)  // 지속적인 추적 로직
        {
            //Anim.SetBool("IsAttacking", true);
            if(weaponObj.activeSelf == false)
                weaponObj.SetActive(true);

            // wheelwind
            weaponObj.transform.Rotate(Vector3.up, 360f * Time.deltaTime);

            //agent.SetDestination(player.gameObject.transform.position);
            yield return null; // 매 프레임마다 위치 업데이트
        }

        weaponObj.SetActive(false);
        //Anim.SetBool("IsAttacking", false);
        yield return null;
    }
}
