using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Fighter : MonoBehaviour
{
    public int xSpeed = 0;
    public Queue<BufferedInput> buffer;
    public List<FighterMove> moves;
    
    /**
     * Set in editor
     */
    [SerializeField]
    public int team;

    private void Awake()
    {
        buffer = new Queue<BufferedInput>();
        
        // Register Moves
        moves = new List<FighterMove>();
        
        FighterMove Fireball = new FighterMove("Fireball",
            new List<Action>() {Action.Down,Action.Forward,Action.Paper},
            1
        );
        
        FighterMove Punch = new FighterMove("Punch",
            new List<Action>() {Action.Paper},
            10
        );
        
        moves.Add(Fireball);
        moves.Add(Punch);
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FlushBuffer()
    {
        buffer.Clear();
    }
    
    public void FixedTimestepUpdate()
    {
        transform.position += new Vector3(xSpeed,0,0);
        xSpeed = 0;
    }
    
    public void HandleButtonPressed(GamepadButton button)
    {
        Action _action = MapGamepadButtonToAction(button);

        if (_action == Action.None)
        {
            return;
        }
        
        buffer.Enqueue(new BufferedInput(_action));
        //Debug.Log($"Queued {_action}");

        if (buffer.Peek().action == _action)
        {
            
        }
    }

    public Action MapGamepadButtonToAction(GamepadButton  button)
    {
        switch (button)
        {
            // Switch if facing other direction todo:
            case GamepadButton.DpadLeft: return Action.Backward;
            case GamepadButton.DpadRight: return Action.Forward;
            
            case GamepadButton.DpadDown: return Action.Down;
            
            case GamepadButton.X: return Action.Paper;
            case GamepadButton.A: return Action.Rock;
            case GamepadButton.Y: return Action.Scissor;
        }

        return Action.None;
    }

    /**
     * Mapping from raw input into a contextual action
     */
    public enum Action
    {
        None,
        Forward,
        Backward,
        Down,
        Jump,
        Rock, // a
        Paper, // x
        Scissor // y
    }
}
