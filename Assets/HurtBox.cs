using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private BoxCollider2D _collider2D;
    private int team;
    public bool active;

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
        ContactFilter2D filter = new ContactFilter2D().NoFilter();

        List<Collider2D> results = new List<Collider2D>();
        _collider2D.OverlapCollider(filter, results);

        foreach (var collider in results)
        {
            if (collider.tag == "Hitbox")
            {
                // Check the team
                Fighter fighter = collider.gameObject.GetComponentInParent<Fighter>();
                if (fighter.team != team)
                {
                    if (fighter.launched)
                    {
                        Debug.Log("Launch Again");
                        active = false;
                        fighter.Launch();
                        return;
                    }
                    
                    if (fighter.blocking && fighter.crouching)
                    {
                        if (!parentFighter.currentAttack.standingMove)
                        {
                            parentFighter.stillFrames = 40; // make var
                            parentFighter.pushFrames = parentFighter.currentAttack.pushFrames;
                        
                            return;
                        }
                        
                        
                    }
                    
                    if (fighter.blocking && !fighter.crouching)
                    {
                        if (parentFighter.currentAttack.standingMove)
                        {
                            parentFighter.stillFrames = 40;
                            parentFighter.pushFrames = parentFighter.currentAttack.pushFrames;
                        
                            return;
                        }
                        
                    }
                    
                    fighter.health -= 10;
                    fighter.pushFrames = parentFighter.currentAttack.pushFrames;
                    fighter.currentAttack = null;

                    if (parentFighter.currentAttack.shouldLaunch)
                    {
                        fighter.Launch();
                    }

                    {
                        fighter.Stun();
                    }
                    
                    active = false;
                    return;
                }
                

            }
            
            //Debug.Log(collider.gameObject.name);
        }
    }
}
