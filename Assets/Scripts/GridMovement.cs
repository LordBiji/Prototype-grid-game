using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private bool isMoving = false;
    private Vector3 targetPosition;
    private Vector3 pushDirection;
    private GameManiger gameManiger; // Reference ke GameManager

    void Awake()
    {
        // Cari GameManager sekali di awal (lebih efisien)
        gameManiger = FindAnyObjectByType<GameManiger>();
    }

    void Update()
    {
        // Input handling
        if (!isMoving)
        {
            if (Input.GetKey(KeyCode.W))
                TryMove(Vector3.forward);
            else if (Input.GetKey(KeyCode.S))
                TryMove(Vector3.back);
            else if (Input.GetKey(KeyCode.A))
                TryMove(Vector3.left);
            else if (Input.GetKey(KeyCode.D))
                TryMove(Vector3.right);
        }
        else
        {
            // Smooth movement
            float step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            // Check if reached target
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition;
                isMoving = false;
            }
        }
    }

    void TryMove(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 1f))
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                // Cek apakah bisa didorong
                if (CanPushObstacle(hit.transform, direction))
                {
                    PushObstacle(hit.transform, direction);
                    InitiateMovement(direction);
                }
            }
            else if (hit.collider.CompareTag("Immovable"))
            {
                // Hentikan gerakan jika kena immovable
                Debug.Log("Ini obstacle tidak bisa didorong!");
                return;
            }
        }
        else
        {
            InitiateMovement(direction);
        }
    }

    bool CanPushObstacle(Transform obstacle, Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(obstacle.position, direction, out hit, 1f))
        {
            // Ada obstacle/halangan di belakang
            return false;
        }
        return true;
    }

    void PushObstacle(Transform obstacle, Vector3 direction)
    {
        obstacle.position += direction;
    }
    private void InitiateMovement(Vector3 direction)
    {
        targetPosition = transform.position + direction;
        isMoving = true;

        // Beri tahu GameManager bahwa player sudah selesai bergerak
        gameManiger.OnPlayerMoveCompleted();
    }

}