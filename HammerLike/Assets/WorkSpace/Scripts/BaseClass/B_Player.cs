using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class B_Player : B_UnitBase
{
    // override Update
    protected override void Update()
    {
        base.Update();

        Move(InputWASD());
    }
    
    private Vector3 InputWASD()
    {
        // Input logic
        Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDir.Normalize();

        return moveDir;
    }

    void Dash()
    {
        // Dash logic
    }

    void ChargeAttack()
    {
        // Charge attack logic
    }

}
