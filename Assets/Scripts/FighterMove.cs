using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMove
{
    public string MoveName;
    public List<Fighter.Action> Actions;
    public int Priority;

    
    // The following are animation frames no fixedUpdateFrames
    
    public int idleFrames = 0;
    public int hurtFrames = 0;
    public int coolDownFrames = 0;
    public int frameInterval = 3;
    public bool standingMove = true;
    public GameObject hurtbox = null;
    public int pushFrames = 6;
    public bool shouldLaunch = false;

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
}
