using System.Collections;
using TMPro;
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    [Header("表示")]
    [SerializeField] TMP_Text[] reels = new TMP_Text[3]; // 3つの数字
    [SerializeField] Vector3 hiddenOffset = new Vector3(0f, -300f, 0f); // 引っ込む方向と距離
    [SerializeField] float slideTime = 0.4f;

    [Header("回転")]
    [SerializeField] float spinTime = 1.5f;     // 1つ目が止まるまで
    [SerializeField] float stopGap = 0.5f;      // 次のリールが止まるまでの間隔
    [SerializeField] float tickInterval = 0.06f;
    [SerializeField] float resultHold = 1.2f;   // 結果を見せる時間

    [Header("抽選")]
    [SerializeField, Range(0f, 1f)] float hitChance = 0.15f; // 揃う確率
    [SerializeField] int payoutPerNumber = 10;
    [SerializeField] bool launchBallAfterSpin = true; // 回すたびにボールを1個排出

    Vector3 shownPos, hiddenPos;
    int pending;
    Coroutine routine;

    void Awake()
    {
        shownPos = transform.localPosition;
        hiddenPos = shownPos + hiddenOffset;
        transform.localPosition = hiddenPos; // 最初は引っ込んだ状態
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
            yield return Slide(shownPos);          // 出てくる
            while (pending > 0)                    // 溜まっている分を連続で回す
            {
                pending--;
                yield return Spin();
            }
            yield return Slide(hiddenPos);         // 引っ込む
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

    IEnumerator Spin()
    {
        int[] result = Decide();
        float[] stopAt = { spinTime, spinTime + stopGap, spinTime + stopGap * 2f };
        float t = 0f, tick = 0f;

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

        yield return new WaitForSeconds(resultHold);
    }

    int[] Decide()
    {
        if (Random.value < hitChance)
        {
            int n = Random.Range(1, 10);
            return new[] { n, n, n };
        }
        int a = Random.Range(1, 10), b = Random.Range(1, 10), c = Random.Range(1, 10);
        while (a == b && b == c) c = Random.Range(1, 10); // ハズレのとき揃わないようにする
        return new[] { a, b, c };
    }
}