using TMPro;
using UnityEngine;

public class FactoryDisplay : MonoBehaviour
{
    [SerializeField] TMP_Text main;     // 点灯する数字
    [SerializeField] TMP_Text ghost;    // 背面の「8」(消灯セグメント用。なくても動く)
    [SerializeField] int digits = 4;
    [SerializeField] bool bindToCoinCount = true; // 手持ち枚数に連動させる
    [SerializeField] Color litColor = new Color(1f, 0.62f, 0.1f);
    [SerializeField] Color ghostColor = new Color(1f, 0.62f, 0.1f, 0.12f);
    [SerializeField] Color flashColor = new Color(1f, 0.95f, 0.7f);
    [SerializeField] float countSpeed = 60f;  // 1秒あたりに進む数
    [SerializeField] float flashTime = 0.25f;

    float shown, target, flash;

    void Awake()
    {
        main.color = litColor;
        if (ghost != null)
        {
            ghost.text = new string('8', digits);
            ghost.color = ghostColor;
        }
    }

    void OnEnable()
    {
        if (!bindToCoinCount) return;
        CoinCollector.OnCoinCountChanged += SetValue;
        Set(CoinCollector.CoinCount, true);
    }

    void OnDisable()
    {
        if (bindToCoinCount) CoinCollector.OnCoinCountChanged -= SetValue;
    }

    // 倍率やJPステップなど、他の表示からも呼べる
    public void SetValue(int value) => Set(value, false);

    void Set(int value, bool instant)
    {
        if (!instant && value != target) flash = flashTime;
        target = value;
        if (instant) { shown = value; Render(); }
    }

    void Update()
    {
        if (!Mathf.Approximately(shown, target))
        {
            float step = Mathf.Max(countSpeed, Mathf.Abs(target - shown) * 4f) * Time.deltaTime;
            shown = Mathf.MoveTowards(shown, target, step);
            Render();
        }
        if (flash > 0f)
        {
            flash -= Time.deltaTime;
            main.color = Color.Lerp(litColor, flashColor, Mathf.Clamp01(flash / flashTime));
        }
    }

    void Render() => main.text = Mathf.RoundToInt(shown).ToString().PadLeft(digits);
}