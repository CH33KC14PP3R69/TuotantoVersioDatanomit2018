using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float speed = 5;
    public float pitch = 3;
    public Vector3 offset = new Vector3(0f, -4.5f, 4f);

    void FixedUpdate()
    {
        // Look
        var newRotation = Quaternion.LookRotation(target.transform.position - transform.position) * Quaternion.Euler(pitch, 0, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, speed * Time.deltaTime);

        // Move
        Vector3 newPosition = target.transform.position - target.transform.forward * offset.z - target.transform.up * offset.y;
        transform.position = Vector3.Slerp(transform.position, newPosition, Time.deltaTime * speed);
    }
}
