using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManiger : MonoBehaviour
{
    private List<EnemyWaypointMovement> enemies = new List<EnemyWaypointMovement>();

    void Start()
    {
        Debug.Log("niger");
        // Daftarkan semua enemy di scene
        enemies.AddRange(FindObjectsByType<EnemyWaypointMovement>(FindObjectsSortMode.None));
    }

    public void OnPlayerMoveCompleted()
    {
        StartCoroutine(ProcessEnemyTurns());
    }

    private IEnumerator ProcessEnemyTurns()
    {
        foreach (EnemyWaypointMovement enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.TakeTurn();

                // Tunggu sampai enemy selesai bergerak
                while (enemy.IsMoving())
                {
                    yield return null; // Gunakan yield return null untuk IEnumerator non-generic
                }
            }
        }
    }
}