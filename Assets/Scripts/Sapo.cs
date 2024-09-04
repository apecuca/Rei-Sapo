using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sapo : MonoBehaviour
{
    public bool moving { get; private set; } = false;
    private float moveSpeed = 5.0f;
    // Tá como público simplesmente pela conveniência que é
    // o vs2022 mostrar onde tá sendo referenciado.
    // Essa variável é 100% gerenciada pelo LevelManager :)
    public Vector2Int currentCellPos { get; set; } = Vector2Int.zero;

    // Controladores de movimento
    private Vector2 targetMovePos = Vector2.zero;
    public Vector2Int lastMoveDir { get; private set; } = Vector2Int.zero;

    // Guarda se pegou uma chave
    public bool hasKey { get; private set; } = false;

    // Atribuições feitas no inspector
    [Header("Para atribuir")]
    [SerializeField] private LevelManager lvlManager;

    // Método da Unity, roda todo frame
    private void Update()
    {
        if (!moving) return;

        // Move em direção ao destino
        // próxima posição =
        // posição atual +
        // (objetivo - posição atual) normalizado *
        // velocidade * tempo do último frame (em milissegundos)
        //
        Vector2 _nextPos = transform.position;
        _nextPos += (targetMovePos - (Vector2)transform.position).normalized *
            moveSpeed * Time.deltaTime;
        // Atualizar posição
        transform.position = _nextPos;

        // Chegou no destino
        if (Vector2.Distance(transform.position, targetMovePos) <= 0.075f)
        {
            transform.position = targetMovePos;
            moving = false;
            lvlManager.OnGotToDestination();
        }
    }

    // Inicia movimento para posição dada
    public void StartMovingTo(Vector2 _targetPos, Vector2Int _moveDir)
    {
        lastMoveDir = _moveDir;
        targetMovePos = _targetPos;
        moving = true;
    }

    // Método da Unity, é acionado quando
    // o objeto com esta classe colidir com algo não físico
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Fazer algo dependendo da tag do objeto
        switch (collision.gameObject.tag)
        {
            case "Chave":
                hasKey = true;
                Destroy(collision.gameObject);
                break;

            default:
                Debug.Log(collision.gameObject.tag);
                break;
        }
    }
}
