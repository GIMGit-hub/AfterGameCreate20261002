using UnityEngine;

public class CoinDestroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Coin")) return;
        Destroy(other.gameObject);
        Debug.Log("coinè¡ÇµÇΩ");
    }
}
