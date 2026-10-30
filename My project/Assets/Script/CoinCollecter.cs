using UnityEngine;

public class CoinCollector : MonoBehaviour
{
    public static int CoinCount { get; private set; }
    public static event System.Action<int> OnCoinCountChanged;

    // 手持ち枚数を指定の値にする(開始時の初期化用)
    public static void SetCount(int n)
    {
        CoinCount = Mathf.Max(0, n);
        OnCoinCountChanged?.Invoke(CoinCount);
    }

    // 1枚消費する。足りなければfalse
    public static bool TryConsume()
    {
        if (CoinCount <= 0) return false;
        CoinCount--;
        OnCoinCountChanged?.Invoke(CoinCount);
        return true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin")) return;

        // 複数Colliderでの二重カウント防止
        other.tag = "Untagged";

        CoinCount++;
        OnCoinCountChanged?.Invoke(CoinCount);
        Debug.Log(CoinCount);
        Destroy(other.gameObject);
    }
}