using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object
/// SOs são containers de data que são usados para
/// evitar cópias de informação, deixando um só objeto
/// para ser referenciado (este)
/// 
/// Para criar um, é só apertar o botão direito no
/// navegador de arquivos da Unity > Create > ScriptableObjects > Level Data
/// 
/// Todos os SOs dos níveis estão na pasta "Level Data"
/// </summary>

[CreateAssetMenu(fileName = "Level Data", menuName = "ScriptableObjects/Level Data", order = 1)]
public class SO_LevelData : ScriptableObject
{
    // Todas as variáveis de ScriptableObjects são
    // controladas pelo Inspector, por isso
    // não tem nenhum setter, somente getters

    // SerializeField é um tag da Unity, que faz a
    // variável ficar visível no Inspector e poder
    // ser editada.
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private Vector2 gridOrigin;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Vector2Int startingCell;

    // Tooltip é o que aparece quando
    // dixa o mouse em cima de um elemento, que
    // nesse caso, é a variável levelData
    [Tooltip(
        "0 - Espaço vazio\n" +
        "1 - Chão livre\n" +
        "2 - Tronco\n" +
        "3 - Vitória Régia\n" +
        "4 - Poça\n" +
        "5 - Portal\n" +
        "6 - Chave\n" +
        "7 - Moeda")]
    [TextArea(15, 20)] [SerializeField] private string levelData;
    // Essa variável tá com 3 tags: Tooltip, TextArea e SerializeField.
    // Fica meio confuso de ler mesmo

    // Retorna um array bidimensional 
    public int[,] getLevelGrid()
    {
        // Inicializar array bidimensional vazio
        // 0 = Y
        // 1 = X
        int[,] _data = new int[gridSize.y, gridSize.x];

        // Controladores do loop
        int _y = 0;
        int _x = 0;
        // O motivo de não fazer um nested loop é que
        // o tamanho da string pode não estar de acordo
        // com o tamanho da grid, por isso, é melhor verificar todos
        // os caracteres e fazer o controle por fora do loop
        for (int i = 0; i < levelData.Length; i++)
        {
            // Verificar se o caractere é um número
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
    
    // Getters das variáveis
    public Vector2 getGridOrigin() { return gridOrigin; }
    public GameObject getLevelPrefab() { return levelPrefab; }
    public Vector2Int getStartingCell() { return startingCell; }
}