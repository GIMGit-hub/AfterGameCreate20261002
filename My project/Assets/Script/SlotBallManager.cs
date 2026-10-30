using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotBallManager : MonoBehaviour
{
    public static SlotBallManager Instance { get; private set; }

    public static event System.Action<int> OnYellowMultiplierChanged;
    public static event System.Action<int> OnJpStepChanged;
    public static event System.Action OnJpChance; // JPチャンス抽選(後で作る)の入口

    [Header("排出")]
    [SerializeField] GameObject ballPrefab;
    [SerializeField] Transform ballSpawnPoint;
    [SerializeField] float spawnInterval = 0.3f;
    [SerializeField] int maxAliveBalls = 30; // 緑の連鎖で増えすぎないための上限

    [Header("色の抽選の重み")]
    [SerializeField] float yellowWeight = 1f;
    [SerializeField] float pinkWeight = 1f;
    [SerializeField] float greenWeight = 1f;

    [Header("黄色")]
    [SerializeField] int yellowBaseCoins = 20;
    [SerializeField] int yellowMaxMultiplier = 10;

    [Header("ピンク")]
    [SerializeField] int jpStepsRequired = 5;

    public int YellowMultiplier { get; private set; }
    public int JpSteps { get; private set; }

    readonly Queue<BallColor> queue = new();
    Coroutine routine;

    void Awake() => Instance = this;

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void LaunchRandom() => Launch(RandomColor());

    public void Launch(BallColor kind)
    {
        queue.Enqueue(kind);
        routine ??= StartCoroutine(SpawnRoutine());
    }

    BallColor RandomColor()
    {
        float r = Random.value * (yellowWeight + pinkWeight + greenWeight);
        if (r < yellowWeight) return BallColor.Yellow;
        if (r < yellowWeight + pinkWeight) return BallColor.Pink;
        return BallColor.Green;
    }

    IEnumerator SpawnRoutine()
    {
        var wait = new WaitForSeconds(spawnInterval);
        while (queue.Count > 0)
        {
            while (SlotBall.AliveCount >= maxAliveBalls) yield return null;

            var kind = queue.Dequeue();
            var pos = ballSpawnPoint.position + Random.insideUnitSphere * 0.05f;
            var go = Instantiate(ballPrefab, pos, Random.rotation);
            if (go.TryGetComponent<SlotBall>(out var ball)) ball.Setup(kind);

            yield return wait;
        }
        routine = null;
    }

    // ボールがゴールに落ちたときに呼ばれる
    public void OnBallGoal(SlotBall ball)
    {
        if (ball.Consumed) return;
        ball.Consumed = true;

        switch (ball.Kind)
        {
            case BallColor.Yellow:
                YellowMultiplier = Mathf.Min(YellowMultiplier + 1, yellowMaxMultiplier);
                OnYellowMultiplierChanged?.Invoke(YellowMultiplier);
                if (CoinEventSpawner.Instance != null)
                    CoinEventSpawner.Instance.Spawn(yellowBaseCoins * YellowMultiplier);
                break;

            case BallColor.Pink:
                JpSteps++;
                OnJpStepChanged?.Invoke(JpSteps);
                if (JpSteps >= jpStepsRequired)
                {
                    OnJpChance?.Invoke();
                    JpSteps = 0;
                    OnJpStepChanged?.Invoke(JpSteps);
                }
                break;

            case BallColor.Green:
                int n = Random.Range(1, 4); // 1〜3個
                for (int i = 0; i < n; i++) Launch(RandomColor());
                break;
        }

        Destroy(ball.gameObject);
    }
}