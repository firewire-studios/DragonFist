using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    public int xSpeed = 0;
    

    /**
     * Set in editor
     */
    [SerializeField]
    public int team;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void FixedTimestepUpdate()
    {
        transform.position += new Vector3(xSpeed,0,0);
        xSpeed = 0;
    }
}
