using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private ClassCaller callClass;
    private CharacterController moveController;
    //[HideInInspector]
    public bool isStart;

    // Start is called before the first frame update
    void Start()
    {
        callClass = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClassCaller>();
        moveController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isStart = callClass.GameManager.isStart;
        if(isStart) Move();
    }

    private void Move()
    {
        moveController.Move(new Vector3(-callClass.GameManager.levelSpeed, 0, 0));
    }
}
