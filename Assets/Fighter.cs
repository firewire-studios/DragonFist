using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Fighter : MonoBehaviour
{
    public int health = 100;
    
    public int xSpeed = 0;
    public Queue<BufferedInput> buffer;
    public List<FighterMove> moves;

    public HealthBar healthbar;
    public int side = 0;
    
    /**
     *Hitbox
     */
    public GameObject Hitbox;

    /**
     * Hurtboxes
     */
    public GameObject Hurtbox;

    /**
     * If more than 0 then character cannot move
     */
    public int stillFrames = 0;

    /**
     * if more than 0 
     */
    public int pushFrames = 0;
    
    /**
     * Set in editor
     */
    [SerializeField]
    public int team;

    private void Awake()
    {
        buffer = new Queue<BufferedInput>();
        
        Hurtbox.SetActive(false);
        
        // Register Moves
        moves = new List<FighterMove>();
        
        // Basic Moves
        FighterMove Rock = new FighterMove("Rock",
            new List<Action>() {Action.Rock},
            10
        );
        
        FighterMove Paper = new FighterMove("Paper",
            new List<Action>() {Action.Paper},
            10
        );
        
        FighterMove Scissors = new FighterMove("Scissors",
            new List<Action>() {Action.Scissor},
            10
        );
        
        
        FighterMove Fireball = new FighterMove("Fireball",
            new List<Action>() {Action.Down,Action.Forward,Action.Paper},
            2
        );
        
        
        FighterMove Doomfist = new FighterMove("Doomfist",
                new List<Action>(){Action.Down,Action.Forward,Action.Rock},
                1
            );
        
        
        moves.Add(Rock);
        moves.Add(Paper);
        moves.Add(Scissors);
        
        moves.Add(Fireball);
        moves.Add(Doomfist);
        
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScissorAttack()
    {
        // Cannot attack while pushed
        if (pushFrames > 0)
        {
            return;
        }
        
        // Grab the correct hurtbox and make it active
        Hurtbox.SetActive(true);
    }

    public void FlushBuffer()
    {
        buffer.Clear();
    }
    
    public void FixedTimestepUpdate()
    {

        healthbar.SetHealth(health);

        if (stillFrames > 0)
        {

            stillFrames--;
            return;
        }

        if (pushFrames > 0)
        {
            // Move backwards

            xSpeed = side == 0 ? -1 : 1;
            pushFrames--;
        }
        
        Hurtbox.SetActive(false);
        transform.position += new Vector3(xSpeed * 0.2f,0,0);
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

    /**
     * Set the side of the fighter
     * 0 = left
     * 1 = right
     */
    public void SetSide(int _side)
    {
        side = _side;
        if (side == 0)
        {
            Hurtbox.transform.localPosition = new Vector3(0.6f,0.3f,-1);
            return;
        }

        if (side == 1)
        {
            Hurtbox.transform.localPosition = new Vector3(-0.6f,0.3f,-1);
        }
    }
}
