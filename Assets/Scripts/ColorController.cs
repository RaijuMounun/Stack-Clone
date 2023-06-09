using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorController : MonoBehaviour
{
    [SerializeField] ColorData colorData;
    [SerializeField] private MeshRenderer referenceMesh;
    [SerializeField] private List<MeshRenderer> pivots;

    List<Color> _listColor;

    private void Start()
    {
        _listColor = new List<Color>();

        List<Color> pool = new List<Color>(colorData.colors);
        int length = pool.Count;
        for (int i = 0; i < length; i++)
        {
            int index = Random.Range(0, pool.Count);
            Color currentColor = pool[index];
            pool.RemoveAt(index);
            _listColor.Add(currentColor);
        }

        SetColor();
        referenceMesh.material.color = _listColor[0];
    }

    private void SetColor()
    {
        Color baseColor = _listColor[Random.Range(1, _listColor.Count)];
        Color target = _listColor[0];

        for (int i = 0; i < pivots.Count; i++)
        {
            float normalized = (float)(i + 1) / pivots.Count;
            pivots[i].material.color = Color.Lerp(target, baseColor, normalized);
        }
    }

    public Color GetColor(int score)
    {
        int index = score / colorData.scoreLimit;

        Color baseColor = _listColor[index];
        Color targetColor = _listColor[index + 1];

        int currentScore = score % colorData.scoreLimit;

        return Color.Lerp(baseColor, targetColor, (float)currentScore / colorData.scoreLimit);
    }
}
