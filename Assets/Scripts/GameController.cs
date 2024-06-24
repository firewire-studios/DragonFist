using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GameController : MonoBehaviour
{
    private const float FixedTimeStep = 1000 / 60;
    private float currentTimeStep = 0.0f;

    private Array buttons;
    
    Gamepad gp1;
    Gamepad gp2;

    // Start is called before the first frame update
    void Start()
    {
        buttons = Enum.GetValues(typeof(GamepadButton));

        // Validate gamepads
        if (Gamepad.all.Count < 2)
        {
            Debug.LogError("Less than two gamepads detected");
        }
        
        gp1 = Gamepad.all[0];
        gp2 = Gamepad.all[1];

    }

    // Update is called once per frame
    void Update()
    {
        currentTimeStep += Time.deltaTime * 1000;
        if (currentTimeStep >= FixedTimeStep)
        {
            FixedTimeStepUpdate();
            currentTimeStep = 0;
        }
        
        PollInput(gp1);
        PollInput(gp2);
    }

    private void FixedTimeStepUpdate()
    {
        
    }

    private void PollInput(Gamepad gp)
    {
        foreach (GamepadButton button in buttons)
        {
            if (gp[button].wasReleasedThisFrame)
            {
                Debug.Log(gp[button].shortDisplayName);
            }
        }
    }
}
