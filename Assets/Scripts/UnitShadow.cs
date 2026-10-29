using UnityEngine;

// Sombra circular negra semitransparente bajo una unidad. Se adjunta como hijo para que la siga.
public class UnitShadow : MonoBehaviour
{
    static Sprite cachedShadowSprite;

    public float radius = 0.5f;
    public float alpha = 0.5f;
    public Vector2 offset = new Vector2(0f, 0f);

    void Awake()
    {
        CreateShadow();
    }

    void CreateShadow()
    {
        if (cachedShadowSprite == null)
            cachedShadowSprite = CreateCircleSprite(64);

        GameObject shadowGO = new GameObject("Shadow");
        shadowGO.transform.SetParent(transform);
        shadowGO.transform.localPosition = new Vector3(offset.x, offset.y, 0f);
        shadowGO.transform.localScale = new Vector3(radius * 2f, radius * 1f, 1f);

        SpriteRenderer sr = shadowGO.AddComponent<SpriteRenderer>();
        sr.sprite = cachedShadowSprite;
        sr.color = new Color(0f, 0f, 0f, alpha);
        sr.sortingOrder = GetBaseSortingOrder() - 1;

        // Evitar que la sombra se escale con el padre de forma extraña: dejar localScale tal cual
    }

    int GetBaseSortingOrder()
    {
        SpriteRenderer parentSr = GetComponent<SpriteRenderer>();
        if (parentSr != null) return parentSr.sortingOrder;
        return 0;
    }

    static Sprite CreateCircleSprite(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float r = size / 2f - 1f;
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float a = Mathf.Clamp01(1f - dist / r);
                // suavizado
                a = a * a * (3f - 2f * a);
                pixels[y * size + x] = new Color(1f, 1f, 1f, a);
            }
        }
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }

    public static void Attach(GameObject host)
    {
        if (host == null) return;
        if (host.GetComponent<UnitShadow>() != null) return;
        host.AddComponent<UnitShadow>();
    }
}