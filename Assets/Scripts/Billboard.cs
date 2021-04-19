using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    void Awake()
    {
        if (cam == null)
        {
            cam = GameObject.Find("Main Camera").transform;
        }
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
