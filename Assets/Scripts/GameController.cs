using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class GameController : MonoBehaviour
{
    private bool firstInput = false;
    private int updates = 0;
    
    public TextMeshProUGUI centerText;
    public TextMeshProUGUI timer;

    public int preStartTimer = 120;
    
    public int roundTime = 60 * 60 * 60;
    public const int maxRoundTime = 60 * 60;
    
    public bool started = true;
    public Vector3 p1Startpos;
    public Vector3 p2Startpos;
    
    private Camera cam;
    private float maxCamx = 18;
    public float playerDistance = 0;
    
    public static GameController instance;
    public bool debugEnabled = true;

    // Players
    public Fighter p1;
    public Fighter p2;
    private Fighter[] _players;
    
    private float FixedTimeStep = 1000 / 60;
    private float currentTimeStep = 0.0f;

    private int restartCountdown = 0;
    private int restartCountDownFrames = 60;
    private int timeStepMultiplier = 4;

    private GamepadButton[] buttons;
    
    Gamepad gp1;
    Gamepad gp2;

    [SerializeField]
    public AudioSource sound;

    [SerializeField]
    public AudioSource music;
    
    public AudioClip Gong;
    public AudioClip roundSound;
    public AudioClip endRoundSound;
    public AudioClip blockSound;
    public AudioClip playAgain;
    public AudioClip winSound;
    public AudioClip wiff;
    public AudioClip counter;

    public static void PlayBlockSound()
    {
        instance.sound.PlayOneShot(instance.blockSound);

    }
    
    public static void PlayWiffSound()
    {
        instance.sound.PlayOneShot(instance.wiff);

    }
    
    public static void PlayCounterSound()
    {
        instance.sound.PlayOneShot(instance.counter);

    }
    
    private void Awake()
    {
        //sound = GetComponent<AudioSource>();
        
        roundTime = maxRoundTime;

        // Singleton
        if (instance == null)
        {
            instance = this;
            cam = Camera.main;
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _players = new[] {p1, p2};

        p1Startpos = p1.transform.position;
        p2Startpos = p2.transform.position;
        
        buttons = new[]
        {
            GamepadButton.DpadDown,
            GamepadButton.DpadUp,
            GamepadButton.DpadLeft,
            GamepadButton.DpadRight,

            GamepadButton.A,
            GamepadButton.X,
            GamepadButton.Y,
            GamepadButton.B,
            
            GamepadButton.Start,
        };

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

        
        
        //Toggle debug
        if (Input.GetKeyDown(KeyCode.Tilde))
        {
            debugEnabled = !debugEnabled;
        }
        
        currentTimeStep += Time.deltaTime * 1000;
        if (currentTimeStep >= FixedTimeStep)
        {
            if (updates < 60)
            {
                updates++;
            }
            
            FixedTimeStepUpdate();
            currentTimeStep = 0;
        }
        
        PollInput(gp1,0);
        PollInput(gp2,1);
        
        // Check for health and winner
        
    }

    private void FixedTimeStepUpdate()
    {
        
        if (!firstInput)
        {
            return;
        }
        
        float p1x = p1.transform.position.x;
        float p2x = p2.transform.position.x;
        playerDistance = Mathf.Abs(p1x - p2x);

        Vector3 Difference = p1.transform.position;
        Difference.x = (((p2x - p1x) / 2) + p1x);
        Difference.z = -10;
        Difference.y = 0.43f;

        if (Difference.x > 18)
        {
            Difference.x = 18;
        }

        if (Difference.x < -17.3f)
        {
            Difference.x = -17.3f;
        }

        cam.transform.position = Vector3.Lerp(cam.transform.position, Difference, 0.05f);
        

        if (preStartTimer > 0)
        {

            preStartTimer--;

            if (preStartTimer == 25)
            {
                sound.PlayOneShot(Gong);
                SetCenterText("FIGHT!");
            }

            if (preStartTimer <= 0)
            {
                SetCenterText("");
            }
            
            return;
        }
        
        if (!started)
        {
            if (restartCountdown <= 0)
            {
                if (p1.wins >= 3 || p2.wins >= 3)
                {
                    p1.wins = 0;
                    p2.wins = 0;

                    music.Stop();
                    firstInput = false;
                    roundTime = maxRoundTime;
                    sound.PlayOneShot(winSound);
                    SetCenterText("PRESS START");
                    return;
                }
                
                // restart the game
                p1.health = 100;
                p2.health = 100;
                p1.healthbar.SetHealth(100);
                p2.healthbar.SetHealth(100);
                
                FixedTimeStep /= timeStepMultiplier;
                started = true;

                p1.transform.position = p1Startpos;
                p2.transform.position = p2Startpos;
                
                preStartTimer = 120;
                SetCenterText("ROUND  " + (p1.wins + p2.wins + 1).ToString());
                sound.PlayOneShot(roundSound);
                roundTime = maxRoundTime;
                return;
                

            }
            else
            {
                restartCountdown -= 1;
            }
            //return;
        }
        
        // Set player left and right positions

       

        if (p1.transform.position.x < p2.transform.position.x)
        {
            p1.SetSide(0);
            p2.SetSide(1);
        }
        else
        {
            p1.SetSide(1);
            p2.SetSide(0);
        }
        
        // Pre player update
        foreach (var fighter in _players)
        {
            if (fighter.buffer.Count < 1)
            {
                continue;
            }
            
            if (fighter.buffer.Peek().frames >= 16)
            {
                //Debug.Log($"Dequeued {fighter.buffer.Peek().action}");
                fighter.buffer.Dequeue();
            }
            else
            {
                fighter.buffer.Peek().frames += 1;
            }

            // Gather a list of moves that we can do

            List<FighterMove> allowedMoves = new List<FighterMove>();
            BufferedInput[] buffer = fighter.buffer.ToArray();

            for (int i = 0; i < buffer.Length; i ++)
            {
                foreach (var fighterMove in fighter.moves)
                {
                    // Can this move be executed with this buffer
                    bool canUse = false;
                    int index = 0;

                    for (int j = i; j < buffer.Length; j++)
                    {
                        BufferedInput bufferedInput = buffer[j];
                        
                        if (fighterMove.Actions.Count >= index + 1)
                        {
                            if (fighterMove.Actions[index] != bufferedInput.action)
                            {
                                break;
                               // Debug.Log($"Move disallowed as {fighterMove.Actions[index]} does not match {bufferedInput.action}" );
                            }
                        }

                        index++;
                        
                        if (fighterMove.Actions.Count == index && fighterMove.Actions[index -1] == bufferedInput.action)
                        {
                            //Debug.Log("Here");
                            canUse = true;
                        }
                    }

                    if (canUse)
                    {
                        //Debug.Log($"Adding {fighterMove.MoveName} to allowed moves");
                        allowedMoves.Add(fighterMove);
                    }
                    
                }
            }


            // Get the first item of the list and use the move
            FighterMove selectedMove = null;
            if (allowedMoves.Count > 0)
            {
                // remove any invalid moves (standing)
                allowedMoves.RemoveAll(move =>
                    (move.standingMove && fighter.crouching) || (!move.standingMove && !fighter.crouching));
                
                //Debug.Log($"Allowed Moves {allowedMoves.Count}");
                
                // Sort the list of allowed moves

                if (allowedMoves.Count > 0)
                {
                    allowedMoves.Sort((x, y) => x.Priority.CompareTo(y.Priority));
                    selectedMove = allowedMoves[0];
                }

            }

            if (selectedMove != null && fighter.currentAttack == null && !fighter.stunned && fighter.stillFrames <= 0)
            {
                Debug.Log($"Used Move {selectedMove.MoveName}");
                fighter.FlushBuffer();

                fighter.UseAttack(selectedMove);
            }
            
        }
        
        
        _players[0].FixedTimestepUpdate();
        _players[1].FixedTimestepUpdate();
        roundTime -= 1;
        timer.text = (roundTime/60).ToString();
        
        
        // Check health
        if (p1.health <= 0 || p2.health <= 0 || roundTime <=0)
        {

            if (!started)
            {
                return;
            }

            restartCountdown = restartCountDownFrames;
            Fighter winner = p1.health > p2.health ? p1 : p2;
            Fighter loser = winner.team ==0 ? p2 : p1;
            
            winner.spr.sprite = winner.StandingSprite;
            winner.wins += 1;
            //loser.spr.sprite = loser.losingSprite;
            
            loser.Launch();
            loser.pushFrames = 40;
            started = false;
            sound.PlayOneShot(endRoundSound);

            
            FixedTimeStep *= timeStepMultiplier;
        }
    }

    public static float GetDifferenceToPlayer(int _player, Vector3 newPosition)
    {
        Vector3 otherPlayerPosition = _player == 0 ? instance.p2.transform.position : instance.p1.transform.position;

        return Mathf.Abs(otherPlayerPosition.x - newPosition.x) ;
    }

    public void SetCenterText(string text)
    {
        centerText.text = text;
    }

    public static void playAudio(AudioClip clip)
    {
        instance.sound.PlayOneShot(clip);
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
                    _players[team].HandleButtonPressed(button);
                }
                
                if (button == GamepadButton.DpadLeft)
                {
                    _players[team].xSpeed = -1;
                    _players[team].HandleButtonPressed(button);
                }
                
                if (button == GamepadButton.DpadDown)
                {
                    //_players[team].crouching = true;
                    
                    if (_players[team].currentAttack != null && _players[team].currentAttack.standingMove )
                    {
                        // If in a crouch attack do not allow crouch
                    }
                    else
                    {
                        _players[team].crouching = true;

                    }
                    
                    //_players[team].HandleButtonPressed(button);
                    
                    
                }
                
                if (button == GamepadButton.B)
                {
                    if (!_players[team].stunned && !_players[team].launched && _players[team].stillFrames <= 0)
                    {
                        _players[team].blocking = true;

                    }
                }
                
            }

            if (gp[button].wasReleasedThisFrame || !gp[button].isPressed)
            {
                
                
                if (button == GamepadButton.DpadDown)
                {
                    if (_players[team].currentAttack != null && !_players[team].currentAttack.standingMove )
                    {
                        // If in a crouch attack do not allow standing up
                    }
                    else
                    {
                        _players[team].crouching = false;

                    }
                    
                    //_players[team].HandleButtonPressed(button);
                }

                if (button == GamepadButton.B)
                {
                    
                    _players[team].blocking = false;
                }
            }
            
            
            if (gp[button].wasPressedThisFrame)
            {
                if (!firstInput  && button == GamepadButton.Start)
                {
                    sound.PlayOneShot(playAgain);
                    
                    Debug.Log($"first input {button}");
                    music.Play();
                    SetCenterText("ROUND 1");
                    sound.PlayOneShot(roundSound);
                    
                    
                    firstInput = true;
                }
                
                _players[team].HandleButtonPressed(button);
            }
        }
    }
}
