using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvent : MonoBehaviour
{
    [SerializeField] private ClassCaller callClass;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EndSliding()
    {
        callClass.PlayerMovement.isSliding = false;
        callClass.PlayerMovement.DefaultHeight();
    }

    void EndJump()
    {
        callClass.PlayerMovement.isJump = false;
    }
}
