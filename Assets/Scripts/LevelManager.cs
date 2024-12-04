using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Índice do nível atual, usado para gerenciar
    // loading de fases
    private int currentLevelIndex;
    // Array com os SOs (Scriptable Objects) dos níveis
    [SerializeField] private SO_LevelData[] levelsData;

    // Dados do nível atual
    private int[,] currentLevelGrid = null;

    // Gerenciamento da grid do nível
    private float cellSize = 1.063f;
    private Vector2 gridOrigin = Vector3.zero;

    // Sapo :)
    // Variável atribuída ao carregar a cena de jogo
    private Sapo sapo = null;

    // Headers são usados pra dividir variáveis em
    // blocos dentro do Inspector da Unity
    [Header("Objetos do jogo")]
    // Array com todos os objetos do jogo, organizados
    // do mesmo jeito que a grid dos níveis
    [SerializeField] private GameObject[] gameObjectPrefabs;

    [Header("Fade")]
    [SerializeField] private Image fadePanel;
    [SerializeField] private float fadeDampness;
    public bool fading { get; private set; }

    [Header("Testing")]
    [SerializeField] private bool testingLevel;
    [SerializeField] private int testLevelIndex;

    public static LevelManager instance { get; private set; }

    // Paleta de cores para debug da grid
    // Presente no OnDrawGizmos (lá em baixo)
    private Color[] _colorPallete = {
            Color.white,
            Color.green,
            Color.black,
            Color.cyan,
            Color.red,
            Color.yellow,
            Color.blue,
            Color.yellow
    };

    private void Awake()
    {
        // Atribui a instância estática dessa classe
        // Caso já tenha uma (que acontece ao voltar pro menu),
        // a instância extra se destrói
        if (instance == null)
        {
            instance = this;

            // Impede que essa instância seja
            // destruída ao carregar outra cena
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    // Método da Unity, roda todo frame
    private void Update()
    {
        if (sapo == null) return;

        // Ler os Inputs se o sapo estiver livre
        if (!sapo.moving && !fading)
            HandleInputs();
    }

    public void StartNewGame()
    {
        if (fading) return;

        currentLevelIndex = testingLevel ? testLevelIndex : 0;
        StartCoroutine(LoadGame());
    }

    private IEnumerator LoadGame()
    {
        // Fade in
        StartCoroutine(FadeIn());
        while (fading)
            yield return new WaitForEndOfFrame();

        // Carregar a nova fase em uma ação assíncrona
        AsyncOperation _newScene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);

        // Esperar a fase carregar
        while (!_newScene.isDone)
            yield return new WaitForEndOfFrame();

        // Atribuir objetos e iniciar level
        sapo = GameObject.FindGameObjectWithTag("Sapo").GetComponent<Sapo>();
        StartLevel(currentLevelIndex);

        // Fade out
        StartCoroutine(FadeOut());
    }

    private void StartLevel(int _index)
    {
        // Atualizar index atual
        currentLevelIndex = _index;

        // Iniciar data da grid
        currentLevelGrid = levelsData[currentLevelIndex].getLevelGrid();

        // Origem da grid
        gridOrigin = levelsData[currentLevelIndex].getGridOrigin();
        //gridOrigin.x += cellSize / 2;
        //gridOrigin.y -= cellSize / 2;

        // Instanciar o level
        Instantiate(levelsData[currentLevelIndex].getLevelPrefab());

        // Posição do sapo
        sapo.currentCellPos = levelsData[currentLevelIndex].getStartingCell();
        Vector2 _startingSapoPosition = gridOrigin;
        _startingSapoPosition.x += sapo.currentCellPos.x * cellSize;
        _startingSapoPosition.y -= sapo.currentCellPos.y * cellSize;
        sapo.transform.position = _startingSapoPosition;

        // Instanciar os objetos do nível
        SpawnLevelObjects();
    }

    private void SpawnLevelObjects()
    {
        // Loop que passa por todas as células
        // Começa pelo topo esquerdo (y0 e x0)
        for (int y = 0; y < currentLevelGrid.GetLength(0); y++) {
            for (int x = 0; x < currentLevelGrid.GetLength(1); x++)
            {
                // Info da célula
                int _currentCellData = currentLevelGrid[y, x];

                // Ignorar se não tiver que spawnar nada
                if (_currentCellData == 0 ||
                    _currentCellData == 1)
                    continue;

                // Posição do objeto que vai ser spawnado
                Vector2 _objPos = gridOrigin;
                _objPos.y -= cellSize * y;
                _objPos.x += cellSize * x;

                // Ignorar se não tiver salvo
                if (gameObjectPrefabs[_currentCellData] == null)
                    continue;

                // Spawnar (instanciar) o objeto em questão
                Instantiate(gameObjectPrefabs[_currentCellData], 
                    _objPos, Quaternion.identity);
            }
        }
    }

    // Método para ler os inputs
    private void HandleInputs()
    {
        // Caso a tecla em questão tenha sido pressionada
        // no frame atual, fazer algo

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSapo(new Vector2Int(1, 0));
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSapo(new Vector2Int(-1, 0));
            return;
        }
            
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSapo(new Vector2Int(0, -1));
            return;
        }
            
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSapo(new Vector2Int(0, 1));
            return;
        }

        // Esses returns são para evitar que dois
        // inputs sejam acionados no mesmo frame.
        // Daria pra fazer uma verificação por fora,
        // mas assim é mais fácil

        if (Input.GetKeyDown(KeyCode.R))
            RestartLevel();
    }

    private void MoveSapo(Vector2Int _dir)
    {
        // Informações que serão usadas no loop
        Vector2Int _finalDestination = sapo.currentCellPos;
        bool _gotToDestination = false;
        // começa pelo 1 pra evitar checar a célula inicial
        int _iterations = 1;

        // Loop que passa por todas as possíveis 
        // células na direção dada.
        // Esse loop ficou bem grande por causa
        // dos comentários kk
        while (!_gotToDestination)
        {
            // Verificar se precisa parar na céula atual
            if (_iterations != 1)
            {
                int _currentCellData = currentLevelGrid[_finalDestination.y, _finalDestination.x];
                // Parar se estiver em uma:
                // Vitória régia
                // Poça
                // Objetivo final + tiver pego a chave
                if (_currentCellData == 3 ||
                    _currentCellData == 4 ||
                    (_currentCellData == 5 && sapo.hasKey))
                {
                    _gotToDestination = true;
                    continue;
                }
            }

            // Preparar próxima posição (célula)
            Vector2Int _nextCell = sapo.currentCellPos + (_dir * _iterations);

            // Verificar se a posição é válida
            if (!checkInsideBoundaries(_nextCell))
            {
                // Se não for, para o loop na última posição
                _gotToDestination = true;
                continue;
            }

            // Guardar informação da célula sendo verificada
            int _nextCellData = currentLevelGrid[_nextCell.y, _nextCell.x];

            // Parar se for uma célula vazia ou lixo
            if (_nextCellData == 0 ||
                _nextCellData == 2)
            {
                _gotToDestination = true;
                continue;
            }
                
            // Se tiver, substitui a próxima
            _finalDestination = _nextCell;

            // Prevenção de loop infinito
            _iterations++;
            if (_iterations >= 100)
            {
                _finalDestination = sapo.currentCellPos;
                _gotToDestination = true;
            }
        }

        // Gerar nova posição para o sapo,
        // em coordenadas do mundo
        Vector2 _newPos = gridOrigin;
        _newPos.x += cellSize * _finalDestination.x;
        _newPos.y -= cellSize * _finalDestination.y;

        // Atualizar a célula atual e iniciar movimento
        // para nova posição
        sapo.currentCellPos = _finalDestination;
        sapo.StartMovingTo(_newPos, _dir);
    }

    // Método que retorna se a coordenada é
    // válida (dentro da grid)
    private bool checkInsideBoundaries(Vector2Int _cell)
    {
        // máximo dentro das camadas da grid (y e x)
        int _yBoundaries = currentLevelGrid.GetLength(0);
        int _xBoundaries = currentLevelGrid.GetLength(1);

        // Se for menor que 0 ou maior/igual que o máximo, tá fora
        if (_cell.x < 0 || _cell.x >= _xBoundaries) return false;
        if (_cell.y < 0 || _cell.y >= _yBoundaries) return false;
        return true;
        // Se não, tá dentro
    }

    // Método chamado quando o sapo chega 
    // em um destino
    public void OnGotToDestination()
    {
        if (fading) return;

        // Info da célula atua
        int _cellData = currentLevelGrid[sapo.currentCellPos.y, sapo.currentCellPos.x];
        
        // Fazer algo dependendo do tipo de célula
        switch (_cellData)
        {
            // Poça
            case 4:
                RestartLevel();
                break;

            // Portal
            case 5:
                if (sapo.hasKey)
                    OnFinishedLevel();
                break;

            // O resto faz nada
            default: break;
        }
    }

    // Método para reiniciar cena atual
    private void RestartLevel()
    {
        StartCoroutine(LoadGame());
    }

    // Método chamado quando o sapo entra no portal
    private void OnFinishedLevel()
    {
        // Escreve no console
        Debug.Log("Finished level!");

        // Aumenta o index de níveis
        currentLevelIndex++;

        if (currentLevelIndex == levelsData.Length)
            OnFinishedGame();
        else
            RestartLevel();
    }

    private void OnFinishedGame()
    {
        StartCoroutine(FadeToScene(2));
        ResetVariables();
    }

    public void ResetVariables()
    {
        currentLevelIndex = 0;
        currentLevelGrid = null;
    }

    public void GotoMenu()
    {
        StartCoroutine(FadeToScene(0));
        ResetVariables();
    }

    private IEnumerator FadeToScene(int _index)
    {
        // Fade out
        StartCoroutine(FadeIn());
        while (fading)
            yield return new WaitForEndOfFrame();

        // Carregar a nova fase em uma ação assíncrona
        AsyncOperation _newScene = SceneManager.LoadSceneAsync(_index, LoadSceneMode.Single);

        // Esperar a fase carregar
        while (!_newScene.isDone)
            yield return new WaitForEndOfFrame();

        // Fade in
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        fading = true;

        while (fadePanel.color.a < 0.95f)
        {
            // Lerp entre cor atual e preto
            fadePanel.color = Color.Lerp(fadePanel.color, Color.black, fadeDampness * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        fadePanel.color = Color.black;

        fading = false;
    }

    private IEnumerator FadeOut()
    {
        fading = true;

        Color _transparent = new Color(0, 0, 0, 0);

        while (fadePanel.color.a > 0.05f)
        {
            // Lerp entre cor atual e transparente
            fadePanel.color = Color.Lerp(fadePanel.color, _transparent, fadeDampness * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        fadePanel.color = _transparent;

        fading = false;
    }

    // Método da Unity, usado para desenhar
    // no editor. Não funciona em builds
    private void OnDrawGizmos()
    {
        // Retorna se não tiver informação da grid
        if (currentLevelGrid == null) return;
        
        // Define a cor dos desenhos como branca
        Gizmos.color = Color.white;

        // Desenhar uma esfera na origem
        Gizmos.DrawWireSphere(gridOrigin, 0.25f);

        // Loop que passa por todas as células da grid
        for (int y = 0; y < currentLevelGrid.GetLength(0); y++) {
            for (int x = 0; x < currentLevelGrid.GetLength(1); x++)
            {
                // Definir posição
                Vector2 _cubePos = gridOrigin;
                _cubePos.y -= cellSize * y;
                _cubePos.x += cellSize * x;

                // Trocar cor do cubo dependendo do tipo de célula
                if (currentLevelGrid[y, x] < _colorPallete.Length)
                    Gizmos.color = _colorPallete[currentLevelGrid[y, x]];

                // desenhar a célula
                Gizmos.DrawWireCube(_cubePos, Vector2.one * cellSize);
            }
        }
    }
}
