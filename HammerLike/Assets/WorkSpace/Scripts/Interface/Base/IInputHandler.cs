using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler
{
    Vector3 GetMovementInput();
    bool IsWalkButtonHeld();

    bool IsAttackButtonPressed();
    bool IsAttackButtonHeld();
    bool IsAttackButtonRelease();

    bool IsInterActionButtonPressed();

    bool IsDashButtonPressed();

    float GetMouseScrollInput();
}
