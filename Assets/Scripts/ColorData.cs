using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "Create ColorData", order = 0)]
public class ColorData : ScriptableObject
{
    public List<Color> colors;
    public int scoreLimit;
}


