using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    private BoxCollider2D _collider2D;
    private int team;
    private bool active;

    private void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();

        team = GetComponentInParent<Fighter>().team;
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
                    fighter.health -= 10;
                    fighter.pushFrames = 6;
                    
                    active = false;
                    return;
                }

            }
            
            //Debug.Log(collider.gameObject.name);
        }
    }
}