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
    
    [SerializeField]
    private List<Sprite> sprList;
    
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (frameTime > 3)
        {
            if (frame >= sprList.Count)
            {
                Destroy(gameObject);
                return;
            }

            _spriteRenderer.sprite = sprList[frame];
            frame++;
            frameTime = 0;
        }
        
        frameTime++;
        
    }
}
