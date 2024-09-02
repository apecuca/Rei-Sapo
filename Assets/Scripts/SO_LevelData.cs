using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 1)]
public class SO_LevelData : ScriptableObject
{
    [SerializeField] private Vector2Int gridSize;
    [Tooltip(
        "0 - Espaço vazio\n" +
        "1 - Chão livre\n" +
        "2 - Lixo\n" +
        "3 - VR\n" +
        "4 - Poça\n" +
        "5 - Objetivo\n" +
        "6 - Chave\n" +
        "7 - Moeda")]
    [TextArea(15, 20)] [SerializeField] private string levelData;

    public int[,] getLevelGrid()
    {
        // Inicializar array bidimensional
        // 0 = Y
        // 1 = X
        int[,] _data = new int[gridSize.y, gridSize.x];

        // Popular array
        int _y = 0;
        int _x = 0;
        // O motivo de não fazer um nested loop é que,
        // o tamanho da string pode não estar de acordo
        // com o tamanho da grid, aí por isso é verificar todos
        // os caracteres e fazer o controle por fora do loop
        for (int i = 0; i < levelData.Length; i++)
        {
            // Verificar se é um número
            if (!char.IsNumber(levelData[i])) continue;

            // Popular a célula atual
            _data[_y, _x] = int.Parse(levelData[i].ToString());
            // Continuar para a próxima coluna
            _x++;

            // Caso tenha chegado na última coluna
            if (_x >= gridSize.x)
            {
                // Ir para a próxima linha
                _y++;
                // Resetar o contador de colunas
                _x = 0;
            }
        }

        // Retornar array
        return _data;
    }
}