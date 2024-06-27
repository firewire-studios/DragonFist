using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private BoxCollider2D _collider2D;
    private int team;
    public bool active;
    private const int dmgMult = 1;

    
    private Fighter parentFighter;
    private void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();

        parentFighter = GetComponentInParent<Fighter>();
        team = parentFighter.team;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        active = true;
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!active)
        {
            return;
        }

        Vector3 hitobjSpawnPosition = transform.position;
        hitobjSpawnPosition.z = -5;
        ContactFilter2D filter = new ContactFilter2D().NoFilter();

        List<Collider2D> results = new List<Collider2D>();
        _collider2D.OverlapCollider(filter, results);

        foreach (var collider in results)
        {
            if (collider.tag == "Hurtbox")
            {
                Fighter fighter = collider.gameObject.GetComponentInParent<Fighter>();
                if (fighter.team == parentFighter.team)
                {
                    return;
                }

                if (fighter.currentAttack != null)
                {
                    if (parentFighter.currentAttack.Beats(fighter.currentAttack))
                    {
                        GameController.PlayCounterSound();
                        parentFighter.CounterBox.GetComponent<AppearAndFade>().Appear();

                        fighter.DeactivateHurtBox(fighter.currentAttack.hurtbox);
                        fighter.Launch();
                        fighter.pushFrames = parentFighter.currentAttack.pushFrames;
                        fighter.currentAttack = null;
                        active = false;
                        return;
                    }
                }
                return;
            }

                if (collider.tag == "Hitbox")
            {
                // Check the team
                Fighter fighter = collider.gameObject.GetComponentInParent<Fighter>();
                if (parentFighter.currentAttack == null)
                {
                    return;
                }
                
                int dmg = parentFighter.currentAttack.dmg * dmgMult;
                
                if (fighter.team != team)
                {
                    if (fighter.launched)
                    {
                        Instantiate(parentFighter.HitObjPrefab, hitobjSpawnPosition, Quaternion.identity);
                        parentFighter.currentAttack.playSound();
                        active = false;
                        fighter.Launch();
                        fighter.pushFrames = parentFighter.currentAttack.pushFrames;
                        fighter.health -= dmg;
                        return;
                    }
                    
                    // If the attack is blocked
                    if (fighter.blocking && fighter.crouching)
                    {
                        if (!parentFighter.currentAttack.standingMove) //check null todo:
                        {
                            GameController.PlayBlockSound();
                            Instantiate(parentFighter.BlockObjPrefab, hitobjSpawnPosition, Quaternion.identity);
                            parentFighter.stillFrames = 10; // make var
                            parentFighter.pushFrames = parentFighter.currentAttack.pushFrames;
                            parentFighter.currentAttack = null;
                            parentFighter.Stun();
                            active = false;

                            return;
                        }
                        
                    }
                    
                    if (fighter.blocking && !fighter.crouching)
                    {
                        if (parentFighter.currentAttack.standingMove)
                        {
                            GameController.PlayBlockSound();
                            Instantiate(parentFighter.BlockObjPrefab, hitobjSpawnPosition, Quaternion.identity);
                            parentFighter.stillFrames = 10;
                            parentFighter.pushFrames = parentFighter.currentAttack.pushFrames;
                            parentFighter.currentAttack = null;
                            parentFighter.Stun();
                            active = false;

                            return;
                        }
                        
                    }
                    
                    // If they are doing a move that you counter then launch them
                    if (fighter.currentAttack != null)
                    {
                        
                        
                        if (parentFighter.currentAttack.Beats(fighter.currentAttack))
                        {
                            GameController.PlayCounterSound();
                            fighter.Launch();
                            fighter.health -= dmg;
                            parentFighter.CounterBox.GetComponent<AppearAndFade>().Appear();
                            fighter.pushFrames = parentFighter.currentAttack.pushFrames;
                           // fighter.currentAttack = null;
                           active = false;
                            return;
                        }
                    }

                    
                    fighter.health -= dmg;
                    fighter.pushFrames = parentFighter.currentAttack.pushFrames;
                    fighter.currentAttack = null;

                    if (parentFighter.currentAttack.shouldLaunch)
                    {
                        fighter.Launch();
                    }

                    {
                        fighter.Stun();
                    }

                    Instantiate(parentFighter.HitObjPrefab, hitobjSpawnPosition, Quaternion.identity);
                    parentFighter.currentAttack.playSound();
                    active = false;
                    return;
                }
                

            }
            
            

            //Debug.Log(collider.gameObject.name);
        }
        
        GameController.PlayWiffSound();
        active = false;
    }
}
