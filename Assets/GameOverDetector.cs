using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverDetector : MonoBehaviour
{
    [SerializeField] private LayerMask obstacleLayer;
    private ClassCaller callClass;
    // Start is called before the first frame update
    void Start()
    {
        callClass = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClassCaller>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == obstacleLayer.value - 1)
        {
            callClass.PlayerMovement.Dead();
        }
    }
}
