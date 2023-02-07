using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class WadeInputs : MonoBehaviour
{


    public Gamepad gamepad;
    public Keyboard keyboard;
    public Controls controls;
    public Vector2 moveInput;
    public bool jumpPress;
    public bool jumpHeld;
    public bool shootPress;
    public bool shootHeld;
    public bool item1Press;
    public bool item1Held;
    public bool triggerHeld;


    // Use this for initialization
    void Start()
    {
        gamepad = InputSystem.GetDevice<Gamepad>();
        keyboard = InputSystem.GetDevice<Keyboard>();
    
    }

    private void OnEnable()
    {
        controls = new Controls();
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }


    private void Awake()
    {
        controls = new Controls();
        
        
    }

    void MoveInput(Vector2 inputs)
    {
        moveInput = inputs;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = controls.Inputs.Movement.ReadValue<Vector2>();
        if (Mathf.Abs(moveInput.x) > 0.2) { moveInput.x = Mathf.Sign(moveInput.x); } else { moveInput.x = 0; }
        if (Mathf.Abs(moveInput.y) > 0.2) { moveInput.y = Mathf.Sign(moveInput.y); } else { moveInput.y = 0; }

        gamepad = InputSystem.GetDevice<Gamepad>();

        keyboard = InputSystem.GetDevice<Keyboard>();


        
        if (keyboard != null && gamepad == null)
        {
            jumpHeld = keyboard.jKey.isPressed;
            jumpPress = keyboard.jKey.wasPressedThisFrame;
            shootHeld = keyboard.kKey.isPressed;
            shootPress = keyboard.kKey.wasPressedThisFrame;
            item1Held = keyboard.nKey.isPressed;
            item1Press = keyboard.nKey.wasPressedThisFrame;
            triggerHeld = keyboard.lKey.isPressed;
        }
        else if (keyboard == null && gamepad != null)
        {
            jumpHeld = gamepad.buttonSouth.isPressed;
            jumpPress = gamepad.buttonSouth.wasPressedThisFrame;
            shootHeld = gamepad.buttonWest.isPressed;
            shootPress = gamepad.buttonWest.wasPressedThisFrame;
            item1Held = gamepad.buttonNorth.isPressed;
            item1Press = gamepad.buttonNorth.wasPressedThisFrame;
            triggerHeld = gamepad.rightTrigger.isPressed;
        }
        else
        {
            jumpHeld = gamepad.buttonSouth.isPressed || keyboard.jKey.isPressed;
            jumpPress = gamepad.buttonSouth.wasPressedThisFrame || keyboard.jKey.wasPressedThisFrame;
            shootHeld = gamepad.buttonWest.isPressed || keyboard.kKey.isPressed;
            shootPress = gamepad.buttonWest.wasPressedThisFrame || keyboard.kKey.wasPressedThisFrame;
            item1Held = keyboard.nKey.isPressed || gamepad.buttonNorth.isPressed;
            item1Press = keyboard.nKey.wasPressedThisFrame || gamepad.buttonNorth.wasPressedThisFrame;
            triggerHeld = gamepad.rightTrigger.isPressed || keyboard.lKey.isPressed;
        }

    }

 
}



