using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearAndFade : MonoBehaviour
{
    public SpriteRenderer spr;
    
    // Start is called before the first frame update
    void Start()
    {
        spr = GetComponent<SpriteRenderer>();
       spr.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (spr.enabled)
        {
            // Lower Alpha
            Color c = spr.color;
            c.a -= 0.02f;
            spr.color = c;
        }

        if (spr.color.a <= 0)
        {
            spr.enabled = false;
        }
    }

    public void Appear()
    {
        spr.enabled = true;
        Color c = spr.color;
        c.a = 1f;
        spr.color = c;
    }
}
