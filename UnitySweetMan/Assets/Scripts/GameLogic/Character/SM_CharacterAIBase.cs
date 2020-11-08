using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class SM_CharacterAIBase : SM_CharacterBase
{
    [HideInInspector]
    public Transform Target;
    
    [HideInInspector]
    public NavMeshAgent Agent;

    [HideInInspector]
    public float DistToGround;
    
    [HideInInspector]
    public SM_CharacterAnimator CharacterAnimator;

    public abstract void Init();

    public abstract void DoUpdate(float dt);

    public abstract bool IsGrounded();
}
