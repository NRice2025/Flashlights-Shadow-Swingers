using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.UIElements;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]

public class PlayerController : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public float airResistance = 2.0f;
    public float accelerationFriction = 4.0f;
    public Camera mainCamera;
    public GameObject flashlight;
    public GameObject grapplePrefab;
    public GameObject tetherPrefab;
    public float hookedVelocityDecay;
    public float grappleMoveReduction = 0.02f;
    public float reelVelocity = 0.2f;

    public GameObject spawnPoint;
    private GameObject grapple;

    private GameObject tether;
    private float grappleDistance;

    public bool enableDebug;
    bool facingRight = true;
    float moveDirection = 0;
    bool isGrounded = false;

    public bool isFlashLightOn = true;
    Vector3 cameraPos;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;
    

    // Use this for initialization
    void Start()
    {
        Spawn(spawnPoint.transform.position);
        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;

        if (mainCamera)
        {
            cameraPos = mainCamera.transform.position;
        }
    }

     public void Spawn(Vector3 spawnPosition) 
    {
        transform.position = spawnPosition;
    }
    public void Death(Vector3 spawnPosition)
    {
        Spawn(spawnPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
        

        // Handle swing physics somewhat
        if (!grapple.IsUnityNull() && grapple.GetComponent<Rigidbody2D>().IsUnityNull())
        {
            Vector2 relativeGrapplePosition = transform.position - grapple.transform.position;
            grappleDistance = Mathf.Min((relativeGrapplePosition).magnitude,grappleDistance);
            r2d.linearVelocity = (r2d.linearVelocity - Mathf.Max(0f,Vector2.Dot(r2d.linearVelocity, relativeGrapplePosition.normalized)) * relativeGrapplePosition.normalized);
            transform.position = relativeGrapplePosition.normalized * grappleDistance + new Vector2(grapple.transform.position.x,grapple.transform.position.y);
            r2d.linearVelocity *= hookedVelocityDecay;
            
            //Debug.Log(relativeGrapplePosition);
            Debug.Log(r2d.linearVelocity);
            //Debug.Log(Vector2.Dot(r2d.linearVelocity, relativeGrapplePosition));
        } else
        {
            grappleDistance = int.MaxValue;
        }

        // Grapple Shoot (Hold G or Left Mouse Button)

        if ((Input.GetKeyDown(KeyCode.G) || Input.GetMouseButtonDown(0)) && grapple.IsUnityNull())
        {
            grapple = Instantiate(grapplePrefab) as GameObject;
            Vector3 pos = transform.position;
            pos.y = pos.y + 0.3f;
            grapple.GetComponent<Grapple>().directionVector = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            pos.z = -1;
            grapple.transform.position = pos;

            //canGrapple = false;
            //Invoke("allowGrapple", 1f);
        }

        // Reel In (Hold R)
        if (Input.GetKey(KeyCode.R) && !grapple.IsUnityNull() && grapple.GetComponent<Rigidbody2D>().IsUnityNull())
        {
            grappleDistance = Mathf.Min((grapple.transform.position - transform.position).magnitude, grappleDistance);
            Vector2 v = (grapple.transform.position - transform.position).normalized * reelVelocity;
            r2d.linearVelocity = v + r2d.linearVelocity * 0.5f;
        }
        // Destroy Grapple (Release G or Left Mouse Button)
        if ((Input.GetKeyUp(KeyCode.G) || Input.GetMouseButtonUp(0)) && !grapple.IsUnityNull())
        {
            Destroy(grapple);
        }

        // Handle Tether
        if (!grapple.IsUnityNull() && tether.IsUnityNull())
        {
            tether = Instantiate(tetherPrefab) as GameObject;
            tether.GetComponent<Tether>().owner = this.GameObject();
            tether.GetComponent<Tether>().hook = grapple;
            Vector3 pos = transform.position;
            pos.y += 0.3f;
            tether.transform.position = pos;
        }
        if (!tether.IsUnityNull())
        {
            Vector3 pos = transform.position;
            pos.y += 0.3f;
            tether.transform.position = pos;
            if (grapple.IsUnityNull())
            {
                tether.GetComponent<Tether>().destroyAll();
            }
        }

        // Movement controls
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            moveDirection = Input.GetKey(KeyCode.A) ? -1 : 1;
        }
        else
        {
            moveDirection = 0;
        }

        // Change facing direction
        if (moveDirection != 0)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(Mathf.Abs(t.localScale.x), t.localScale.y, transform.localScale.z);
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(-Mathf.Abs(t.localScale.x), t.localScale.y, t.localScale.z);
            }
        }

         // Toggle flashlight
        if (Input.GetKeyDown(KeyCode.F))
        {
            isFlashLightOn = !isFlashLightOn;
            flashlight.SetActive(isFlashLightOn);
        }

        // Jumping
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            r2d.linearVelocity = new Vector2(r2d.linearVelocityX, jumpHeight);
        }

        // Camera follow
        if (mainCamera)
        {
            mainCamera.transform.position = new Vector3(t.position.x, t.position.y + 2.0f, cameraPos.z);
        }

        // Death
        if (transform.position.y < -20f)
        {
            Death(spawnPoint.transform.position);
        }


    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wire")
        {
            Death(spawnPoint.transform.position);
        }
    }
    void FixedUpdate()
    {
        //Vector3 vec = r2d.linearVelocity;
        //vec.y -= gravityScale * 0.1f;
        //r2d.linearVelocity = vec;

        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos = colliderBounds.min + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider && colliders[i].tag != "NotGround")
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        // Apply movement velocity

        // Grounded Movement
        if (isGrounded)
        {
            r2d.linearVelocity = new Vector2((moveDirection) * maxSpeed, r2d.linearVelocityY);
        } else if (grapple.IsUnityNull() || !grapple.GetComponent<Rigidbody2D>().IsUnityNull()) // Air Movement
        {
            var v = r2d.linearVelocityX;
            v = v * moveDirection < maxSpeed ? (moveDirection * maxSpeed / accelerationFriction) + v : v;
            //v = Mathf.Clamp((moveDirection * maxSpeed / airResistance) + v, -maxSpeed, maxSpeed);
            v *= airResistance;
            r2d.linearVelocity = new Vector2(v, r2d.linearVelocityY);
        } else
        {
            r2d.linearVelocityX += moveDirection * maxSpeed * grappleMoveReduction;
            r2d.linearVelocityX *= airResistance;
        }
        

        // Simple debug
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(0, colliderRadius, 0), isGrounded ? Color.green : Color.red);
        Debug.DrawLine(groundCheckPos, groundCheckPos - new Vector3(colliderRadius, 0, 0), isGrounded ? Color.green : Color.red);
    }
}
