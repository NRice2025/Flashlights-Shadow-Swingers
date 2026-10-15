using UnityEngine;

public class Collectable : MonoBehaviour
{
    public GameObject player;
    PlayerController controller;

    void Start()
    {
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
