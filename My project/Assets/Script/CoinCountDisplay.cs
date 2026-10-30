using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CoinCountDisplay : MonoBehaviour
{
    [SerializeField] string format = "{0}"; // —á: "COIN {0}" / "{0} –‡"

    TMP_Text label;

    void Awake() => label = GetComponent<TMP_Text>();

    void OnEnable()
    {
        CoinCollector.OnCoinCountChanged += Refresh;
        Refresh(CoinCollector.CoinCount); // —LŒø‚É‚È‚Á‚½Žž“_‚Ì–‡”‚ð‘¦•\Ž¦
    }

    void OnDisable() => CoinCollector.OnCoinCountChanged -= Refresh;

    void Refresh(int count) => label.text = string.Format(format, count);
}