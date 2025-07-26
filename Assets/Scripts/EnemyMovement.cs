using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum MovementPattern { Horizontal, Square }
    public MovementPattern pattern;
    public float moveSpeed = 5f;

    private Vector3[] directions;
    private int currentDirectionIndex = 0;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        // Set pola gerakan berdasarkan pilihan
        switch (pattern)
        {
            case MovementPattern.Horizontal:
                directions = new Vector3[] { Vector3.right, Vector3.left };
                break;
            case MovementPattern.Square:
                directions = new Vector3[] { Vector3.forward, Vector3.right,
                                           Vector3.back, Vector3.left };
                break;
        }
    }

    // Dipanggil dari GameManager saat giliran enemy
    public void TakeTurn()
    {
        if (!isMoving)
        {
            Vector3 nextDirection = directions[currentDirectionIndex];
            TryMove(nextDirection);
        }
    }

    void TryMove(Vector3 direction)
    {
        RaycastHit hit;
        if (!Physics.Raycast(transform.position, direction, out hit, 1f))
        {
            // Jika tidak ada halangan, mulai gerakan
            targetPosition = transform.position + direction;
            StartCoroutine(MoveEnemy());
        }
        else
        {
            // Jika ada halangan, balik arah (untuk horizontal) atau lanjut ke arah berikutnya (square)
            if (pattern == MovementPattern.Horizontal)
            {
                currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
            }
            else if (pattern == MovementPattern.Square)
            {
                currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
            }
        }
    }

    System.Collections.IEnumerator MoveEnemy()
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                                                    targetPosition,
                                                    moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition; // Snap ke grid
        isMoving = false;

        // Update indeks arah untuk pola
        currentDirectionIndex = (currentDirectionIndex + 1) % directions.Length;
    }
}