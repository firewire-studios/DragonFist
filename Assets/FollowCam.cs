using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera cam;
    
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newPosition = cam.transform.position;
        newPosition.z = transform.position.z;
        newPosition.x /= 8;
        newPosition.x *= -1;
        
        //transform.position = Vector3.Lerp(transform.position, newPosition, 0.01f);
        transform.position = newPosition;
    }
}
