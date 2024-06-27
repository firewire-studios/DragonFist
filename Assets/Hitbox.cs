using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    private BoxCollider2D _collider2D;
    public bool canBePushedThisFrame = true;
    public Fighter parent;

    
    private void Awake()
    {
        _collider2D = GetComponent<BoxCollider2D>();
        parent = GetComponentInParent<Fighter>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        canBePushedThisFrame = true;
        ContactFilter2D filter = new ContactFilter2D().NoFilter();

        List<Collider2D> results = new List<Collider2D>();
        _collider2D.OverlapCollider(filter, results);

        foreach (var collider in results)
        {
            if (collider.tag == "Hitbox")
            {
                // can the collider be pushed?
                if (collider.gameObject.GetComponent<Hitbox>().canBePushedThisFrame)
                {
                    // Check the team
                    Fighter fighter = collider.gameObject.GetComponentInParent<Fighter>();
                    //fighter.pushFrames = 2;
                    //parent.pushFrames = 1;
                    //canBePushedThisFrame = false;
                }
                
            }
            
            //Debug.Log(collider.gameObject.name);
        }
    }
}
