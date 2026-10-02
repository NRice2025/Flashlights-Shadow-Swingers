using UnityEngine;

public class MouseDebug : MonoBehaviour
{
    public Camera mainCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Input.mousePosition;
        pos = mainCamera.ScreenToWorldPoint(pos);
        pos.z = 0;
        transform.position = pos;
    }
}
