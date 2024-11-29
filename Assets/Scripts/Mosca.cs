using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mosca : MonoBehaviour
{
    private Transform followTarget;
    [SerializeField] private float minFollowDist;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        if (followTarget == null) return;

        if (Vector2.Distance(transform.position, followTarget.position) <= minFollowDist)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = (followTarget.position - transform.position).normalized * moveSpeed;
    }

    public void StartMosca(Transform _followTarget)
    {
        Debug.Log("Oshi");

        this.enabled = true;
        followTarget = _followTarget;
    }
}
