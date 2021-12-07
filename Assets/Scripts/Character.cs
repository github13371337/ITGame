//#define showRays

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags] public enum StateFlags
{
    [InspectorName("default")] default_ = 0,
    inAir = 1,
    stunned = 1 << 1,
    onSlope = 1 << 2,
    dead = 1 << 31
}

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float RunSpeed;
    [SerializeField] float groundContactMinDistance;

    public CharacterController controller { get; private set; }

    public float moveSpeed => MoveSpeed;
    public float runSpeed => RunSpeed;

    public StateFlags currentState;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
#if showRays
        Debug.DrawRay(new Vector3(transform.position.x + controller.center.x * transform.localScale.x,
                                  transform.position.y + controller.center.y * transform.localScale.y - controller.height * 0.5f * transform.localScale.y + 0.3f * transform.localScale.y,
                                  transform.position.z + controller.center.z * transform.localScale.z), -transform.up);
        Debug.DrawRay(new Vector3(transform.position.x + controller.center.x * transform.localScale.x,
                                  transform.position.y + controller.center.y * transform.localScale.y - controller.height * 0.5f * transform.localScale.y + 0.3f * transform.localScale.y,
                                  transform.position.z + controller.center.z * transform.localScale.z), transform.right);
#endif
        currentState &= ~(StateFlags.inAir | StateFlags.onSlope);
        if (!Physics.Raycast(new Vector3(transform.position.x + controller.center.x * transform.localScale.x,
                                         transform.position.y + controller.center.y * transform.localScale.y - controller.height * 0.5f * transform.localScale.y + 0.3f * transform.localScale.y,
                                         transform.position.z + controller.center.z * transform.localScale.z),
                                        -transform.up, groundContactMinDistance + 0.3f, Physics.DefaultRaycastLayers & ~(1 << gameObject.layer), QueryTriggerInteraction.Ignore)) currentState |= controller.isGrounded ? StateFlags.onSlope : StateFlags.inAir;       
    }   
}