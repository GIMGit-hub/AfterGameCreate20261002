using UnityEngine;

public class SlotHole : MonoBehaviour
{
    // スロット側がこれに登録して、実行のきっかけにする
    public static event System.Action OnSlotTriggered;

    [SerializeField] bool destroyCoin = true; // 穴に入ったコインを消すか

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin")) return;

        // 二重反応防止(複数Colliderや他のTriggerとの重複対策)
        other.tag = "Untagged";

        if (destroyCoin) Destroy(other.gameObject);

        Debug.Log("スロット穴に入った");
        OnSlotTriggered?.Invoke();
    }
}