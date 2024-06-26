using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObj : MonoBehaviour
{
    // Start is called before the first frame update

    private SpriteRenderer _spriteRenderer;
    
    public Sprite spr1;
    public Sprite spr2;
    public Sprite spr3;


    private int frame = 0;
    private int frameTime = 0;
    
    private List<Sprite> sprList;
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        sprList = new List<Sprite>()
        {
            spr1,
            spr2,
            spr3,
        };
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (frameTime > 4)
        {
            frame = frame + 1 >= sprList.Count ? 0 : frame + 1;
            //_spriteRenderer.sprite = sprList[frame];
            frameTime = 0;
        }
        
        frameTime++;
        
    }
}
