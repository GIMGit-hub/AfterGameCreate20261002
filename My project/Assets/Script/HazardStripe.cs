using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class HazardStripe : MonoBehaviour
{
    [SerializeField] Color a = new Color(1f, 0.78f, 0f);
    [SerializeField] Color b = new Color(0.08f, 0.08f, 0.08f);
    [SerializeField] float tilePixels = 24f; // ストライプ1周期の画面上の大きさ

    void Start()
    {
        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Repeat,
            filterMode = FilterMode.Bilinear
        };
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, ((x + y) % size) < size / 2 ? a : b);
        tex.Apply();

        var img = GetComponent<RawImage>();
        img.texture = tex;
        var r = ((RectTransform)transform).rect;
        img.uvRect = new Rect(0, 0, r.width / tilePixels, r.height / tilePixels);
    }
}