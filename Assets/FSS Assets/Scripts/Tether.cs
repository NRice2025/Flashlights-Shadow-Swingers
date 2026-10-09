using System;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class Tether : MonoBehaviour
{
    public GameObject owner;
    public GameObject hook;
    public GameObject tetherPrefab;

    public Collider2D collider;
    private GameObject tetherChild = null;

    public bool isRootTether = false;

    private bool isOnHook = false;

    Vector3 unitDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (hook.IsUnityNull())
        {
            //Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isRootTether)
        {
            transform.position = owner.transform.position;
        }

        unitDirection = (hook.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, unitDirection);

        if (!tetherChild.IsUnityNull())
        {
            Vector3 pos = transform.position;
            pos = unitDirection * 0.31f + pos;
            tetherChild.transform.position = pos;
        }

        if (Mathf.Abs((hook.transform.position - transform.position).magnitude) > 0.31f && tetherChild.IsUnityNull())
        {
            tetherChild = Instantiate(tetherPrefab) as GameObject;
            tetherChild.GetComponent<Tether>().hook = hook;
            tetherChild.GetComponent<Tether>().owner = owner;
        }
        if (Mathf.Abs((hook.transform.position - transform.position).magnitude) < 0.31f && !tetherChild.IsUnityNull())
        {
            tetherChild.GetComponent<Tether>().destroyAll();
        }
    }

    public void destroyAll()
    {
        if (!tetherChild.IsUnityNull())
        {
            tetherChild.GetComponent<Tether>().destroyAll();
        }
        Destroy(gameObject);
    }
}
