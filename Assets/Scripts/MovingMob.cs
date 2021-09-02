using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingMob : MonoBehaviour
{
    public float timeToMove = 0.01f;
    public LayerMask blockLayer;

    private BoxCollider2D collider;
    private Rigidbody2D rb;
    private float timeToMoveInverse;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        timeToMoveInverse = 7f / timeToMove;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos + new Vector2(xDir, yDir);

        collider.enabled = false;

        hit = Physics2D.Linecast(startPos, endPos, blockLayer);
        collider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(Movement(endPos));
            return true;
        }

        return false;
    }

    protected IEnumerator Movement (Vector3 end)
    {
        float remainingDistance = (transform.position - end).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb.position, end, timeToMoveInverse * Time.deltaTime);
            rb.MovePosition(newPosition);

            remainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }
    }

    protected virtual void AttemptToMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;
}
