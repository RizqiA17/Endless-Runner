using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class GameManager : MonoBehaviour
{
    [SerializeField] private ClassCaller callClass;
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedUpSensivity;
    [HideInInspector] public float levelSpeed;
    //[HideInInspector] 
    public bool isStart;

    // Start is called before the first frame update
    void Start()
    {
        callClass = GameObject.FindGameObjectWithTag("GameController").GetComponent<ClassCaller>();
        levelSpeed = movementSpeed;
    }

    private void Update()
    {
        SpeedUp();
    }

    private void SpeedUp()
    {
        if (levelSpeed < maxSpeed) levelSpeed = Utility.SmoothTransitionFloat(levelSpeed, levelSpeed + 1, speedUpSensivity * 0.001f);
        else levelSpeed = maxSpeed;
    }

    public void GameOver() { 
        gameOverCanvas.SetActive(true);
    }
}
