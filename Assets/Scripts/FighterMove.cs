using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMove
{
    public string MoveName;
    public List<Fighter.Action> Actions;
    public int Priority;
    public MoveType type = MoveType.Rock;

    
    // The following are animation frames no fixedUpdateFrames
    
    public int idleFrames = 0;
    public int hurtFrames = 0;
    public int coolDownFrames = 0;
    public int frameInterval = 3;
    public bool standingMove = true;
    public GameObject hurtbox = null;
    public int pushFrames = 6;
    public bool shouldLaunch = false;
    public int dmg = 1;

    public List<Sprite> sprites;
    
    public FighterMove(
        string name,
        List<Fighter.Action> actions,
        int priority
    )
    {
        sprites = new List<Sprite>();
        MoveName = name;
        Actions = actions;
        Priority = priority;
    }

    public void SetSprites(List<Sprite> _sprites)
    {
        sprites = _sprites;
    }

    public bool IsIdleFrame(int frame)
    {
        return frame < idleFrames;
    }

    public bool IsHurtFrame(int frame)
    {
        return frame >= idleFrames && frame < (idleFrames + hurtFrames);
    }

    public bool IsCooldownFrame(int frame)
    {
        return frame >= (idleFrames + hurtFrames) && frame < (idleFrames + hurtFrames + coolDownFrames);
    }

    public enum MoveType
    {
        Rock,
        Paper,
        Scissor
    }

    public bool Beats(FighterMove move)
    {
        MoveType type1 = type;
        MoveType type2 = move.type;


        if (type1 == type2)
        {
            return false;
        }

        if (type1 == MoveType.Rock && type2 == MoveType.Scissor)
        {
            return true;
        }
        
        if (type1 == MoveType.Scissor && type2 == MoveType.Paper)
        {
            return true;
        }
        
        if (type1 == MoveType.Paper && type2 == MoveType.Rock)
        {
            return true;
        }
        

        return false;

    }
}
