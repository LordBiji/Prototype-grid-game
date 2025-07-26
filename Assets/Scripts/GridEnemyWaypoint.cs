using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWaypointMovement : MonoBehaviour
{
    public List<Vector3> waypointsGrid;
    public float moveSpeed = 5f;
    public bool loop = true;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private Vector3 targetPosition;

    void Start()
    {
        // Pastikan posisi awal snap ke grid
        SnapToGrid();
    }

    public void TakeTurn()
    {
        if (!isMoving && waypointsGrid.Count > 0)
        {
            targetPosition = waypointsGrid[currentWaypointIndex];
            StartCoroutine(MoveToWaypoint());
        }
    }

    private IEnumerator MoveToWaypoint()
    {
        isMoving = true;

        // Gerakan per grid (1 unit per langkah)
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 nextStep = transform.position + direction;

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextStep, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, nextStep) < 0.01f)
            {
                nextStep += direction; // Langkah berikutnya
            }

            yield return null;
        }

        transform.position = targetPosition;
        UpdateWaypointIndex();
        isMoving = false;
    }

    private void UpdateWaypointIndex()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypointsGrid.Count)
        {
            currentWaypointIndex = loop ? 0 : waypointsGrid.Count - 1;
        }
    }

    public bool IsMoving() => isMoving;

    private void SnapToGrid()
    {
        transform.position = new Vector3(
            Mathf.Round(transform.position.x),
            Mathf.Round(transform.position.y),
            Mathf.Round(transform.position.z)
        );
    }

    // Visualisasi waypoint di Editor
    private void OnDrawGizmosSelected()
    {
        if (waypointsGrid == null) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypointsGrid.Count; i++)
        {
            Gizmos.DrawSphere(waypointsGrid[i], 0.2f);
            if (i < waypointsGrid.Count - 1)
                Gizmos.DrawLine(waypointsGrid[i], waypointsGrid[i + 1]);
            if (loop && i == waypointsGrid.Count - 1)
                Gizmos.DrawLine(waypointsGrid[i], waypointsGrid[0]);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Deteksi tabrakan dengan player
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject); // Hancurkan player
            Debug.Log("Player hancur oleh: " + gameObject.name);
        }
    }
}