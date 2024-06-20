using Rewired;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeshDestroy;

public class B_BreakableDoor : B_BreakableProp
{
    protected override void Dead()
    {
        base.Dead();
        B_AudioManager.Instance.PlaySound(AudioCategory.SFX, AudioTag.Crash);
    }
}
