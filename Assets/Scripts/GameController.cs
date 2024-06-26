using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool debugEnabled = true;

    // Players
    public Fighter p1;
    public Fighter p2;
    private Fighter[] _players;
    
    private const float FixedTimeStep = 1000 / 60;
    private float currentTimeStep = 0.0f;

    private GamepadButton[] buttons;
    
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
        buttons = new[]
        {
            GamepadButton.DpadDown,
            GamepadButton.DpadUp,
            GamepadButton.DpadLeft,
            GamepadButton.DpadRight,

            GamepadButton.A,
            GamepadButton.X,
            GamepadButton.Y,
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
            FixedTimeStepUpdate();
            currentTimeStep = 0;
        }
        
        PollInput(gp1,0);
        PollInput(gp2,1);
    }

    private void FixedTimeStepUpdate()
    {
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
            
            if (fighter.buffer.Peek().frames >= 8)
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

            if (selectedMove != null && fighter.currentAttack == null && !fighter.stunned)
            {
                Debug.Log($"Used Move {selectedMove.MoveName}");
                fighter.FlushBuffer();

                fighter.UseAttack(selectedMove);
            }
            
        }
        
        
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
            }
            
            if (gp[button].wasPressedThisFrame)
            {
                _players[team].HandleButtonPressed(button);
            }
        }
    }
}
