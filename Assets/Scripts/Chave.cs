using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chave : MonoBehaviour
{
    private Transform followTarget;
    [SerializeField] private Vector2 followOffset;
    [SerializeField] private float followDamp;

    private void Update()
    {
        // Retornar caso não tenha um alvo
        if (followTarget == null) return;

        // Lerp entre posição atual e (alvo + offset)
        transform.position = Vector2.Lerp(
            transform.position,
            (Vector2)followTarget.position + followOffset,
            followDamp * Time.deltaTime
            );
    }

    public void StartChave(Transform _followTarget)
    {
        // Desabilitar colisor para evitar que essa função seja chamada novamente
        this.gameObject.GetComponent<BoxCollider2D>().enabled = false;

        // Ativar funções de gerenciamento (awake, start, update, etc...)
        this.enabled = true;

        // Atribuir Transform a ser seguido
        followTarget = _followTarget;
    }
}
