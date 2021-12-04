using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0649

public enum UIFocus
{
    game,
    menu
}

public enum MovementDirection
{
    forward = 1,
    backwards = -1, 
    rigth = 2,
    left = -2
}

[RequireComponent(typeof(Character))]
public class Control : MonoBehaviour
{
    [SerializeField] UIFocus Focus = UIFocus.game;
    [SerializeField] bool horizontalAxisActive = false;
    [SerializeField] bool verticalAxisActive = false;
    [SerializeField] [EditorReadOnly] Vector3 movement;
    [SerializeField] float Mass;
    [SerializeField] float fallSpeedLimit;
    [SerializeField] float groundedGravity;
    [SerializeField] float[] jumpSpeeds;
    [SerializeField] bool infiniteJumps;
    [SerializeField] bool checkSlope;
    [SerializeField] [HideUnless("checkSlope", true)] float slipping = 0.1f;

    private Character character;
    private MouseLook mouse;
    private float moveSpeed;
    private bool run;
    private bool jump;
    private float tVertSpeed;
    private int jumpIndex = 1;
    private ControllerColliderHit controllerColliderHit;
    private Vector3 groundDir;

    public float mass
    {
        get => Mass;
        set => Mass = value > 0 ? value : 0;
    }

    public UIFocus focus => Focus;

    public bool alwaysRun;

    private void Awake()
    {
        character = GetComponent<Character>();
        mouse = GetComponentInChildren<MouseLook>();
        FocusChange += ChangeMouseState;
    }

    public void ChangeMouseState(UIFocus focus)
    {
        bool lock_ = focus == UIFocus.game ? true : false;
        mouse.SetCursorLock(lock_);
    }

    private void Update ()
    {
        #region UI
        if (Focus == UIFocus.menu)
        {
            if (Input.GetMouseButtonDown(0)) { ScreenMouseClick?.Invoke(Input.mousePosition); }
            else if (Input.GetMouseButtonUp(0)) MouseRelease?.Invoke();
            else if (Input.GetButtonDown(Constants.Input.confirmButton)) ConfirmButtonPressed?.Invoke();
            else if (Input.GetButtonDown(Constants.Input.cancelButton)) CancelButtonPressed?.Invoke();
            else
            {
                float axis;
                if ((axis = Input.GetAxisRaw(Constants.Input.horizontalAxis)) != 0 && !horizontalAxisActive)
                {
                    GuiMovement?.Invoke((MovementDirection)(axis * 2));
                    horizontalAxisActive = true;
                }
                else if ((axis = Input.GetAxisRaw(Constants.Input.verticalAxis)) != 0 && !verticalAxisActive)
                {
                    GuiMovement?.Invoke((MovementDirection)(axis));
                    verticalAxisActive = true;
                }
                if (Input.GetAxisRaw(Constants.Input.horizontalAxis) == 0) horizontalAxisActive = false;
                if (Input.GetAxisRaw(Constants.Input.verticalAxis) == 0) verticalAxisActive = false;
            }                
        }
        #endregion

        #region movement
        tVertSpeed = movement.y;
        movement.y = 0f;
        movement.x = 0f;
        movement.z = 0f;
        jump = false;

        #region movement input
        if (focus == UIFocus.game)
        {
            run = Input.GetButton(Constants.Input.runButton);
            if (alwaysRun) run = !run;

            if (run) moveSpeed = character.runSpeed;
            else moveSpeed = character.moveSpeed;

            movement.z = Input.GetAxis(Constants.Input.verticalAxis);

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                Vector3 rotation = mouse.transform.eulerAngles;
                Vector3 dir = new Vector3(transform.eulerAngles.x, mouse.transform.eulerAngles.y + (Mathf.Atan2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"))) * Mathf.Rad2Deg, transform.eulerAngles.z);
                transform.eulerAngles = dir;
                mouse.transform.eulerAngles = rotation;
            }

            movement.z = Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Horizontal"));
            movement.z = Mathf.Clamp01(movement.z);
            movement.z *= moveSpeed;

            jump = Input.GetButtonDown(Constants.Input.jumpButton);
        }
        #endregion

        movement.y = tVertSpeed;

        #region jumps and slopes
        if (character.currentState.HasFlag(StateFlags.inAir))
        {
            if (movement.y > fallSpeedLimit) movement.y += Physics.gravity.y * Mass * Time.deltaTime;

            if (jump)
            {
                if (!infiniteJumps)
                {
                    if (jumpIndex < jumpSpeeds.Length)
                    {
                        movement.y += jumpSpeeds[jumpIndex];
                        movement.y = Mathf.Clamp(movement.y, 0f, jumpSpeeds[jumpIndex]);
                        jumpIndex++;
                    }
                }
                else
                {
                    movement.y += jumpSpeeds[0];
                    movement.y = Mathf.Clamp(movement.y, 0f, jumpSpeeds[0]);
                }
            }
        }
        else
        {
            movement.y = groundedGravity;
            jumpIndex = 1;

            if (checkSlope && character.currentState.HasFlag(StateFlags.onSlope))
            {
                groundDir = transform.InverseTransformDirection(Vector3.Normalize(controllerColliderHit.point - new Vector3
                (
                    transform.position.x + character.controller.center.x * transform.localScale.x,
                    transform.position.y + character.controller.center.y * transform.localScale.y - character.controller.height * transform.localScale.y,
                    transform.position.z + character.controller.center.z * transform.localScale.z
                )));

                movement -= groundDir * mass * slipping * Mathf.Acos(Vector3.Dot(groundDir, Vector3.down)) * Mathf.Rad2Deg;
            }
            else if (jump) movement.y = jumpSpeeds[0];
        }
        #endregion

        character.controller.Move(transform.TransformDirection(movement * Time.deltaTime));
        #endregion
    }

    protected void ChangeUIFocus(UIFocus focus)
    {
        if (Focus != focus)
        {
            Focus = focus;
            FocusChange?.Invoke(focus);
        }
    }

    public event Action ConfirmButtonPressed;
    public event Action CancelButtonPressed;
    public event Action<MovementDirection> GuiMovement;
    public event Action<Vector3> ScreenMouseClick;
    public event Action MouseRelease;
    public event Action<UIFocus> FocusChange;
}
