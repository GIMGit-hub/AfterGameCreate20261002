using System.Collections;
using TMPro;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [Header("表示")]
    [SerializeField] TMP_Text[] reels = new TMP_Text[3]; // 3つの数字(Ghostは入れない)
    [SerializeField] bool alwaysVisible = true;           // オンなら出入りしない
    [SerializeField] Vector3 hiddenOffset = new Vector3(0f, -300f, 0f); // alwaysVisibleオフのときだけ使う
    [SerializeField] float slideTime = 0.4f;

    [Header("色")]
    [SerializeField] Color idleColor = new Color(1f, 0.62f, 0.1f, 0.45f); // 待機中(暗め)
    [SerializeField] Color spinColor = new Color(1f, 0.62f, 0.1f, 1f);    // 回転中
    [SerializeField] Color hitColor = new Color(1f, 0.95f, 0.7f, 1f);    // 当たりの点滅

    [Header("回転")]
    [SerializeField] float spinTime = 1.5f;
    [SerializeField] float stopGap = 0.5f;
    [SerializeField] float tickInterval = 0.06f;
    [SerializeField] float resultHold = 1.2f;

    [Header("抽選")]
    [SerializeField, Range(0f, 1f)] float hitChance = 0.15f;
    [SerializeField] int payoutPerNumber = 10;
    [SerializeField] bool launchBallAfterSpin = true;

    Vector3 shownPos, hiddenPos;
    int pending;
    Coroutine routine;

    void Awake()
    {
        shownPos = transform.localPosition;
        hiddenPos = shownPos + hiddenOffset;
        transform.localPosition = alwaysVisible ? shownPos : hiddenPos;

        foreach (var r in reels) { r.text = "0"; r.color = idleColor; }
    }

    void OnEnable() => SlotHole.OnSlotTriggered += Request;
    void OnDisable() => SlotHole.OnSlotTriggered -= Request;

    void Request()
    {
        pending++;
        routine ??= StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        while (pending > 0)
        {
            if (!alwaysVisible) yield return Slide(shownPos);
            while (pending > 0)
            {
                pending--;
                yield return Spin();
            }
            if (!alwaysVisible) yield return Slide(hiddenPos);
        }
        routine = null;
    }

    IEnumerator Slide(Vector3 to)
    {
        Vector3 from = transform.localPosition;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / slideTime;
            transform.localPosition = Vector3.Lerp(from, to, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }
        transform.localPosition = to;
    }

    void SetColor(Color c) { foreach (var r in reels) r.color = c; }

    IEnumerator Spin()
    {
        int[] result = Decide();
        float[] stopAt = { spinTime, spinTime + stopGap, spinTime + stopGap * 2f };
        float t = 0f, tick = 0f;

        SetColor(spinColor);

        while (t < stopAt[2])
        {
            tick -= Time.deltaTime;
            if (tick <= 0f)
            {
                tick = tickInterval;
                for (int i = 0; i < reels.Length; i++)
                    reels[i].text = (t < stopAt[i] ? Random.Range(1, 10) : result[i]).ToString();
            }
            t += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < reels.Length; i++) reels[i].text = result[i].ToString();

        bool hit = result[0] == result[1] && result[1] == result[2];
        if (hit && CoinEventSpawner.Instance != null)
            CoinEventSpawner.Instance.Spawn(result[0] * payoutPerNumber);

        if (launchBallAfterSpin && SlotBallManager.Instance != null)
            SlotBallManager.Instance.LaunchRandom();

        if (hit)
        {
            for (int i = 0; i < 6; i++) // 当たりの点滅
            {
                SetColor(i % 2 == 0 ? hitColor : spinColor);
                yield return new WaitForSeconds(0.12f);
            }
        }

        yield return new WaitForSeconds(resultHold);
        SetColor(idleColor);
    }

    int[] Decide()
    {
        if (Random.value < hitChance)
        {
            int n = Random.Range(1, 10);
            return new[] { n, n, n };
        }
        int a = Random.Range(1, 10), b = Random.Range(1, 10), c = Random.Range(1, 10);
        while (a == b && b == c) c = Random.Range(1, 10);
        return new[] { a, b, c };
    }
}