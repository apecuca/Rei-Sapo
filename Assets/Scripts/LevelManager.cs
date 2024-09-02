using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private SO_LevelData levelData;
    private float cellSize = 1.055f;
    //private Vector2 gridOrigin = Vector3.zero;

    private void OnDrawGizmos()
    {
        Color[] _colorPallete = {
            Color.white,
            Color.green,
            Color.black,
            Color.cyan,
            Color.red,
            Color.yellow,
            Color.blue,
            Color.yellow
        };

        // Data
        int[,] _data = levelData.getLevelGrid();
        Gizmos.color = Color.white;

        // Definir a origem da grid
        Vector3 _gridOrigin = transform.position;
        // Desenhar uma esfera nesse ponto
        Gizmos.DrawWireSphere(_gridOrigin, 0.25f);
        // Corrigir a origem para desenho das células
        _gridOrigin.x += cellSize / 2;
        _gridOrigin.y -= cellSize / 2;

        // desenhar
        // Nestes loop, passa por todas as células
        // da grid, não importa o tamanho
        for (int y = 0; y < _data.GetLength(0); y++) {
            for (int x = 0; x < _data.GetLength(1); x++)
            {
                // Definir posição
                Vector2 _cubePos = _gridOrigin;
                _cubePos.x += cellSize * x;
                _cubePos.y -= cellSize * y;

                // Trocar cor do cubo
                if (_data[y, x] < _colorPallete.Length)
                    Gizmos.color = _colorPallete[_data[y, x]];

                // desenhar a cell
                Gizmos.DrawWireCube(_cubePos, Vector2.one * cellSize);
            }
        }
    }
}
