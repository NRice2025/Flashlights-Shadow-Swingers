using NUnit.Framework.Constraints;
using UnityEngine;

public class Grapple : MonoBehaviour
{

    public Vector3 directionVector;
    private Rigidbody2D body;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        body.linearVelocity = directionVector;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
