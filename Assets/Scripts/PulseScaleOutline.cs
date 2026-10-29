using TMPro;
using UnityEngine;

// Escala suave + outline negro intermitente para texto de UI (TMP)
[RequireComponent(typeof(TMP_Text))]
public class PulseScaleOutline : MonoBehaviour
{
    [Header("Escala")]
    public float scaleAmount = 0.05f;     // sube/baja máx 5 %
    public float scaleSpeed = 0.9f;       // velocidad del respiro

    [Header("Outline")]
    public float outlineWidth = 0.4f;
    public float outlinePulseSpeed = 0.6f;  // "intermitente" suave (sine)
    [Range(0f,1f)] public float outlineOpacityMin = 0f;
    [Range(0f,1f)] public float outlineOpacityMax = 1f;

    RectTransform rectTransform;
    TMP_Text tmp;
    Material mat;
    Vector3 originalScale;
    Color faceColor;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tmp = GetComponent<TMP_Text>();
        mat = new Material(tmp.fontSharedMaterial == null ? tmp.font.material : tmp.fontSharedMaterial);
        mat.name = "PulseOutlineMat";
        faceColor = mat.GetColor(ShaderUtilities.ID_FaceColor);
        tmp.fontSharedMaterial = mat;
        tmp.fontMaterial = mat;
        originalScale = transform.localScale;
    }

    void Update()
    {
        float breathe = (Mathf.Sin(Time.time * scaleSpeed * Mathf.PI) + 1f) * 0.5f;
        float scale = Mathf.Lerp(1f, 1f + scaleAmount, breathe);
        rectTransform.localScale = originalScale * scale;

        float pulse = (Mathf.Sin(Time.time * outlinePulseSpeed * Mathf.PI) + 1f) * 0.5f;
        float alpha = Mathf.Lerp(outlineOpacityMin, outlineOpacityMax, pulse);
        // Solo cambia la opacidad del borde (outline), no la del texto
        mat.SetColor(ShaderUtilities.ID_OutlineColor, new Color(0f, 0f, 0f, alpha));
        mat.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);
        // Mantiene el color del texto intacto
        mat.SetColor(ShaderUtilities.ID_FaceColor, faceColor);
    }

    void OnDestroy()
    {
        if (mat != null)
            Destroy(mat);
    }
}