using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    
    // Players
    public Fighter p1;
    public Fighter p2;
    private Fighter[] _players;
    
    private const float FixedTimeStep = 1000 / 60;
    private float currentTimeStep = 0.0f;

    private Array buttons;
    
    Gamepad gp1;
    Gamepad gp2;

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _players = new[] {p1, p2};
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
        
        PollInput(gp1,0);
        PollInput(gp2,1);
    }

    private void FixedTimeStepUpdate()
    {
        _players[0].FixedTimestepUpdate();
        _players[1].FixedTimestepUpdate();
    }

    private void PollInput(Gamepad gp, int team)
    {
        foreach (GamepadButton button in buttons)
        {
            if (gp[button].isPressed)
            {
                if (button == GamepadButton.DpadRight)
                {
                    _players[team].xSpeed = 1;
                }
                
                if (button == GamepadButton.DpadLeft)
                {
                    _players[team].xSpeed = -1;
                }
                
                Debug.Log(gp[button].shortDisplayName);
            }
        }
        
        
    }
}
