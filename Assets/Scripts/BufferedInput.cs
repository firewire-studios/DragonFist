using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BufferedInput
{
    public Fighter.Action action;
    public int frames;

    public BufferedInput(Fighter.Action _action)
    {
        action = _action;
        frames = 0;
    }
    
}
