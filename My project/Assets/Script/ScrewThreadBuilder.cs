using UnityEngine;

public class ScrewThreadBuilder : MonoBehaviour
{
    [SerializeField] float length = 3f;         // ÉXÉNÉäÉÖÅ[ÇÃí∑Ç≥
    [SerializeField] float coreRadius = 0.06f;  // íÜêSé≤ÇÃîºåa
    [SerializeField] float outerRadius = 0.3f;  // âHç™ÇÃäOë§ÇÃîºåa
    [SerializeField] float pitch = 0.5f;        // 1âÒì]Ç≈êiÇﬁãóó£(É{Å[ÉãÇÃíºåaÇÊÇËëÂÇ´Ç≠)
    [SerializeField] float thickness = 0.04f;   // âHç™ÇÃå˙Ç›
    [SerializeField] int segmentsPerTurn = 16;  // 1âÒì]Ç†ÇΩÇËÇÃî¬ÇÃêî

    [ContextMenu("Build")]
    public void Build()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // íÜêSé≤(Zï˚å¸Ç…êLÇŒÇ∑)
        var core = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        DestroyImmediate(core.GetComponent<Collider>());
        core.transform.SetParent(transform, false);
        core.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        core.transform.localScale = new Vector3(coreRadius * 2f, length / 2f, coreRadius * 2f);

        // âHç™
        float rc = (coreRadius + outerRadius) * 0.5f;
        float depth = outerRadius - coreRadius;
        float segLen = Mathf.Sqrt(Mathf.Pow(2f * Mathf.PI * rc, 2f) + pitch * pitch) / segmentsPerTurn * 1.15f;
        int total = Mathf.CeilToInt(length / pitch * segmentsPerTurn);

        for (int i = 0; i <= total; i++)
        {
            float turns = i / (float)segmentsPerTurn;
            float z = -length / 2f + turns * pitch;
            if (z > length / 2f) break;

            float a = turns * Mathf.PI * 2f;
            Vector3 outward = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f);
            Vector3 tangent = new Vector3(-Mathf.Sin(a) * rc, Mathf.Cos(a) * rc, pitch / (2f * Mathf.PI)).normalized;

            var fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            DestroyImmediate(fin.GetComponent<Collider>()); // å©ÇΩñ⁄ÇæÇØ
            fin.transform.SetParent(transform, false);
            fin.transform.localPosition = new Vector3(outward.x * rc, outward.y * rc, z);
            fin.transform.localRotation = Quaternion.LookRotation(tangent, outward);
            fin.transform.localScale = new Vector3(thickness, depth, segLen);
        }
    }
}