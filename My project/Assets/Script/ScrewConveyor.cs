using System.Collections.Generic;
using UnityEngine;

public class ScrewConveyor : MonoBehaviour
{
    [SerializeField] Transform screwVisual;                 // 回転させる見た目
    [SerializeField] Vector3 visualAxis = Vector3.forward;  // 見た目の回転軸(スクリューのローカル軸)
    [SerializeField] float rpm = 90f;                       // 見た目の回転速度(逆回しなら負の値)
    [SerializeField] float speed = 1.5f;                    // ボールを運ぶ最高速度
    [SerializeField] float acceleration = 8f;               // 加速の強さ
    [SerializeField] bool active = true;
    [SerializeField] Vector3 localDirection = Vector3.forward;

    // OnTriggerStayは眠ったRigidbodyに呼ばれないので、中にいるボールを自前で管理する
    readonly HashSet<Rigidbody> balls = new();

    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponentInParent<SlotBall>();
        if (ball != null) balls.Add(ball.GetComponent<Rigidbody>());
    }

    void OnTriggerExit(Collider other)
    {
        var ball = other.GetComponentInParent<SlotBall>();
        if (ball != null) balls.Remove(ball.GetComponent<Rigidbody>());
    }

    void FixedUpdate()
    {
        balls.RemoveWhere(r => r == null); // 破棄されたボールを除く
        if (!active) return;

        Vector3 dir = transform.TransformDirection(localDirection.normalized); // 青い矢印の向きに運ぶ
        foreach (var rb in balls)
        {
            rb.WakeUp();
            if (Vector3.Dot(rb.linearVelocity, dir) < speed)
                rb.AddForce(dir * acceleration, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (active && screwVisual != null)
            screwVisual.Rotate(visualAxis, rpm * 6f * Time.deltaTime, Space.Self); // rpm→度/秒
    }

    public void SetActive(bool value) => active = value;
}