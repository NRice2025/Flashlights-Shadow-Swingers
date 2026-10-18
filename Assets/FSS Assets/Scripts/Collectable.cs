using UnityEngine;

public class Collectable : MonoBehaviour
{
    GameObject player;
    PlayerController controller;

    void Start()
    {
        player = GameObject.Find("Player");
        controller = player.GetComponent<PlayerController>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            controller.CollectSurvivor();
            Destroy(gameObject);
        }
    }
}
