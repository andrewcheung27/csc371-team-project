using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Input Action Set")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Player Inputs")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string interact = "Interact";
    [SerializeField] private string dash = "dash";
    [SerializeField] private string chargeDash = "chargeDash";

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction dashAction;
    private InputAction chargeDashAction;

    public Vector2 MoveInput { get; private set; }
    public bool jumpTriggered { get; private set; }
    public bool interactTriggered { get; private set; }
    public bool dashTriggered { get; private set; }
    public bool chargeDashTriggered { get; private set; }
    public float chargeDashStartTime = 0.0f;
    public float chargeDashDuration { get; private set; }

    public static PlayerInputHandler Instance { get; private set; }

    private IEnumerator ResetJumpTrigger()
    {
        yield return null; // Wait one frame
        jumpTriggered = false;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance =  this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);
        dashAction = playerControls.FindActionMap(actionMapName).FindAction(dash);
        chargeDashAction = playerControls.FindActionMap(actionMapName).FindAction(chargeDash);
        RegisterInputActions();
    }


    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        jumpAction.performed += context =>
        {
            jumpTriggered = true;
            // Debug.Log("Jump Pressed");
            StartCoroutine(ResetJumpTrigger());
        };
        jumpAction.canceled += context => jumpTriggered = false;
        
        interactAction.performed += context =>
        {
            interactTriggered = true;
            // Debug.Log("Interact Pressed");
        };
        interactAction.canceled += context => interactTriggered = false;
        
        dashAction.performed += context =>
        {
            dashTriggered = true;
            // Debug.Log("Dash Pressed");
        };
        dashAction.canceled += context => dashTriggered = false;
        
        chargeDashAction.performed += context =>
        {
            chargeDashTriggered = true;
            chargeDashStartTime = Time.time;
            // Debug.Log("Charge Dash Pressed");
        };
        chargeDashAction.canceled += context =>
        {
            chargeDashTriggered = false;
            chargeDashDuration = Time.time - chargeDashStartTime;
        };
    }


    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        interactAction.Enable();
        dashAction.Enable();
        chargeDashAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        interactAction.Disable();
        dashAction.Disable();
        chargeDashAction.Disable();
    }
}