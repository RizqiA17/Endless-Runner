using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private ClassCaller callClass;
    private GameObject[] level = null;
    private float delay;
    public int totalLevelLenght;

    private void Awake()
    {
        InisiationLevel();
        SetLevel();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetLevel()
    {
        int pos = 0;
        for (int i = 0; i < totalLevelLenght; i++)
        {
            Instantiate(level[Random.Range(0, level.Length - 1)], new Vector3(pos, 0, 0), Quaternion.Euler(Vector3.zero));
            pos += 20;
        }
        //callClass.GameManager.isStart = true; 
    }

    private void InisiationLevel()
    {
        object[] level = Resources.LoadAll("Level");
        this.level = new GameObject[level.Length];
        for (int i = 0; i < level.Length; i++)
        {
            this.level[i] = (GameObject)level[i];
        }
    }

    private void AddLevel(float position)
    {
        Instantiate(level[Random.Range(0, level.Length)], new Vector3(position, 0, 0), Quaternion.Euler(Vector3.zero));
        delay = 0;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Level"))
        {
            delay += callClass.GameManager.levelSpeed / Time.deltaTime;
            AddLevel(other.transform.position.x + 20 * totalLevelLenght - delay);
            Destroy(other.gameObject);
        }
    }
}
