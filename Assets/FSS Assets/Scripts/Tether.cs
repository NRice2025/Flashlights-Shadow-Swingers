using System;
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isRootTether)
        {
            transform.position = owner.transform.position;
        }

        Vector3 unitDirection = (hook.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, unitDirection);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (tetherChild.IsUnityNull())
        {
            tetherChild = Instantiate(tetherPrefab) as GameObject;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!tetherChild.IsUnityNull())
        {
            //Destroy(tetherChild);
        }
    }

    public void destroyAll()
    {
        if (!tetherChild.IsUnityNull())
        {
            tetherChild.GetComponent<Tether>().destroyAll();
        }
        Destroy(this);
    }
}
