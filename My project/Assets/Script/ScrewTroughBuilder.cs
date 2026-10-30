using UnityEngine;

public class ScrewTroughBuilder : MonoBehaviour
{
    [SerializeField] float length = 6f;        // スクリューと同じ長さ
    [SerializeField] float width = 1.2f;       // 溝の内側の幅(= Outer Radius × 2)
    [SerializeField] float wallHeight = 2.0f;  // 壁の高さ(ボールが乗る分、高くする)
    [SerializeField] float floorTopY = -0.6f;  // 床の上面(= -Outer Radius)
    [SerializeField] float ballFloorY = 0.6f;  // ボールが乗る見えない床の高さ(= Outer Radius)
    [SerializeField] float thickness = 0.2f;
    [SerializeField] PhysicsMaterial material; // 摩擦の低いもの(0.1程度)

    [ContextMenu("Build")]
    public void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // 床
        MakeBox("Floor", new Vector3(0f, floorTopY - thickness / 2f, 0f),
            new Vector3(width + thickness * 2f, thickness, length), true);

        // 左右の壁
        float wx = width / 2f + thickness / 2f;
        float wy = floorTopY + wallHeight / 2f;
        MakeBox("WallL", new Vector3(-wx, wy, 0f), new Vector3(thickness, wallHeight, length), true);
        MakeBox("WallR", new Vector3(wx, wy, 0f), new Vector3(thickness, wallHeight, length), true);

        // ボールが乗る見えない床(羽根の上端の高さ)
        MakeBox("BallFloor", new Vector3(0f, ballFloorY - thickness / 2f, 0f),
            new Vector3(width, thickness, length), false);
    }

    void MakeBox(string objName, Vector3 pos, Vector3 scale, bool visible)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = objName;
        go.transform.SetParent(transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.layer = gameObject.layer; // 親(Trough)のレイヤーを引き継ぐ

        if (material != null) go.GetComponent<Collider>().material = material;
        if (!visible) DestroyImmediate(go.GetComponent<MeshRenderer>());
    }
}