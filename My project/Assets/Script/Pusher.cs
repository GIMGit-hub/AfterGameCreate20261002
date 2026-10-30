using System.Data;
using UnityEngine;

public class Pusher : MonoBehaviour
{
    [Header("PusherObj")]
    [SerializeField] float distance = 1.5f;
    [SerializeField] float speed = 0.5f;

    Rigidbody rb;
    Vector3 startPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        Vector3 target = startPos + transform.forward * distance * t;
        rb.MovePosition(target);
    }
}
