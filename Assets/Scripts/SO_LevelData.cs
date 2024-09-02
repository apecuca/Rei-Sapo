using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 1)]
public class SO_LevelData : ScriptableObject
{
    [SerializeField] private Vector2 size;
    [TextArea(15, 20)] [SerializeField] private string levelData;

    public int[] getNormalizedLevelData()
    {
        List<int> _normalizedData = new List<int>();
        foreach (char _c in levelData)
        {
            if (!char.IsNumber(_c)) continue;

            _normalizedData.Add(int.Parse(_c.ToString()));
        }

        return _normalizedData.ToArray();
    }

    public Vector2 getSize() { return size; }
}
