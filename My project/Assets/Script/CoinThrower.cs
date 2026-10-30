using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CoinThrower : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab;
    [SerializeField] Transform spawnPoint;       // 手前の投入位置(フィールドより少し高く)
    [SerializeField] Transform aimSurface;       // 狙う高さの基準(Pusher上面あたり)
    [SerializeField] float throwSpeed = 3f;      // 前に飛ぶ速さ
    [SerializeField] float upSpeed = 1f;         // 上向きの初速(山なりにする)
    [SerializeField] float fireInterval = 0.15f; // 連射の間隔(秒)
    [SerializeField] int startCoins = 50;        // ゲーム開始時の手持ち枚数

    Camera cam;
    float cooldown;

    void Awake() => cam = Camera.main;

    void Start() => CoinCollector.SetCount(startCoins);

    void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        cooldown -= Time.deltaTime;

        // UI上のクリックでは投げない
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (mouse.leftButton.isPressed && cooldown <= 0f)
        {
            if (Throw(mouse.position.ReadValue()))
                cooldown = fireInterval;
        }
    }

    bool Throw(Vector2 screenPos)
    {
        // クリック位置を、Pusher上面の高さの平面に投影して狙いを決める
        var plane = new Plane(Vector3.up, aimSurface.position);
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (!plane.Raycast(ray, out float dist)) return false;

        if (!CoinCollector.TryConsume()) return false; // 手持ちがなければ投げない

        Vector3 target = ray.GetPoint(dist);
        Vector3 dir = target - spawnPoint.position;
        dir.y = 0f;
        dir = dir.sqrMagnitude < 0.01f ? spawnPoint.forward : dir.normalized;

        var coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity);
        var rb = coin.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * throwSpeed + Vector3.up * upSpeed;
        rb.angularVelocity = new Vector3(0f, Random.Range(-3f, 3f), 0f);
        return true;
    }
}