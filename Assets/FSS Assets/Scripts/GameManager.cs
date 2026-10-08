using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject wirePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.Intersects(wirePrefab))
        {
            player.GetComponent<PlayerController>().Death();
        }
    }
}
