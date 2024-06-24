using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterMove
{
    public string MoveName;
    public List<Fighter.Action> Actions;
    public int Priority;
    
    public FighterMove(
        string name,
        List<Fighter.Action> actions,
        int priority
    )
    {
        MoveName = name;
        Actions = actions;
        Priority = priority;
    }
}
