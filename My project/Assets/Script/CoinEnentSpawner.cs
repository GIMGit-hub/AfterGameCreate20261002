using System.Collections;
using UnityEngine;

public class CoinEventSpawner : MonoBehaviour
{
    public static CoinEventSpawner Instance { get; private set; }

    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform spawnPoint;      // IventSpawnPoint
    [SerializeField] float spreadRadius = 0.3f; // 排出位置のばらつき
    [SerializeField] float interval = 0.05f;    // 1枚ごとの間隔(秒)
    [SerializeField] int initialCoins = 100;    // 開始時に出す枚数(仮)

    int pending;
    Coroutine routine;

    void Awake() => Instance = this;

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (initialCoins > 0) Spawn(initialCoins);
    }

    // 外部(イベント)から呼ぶ。排出中に呼ばれても枚数が足されて続けて出る
    public void Spawn(int count)
    {
        if (count <= 0) return;
        pending += count;
        routine ??= StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        var wait = new WaitForSeconds(interval);
        while (pending > 0)
        {
            SpawnOne();
            pending--;
            yield return wait;
        }
        routine = null;
    }

    void SpawnOne()
    {
        Vector2 r = Random.insideUnitCircle * spreadRadius;
        Vector3 pos = spawnPoint.position + new Vector3(r.x, 0f, r.y);
        Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        Instantiate(coinPrefab, pos, rot);
    }
}