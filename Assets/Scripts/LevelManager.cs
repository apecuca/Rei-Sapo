using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private SO_LevelData levelData;
    [SerializeField] private float cellSize;
    private Vector2 gridOrigin;

    private void OnDrawGizmos()
    {
        int[] data = levelData.getNormalizedLevelData();
        Gizmos.color = Color.white;

        //
        gridOrigin = transform.position;
        Gizmos.DrawWireSphere(gridOrigin, 0.25f);
        gridOrigin.x += cellSize / 2;
        gridOrigin.y -= cellSize / 2;
        //

        for (int y = 0; y < levelData.getSize().y; y++)
        {
            for (int x = 0; x < levelData.getSize().x; x++)
            {
                Vector2 _cubePos = gridOrigin;
                _cubePos.x += cellSize * x;
                _cubePos.y -= cellSize * y;
                
                // trocar cor do cubo aqui;
                Gizmos.DrawWireCube(_cubePos, Vector2.one * cellSize);
            }
        }

        /*
        gridOrigin = Vector2.zero;
        gridOrigin.x -= (levelData.getSize().x / 2) * cellSize.x;
        gridOrigin.y += (levelData.getSize().y / 2) * cellSize.y;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gridOrigin, 0.5f);

        for (int y = 0; y < levelData.getSize().y; y++) {
            for (int x = 0; x < levelData.getSize().x; x++)
            {
                Vector2 _cubePos = gridOrigin;
                _cubePos.x += cellSize.x * x;
                _cubePos.y -= cellSize.y * y;

                Gizmos.DrawWireCube(_cubePos, cellSize);
            }
        }
        */
    }
}
