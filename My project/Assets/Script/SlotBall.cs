using UnityEngine;

public enum BallColor { Yellow, Pink, Green }

[RequireComponent(typeof(Rigidbody))]
public class SlotBall : MonoBehaviour
{
    public static int AliveCount { get; private set; }

    public BallColor Kind { get; private set; }
    public bool Consumed { get; set; }

    [SerializeField] float lifetime = 90f; // ゴールに入らなかったボールを消すまでの秒数

    void OnEnable() => AliveCount++;
    void OnDisable() => AliveCount--;

    public void Setup(BallColor kind)
    {
        Kind = kind;
        Color col = kind switch
        {
            BallColor.Yellow => new Color(1f, 0.85f, 0.1f),
            BallColor.Pink => new Color(1f, 0.45f, 0.75f),
            _ => new Color(0.25f, 0.85f, 0.3f),
        };

        // マテリアルを複製せず色だけ変える(軽量)
        var r = GetComponentInChildren<Renderer>();
        var mpb = new MaterialPropertyBlock();
        r.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", col); // URP Lit
        mpb.SetColor("_Color", col);     // 標準シェーダー
        r.SetPropertyBlock(mpb);

        Destroy(gameObject, lifetime);
    }
}