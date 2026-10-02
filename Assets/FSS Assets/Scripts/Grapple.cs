using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Grapple : MonoBehaviour
{
    public CircleCollider2D wallHitbox;
    public CircleCollider2D latchHitbox;
    public Vector3 directionVector;
    private Rigidbody2D body;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        directionVector.z = 0;
        body = GetComponent<Rigidbody2D>();
        body.linearVelocity = directionVector.normalized * 40;

        Invoke("despawnHook", 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void despawnHook()
    {
        if (body != null)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.otherCollider == wallHitbox) Destroy(this.gameObject);
        else Destroy(body);
    }
}
