using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public GameObject Greenbar;
    private RectTransform _rectTransform;
    
    // Start is called before the first frame update
    void Awake()
    {
        _rectTransform = Greenbar.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealth(int health)
    {
        
        float scale = (float) health / 100;
        _rectTransform.localScale = new Vector3(scale,1,1);
        
    }
}
