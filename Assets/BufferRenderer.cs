using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BufferRenderer : MonoBehaviour
{

    public Texture down;
    public Texture left;
    public Texture right;
    public Texture up;
    
    public Texture x;
    public Texture a;
    public Texture b;
    public Texture y;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        
        // Draw Buffer for fighter 1
        int x = 0;
        foreach (var bufferedInput in GameController.instance.p1.buffer)
        {
            
            Rect position = new Rect(x * 64,0,64,64);
            Texture text = mapActionToTexture(bufferedInput.action);
            
            GUI.DrawTexture(
                position,
                text
            );
            x++;
        }
        
    }

    private Texture mapActionToTexture(Fighter.Action action)
    {
        switch (action)
        {
            case Fighter.Action.Backward: return left;
            case Fighter.Action.Forward: return right;
            case Fighter.Action.Down: return down;
            case Fighter.Action.Jump: return up;
            
            case Fighter.Action.Rock: return a;
            case Fighter.Action.Paper: return x;
            case Fighter.Action.Scissor: return y;
            case Fighter.Action.Block: return b;
        }

        return b;
    }
}
