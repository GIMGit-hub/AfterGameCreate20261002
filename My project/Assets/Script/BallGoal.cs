using UnityEngine;

public class BallGoal : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponentInParent<SlotBall>();
        if (ball == null) return;
        if (SlotBallManager.Instance != null) SlotBallManager.Instance.OnBallGoal(ball);
    }
}