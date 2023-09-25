using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UniRx;
using UniRx.Triggers;

public class PlayerTest : MonoBehaviour
{
    enum MainState
    {
        Idle,
        TakingDamage,
        Move,
        Dash
    }

    enum ActionState
    {
        Idle,
        Attacking,
        Charging
    }

    public Transform upperbody;
    public Transform lowerbody;

    MainState mainState;
    ActionState actionState;

    // Start is called before the first frame update
    void Start()
    {







        // 테스트용 임시

        // 이동
        this.UpdateAsObservable()
            .Where(_ => Input.GetAxis("Horizontal") != 0.0f || Input.GetAxis("Vertical") != 0.0f)
            .Select(_ => new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")))
            .Subscribe(Move);

        // 커서 위치로 머리 회전
        this.UpdateAsObservable()
            .Where(_ => Time.timeScale != 0.0f)
            .Select(_ =>
            {
                Vector3 dir = Camera.main.ScreenToViewportPoint(Input.mousePosition) - Camera.main.WorldToViewportPoint(upperbody.position);
                return new Vector3(dir.x, 0.0f, dir.y);
            })
            .Subscribe(Turn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move(Vector3 moveDelta)
    {
        transform.position = transform.position + moveDelta * Time.deltaTime;
    }

    void Turn(Vector3 target)
    {
        upperbody.forward = target.normalized;
    }
}
