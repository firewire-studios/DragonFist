using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class Fighter : MonoBehaviour
{
    public Sprite StandingSprite;
    public Sprite CrouchingSprite;
    [SerializeField] public List<Sprite> WalkSprites;
    [SerializeField] public List<Sprite> WalkBackSprites;
    [SerializeField] public List<Sprite> StunSprites;
    
    
    [SerializeField] public List<Sprite> RockPunchSprites;
    [SerializeField] public List<Sprite> DragonJawSprites;
    
    /*
     * Temp
     */

    public bool rockpunch = false;

    public FighterMove currentAttack = null;
    
    //===========
    
    private int currentSpriteIndex = 0;
    private int frameInterval = 5;
    private int currentFrame = 0;
    
    public SpriteRenderer spr;
    
    public int health = 100;

    public bool crouching = false;
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
    
    // Hurtbox transforms
    private Vector3 hurtBoxStartPosition;
    
    // Hitbox transforms
    private Vector3 hitBoxStandingScale =new Vector3(0.879074633f,1.41376948f,1.31819999f) ;
    private Vector3 hitBoxStandingPosition =new Vector3(0,0f,-1);
    
    private Vector3 hitBoxCrouchingScale = new Vector3(0.879074633f,1.04654276f,1.31819999f);
    private Vector3 hitBoxCrouchingPosition = new Vector3(0,-0.210999995f,-1);

    public bool stunned = false;

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
        spr = GetComponent<SpriteRenderer>();
        spr.sprite = StandingSprite;
        buffer = new Queue<BufferedInput>();

        //Hitbox.GetComponent<SpriteRenderer>().enabled = false;
        //Hurtbox.GetComponent<SpriteRenderer>().enabled = false;
        
        hurtBoxStartPosition = Hurtbox.transform.localPosition;
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
        
        // Special Moves
        FighterMove DragonJaw = new FighterMove("DragonJaw",
            new List<Action>() {Action.Backward,Action.Rock},
            5
        );
        
        // Configure Frames
        Rock.idleFrames = 1;
        Rock.hurtFrames = 2;
        Rock.coolDownFrames = 1;
        Rock.sprites = RockPunchSprites;
        //Rock.frameInterval = 30;
        moves.Add(Rock);

        DragonJaw.idleFrames = 1;
        DragonJaw.hurtFrames = 1;
        DragonJaw.coolDownFrames = 1;
        DragonJaw.sprites = DragonJawSprites;
        DragonJaw.frameInterval = 4;
        moves.Add(DragonJaw);
        
        //moves.Add(Paper);
        //moves.Add(Scissors);

        //moves.Add(Fireball);
        //moves.Add(Doomfist);

    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UseAttack(FighterMove move)
    {
        
        // Cannot attack while pushed
        if (pushFrames > 0)
        {
            return;
        }
        
        // Grab the correct hurtbox and make it active
        currentAttack = move;
        rockpunch = true; // ahhh
        currentSpriteIndex = 0;
        stillFrames = 5;
        
    }

    public void ActivateHurtBox()
    {
        Hurtbox.SetActive(true);
        Hurtbox.GetComponent<HurtBox>().active = true;
    }

    public void DeactivateHurtBox()
    {
        Hurtbox.SetActive(false);
        Hurtbox.GetComponent<HurtBox>().active = false;
    }

    public void FlushBuffer()
    {
        buffer.Clear();
    }

    public void Stun()
    {
        stunned = true;
        currentSpriteIndex = 0;
    }
    
    public void FixedTimestepUpdate()
    {

        healthbar.SetHealth(health);
        
        if (pushFrames > 0)
        {
            // Move backwards

            xSpeed = side == 0 ? -1 : 1;
            transform.position += new Vector3(xSpeed * 0.025f,0,0);
            pushFrames--;
            xSpeed = 0;
        }

        if (stunned)
        {
            if (currentFrame >= 4) // todo
            {
                spr.sprite = StunSprites[currentSpriteIndex];
                currentSpriteIndex = currentSpriteIndex + 1 >= StunSprites.Count?  0: currentSpriteIndex + 1;
                currentFrame = 0;

                if (currentSpriteIndex == 0)
                {
                    stunned = false;
                }
            }
            currentFrame++;

            return;
        }
        

        // Animate current move
        if (currentAttack != null)
        {
            if (currentFrame >= currentAttack.frameInterval)
            {
                // If has looped over
                if (currentSpriteIndex == currentAttack.sprites.Count)
                {
                    currentSpriteIndex = 0;
                    spr.sprite = StandingSprite;
                    currentAttack = null;
                    currentFrame++;
                    return;
                }
                
                spr.sprite = currentAttack.sprites[currentSpriteIndex];
                if (currentAttack.IsHurtFrame(currentSpriteIndex))
                {
                    Debug.Log("Activeate Hurt Box");
                    ActivateHurtBox();
                }

                if (currentAttack.IsCooldownFrame(currentSpriteIndex))
                {
                    DeactivateHurtBox();
                }

                currentSpriteIndex++; ///AHHHHHHHH;
                currentFrame = 0;

                
            }
            currentFrame++;

            return;
        }
        

        if (stillFrames > 0)
        {

            stillFrames--;
            return;
        }


        if (crouching)
        {
            spr.sprite = CrouchingSprite;
            Hitbox.transform.localPosition = hitBoxCrouchingPosition;
            Hitbox.transform.localScale = hitBoxCrouchingScale;
            currentFrame++;
            return;
        }
        else
        {
            Hitbox.transform.localPosition = hitBoxStandingPosition;
            Hitbox.transform.localScale = hitBoxStandingScale;
        }
        
        // Hurtbox should not exist while the player can move
        Hurtbox.SetActive(false);
        transform.position += new Vector3(xSpeed * 0.2f,0,0);

        if (xSpeed != 0)
        {
            // Get walking direction
            int direction = side == 0 ? xSpeed : -xSpeed;

            if (direction == 1)
            {
                if (currentFrame >= frameInterval)
                {
                    spr.sprite = WalkSprites[currentSpriteIndex];
                    currentSpriteIndex = currentSpriteIndex + 1 >= WalkSprites.Count?  0: currentSpriteIndex + 1;
                    currentFrame = 0;
                }
            }
            
            if (direction == -1)
            {
                if (currentFrame >= frameInterval)
                {
                    spr.sprite = WalkBackSprites[currentSpriteIndex];
                    currentSpriteIndex = currentSpriteIndex + 1 >= WalkBackSprites.Count?  0: currentSpriteIndex + 1;
                    currentFrame = 0;
                }
            }
            
        }
        else
        {
            spr.sprite = StandingSprite;
        }
        
        
        

        xSpeed = 0;
        currentFrame++;

    }
    
    public void HandleButtonPressed(GamepadButton button)
    {
        Action _action = MapGamepadButtonToAction(button);

        if (_action == Action.None)
        {
            return;
        }
        
        //Debug.Log($"Queued {_action}");

        if (buffer.Count > 0 && buffer.ToArray()[buffer.Count -1].action == _action)
        {
            return;
        }
        
        buffer.Enqueue(new BufferedInput(_action));
    }

    public Action MapGamepadButtonToAction(GamepadButton  button)
    {
        switch (button)
        {
            // Switch if facing other direction todo:
            case GamepadButton.DpadLeft: return side == 0? Action.Backward : Action.Forward;
            case GamepadButton.DpadRight: return side == 0? Action.Forward : Action.Backward;
            
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
            Hurtbox.transform.localPosition = hurtBoxStartPosition;
            spr.flipX = false;
            return;
        }

        if (side == 1)
        {
            spr.flipX = true;
            Hurtbox.transform.localPosition = new Vector3(-hurtBoxStartPosition.x,hurtBoxStartPosition.y,-hurtBoxStartPosition.z);
        }
    }
}
