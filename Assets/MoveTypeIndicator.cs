using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTypeIndicator : MonoBehaviour
{
    private SpriteRenderer spr;
    
    public Sprite Rock;
    public Sprite Paper;
    public Sprite Scissors;
    
    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
        setAlpha(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FixedUpdate()
    {
        setAlpha(spr.color.a - 0.02f);
    }

    public void Appear(FighterMove.MoveType type)
    {
        
        setAlpha(1);

        
        switch (type)
        {
            case FighterMove.MoveType.Paper: spr.sprite = Paper; return;
            case FighterMove.MoveType.Rock: spr.sprite = Rock; return;
            case FighterMove.MoveType.Scissor: spr.sprite = Scissors; return;
        }
        
        
    }

    public void setAlpha(float newAlpha)
    {
        Color c = spr.color;
        c.a = newAlpha;
        spr.color = c;
    }
}
