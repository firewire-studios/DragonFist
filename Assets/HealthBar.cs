using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject Greenbar;
    private RectTransform _rectTransform;

    public GameObject win1;
    public GameObject win2;
    public GameObject win3;
    
    // Start is called before the first frame update
    void Awake()
    {
        _rectTransform = Greenbar.GetComponent<RectTransform>();
        enableWins(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int health)
    {
        
        float scale = (float) health / 100;
        if (scale <= 0)
        {
            scale = 0;
        }
        _rectTransform.localScale = new Vector3(scale,1,1);
        
    }

    public void enableWins(int count)
    {
        Image spr1 = win1.GetComponent<Image>();
        Image spr2 = win2.GetComponent<Image>();
        Image spr3 = win3.GetComponent<Image>();
        
        if (count <= 0)
        {
            spr1.enabled = false;
            spr2.enabled = false;
            spr3.enabled = false;
        }
        
        if (count == 1)
        {
            spr1.enabled = true;
            spr2.enabled = false;
            spr3.enabled = false;
        }
        
        if (count == 2)
        {
            spr1.enabled = true;
            spr2.enabled = true;
            spr3.enabled = false;
        }
        
        if (count >= 3)
        {
            spr1.enabled = true;
            spr2.enabled = true;
            spr3.enabled = true;
        }
    }
}
