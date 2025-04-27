using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    public EnemyModel GreedySearch(EnemyModel baseEnemy, int fitnessVersion, float difficultyWeight, float balanceWeight, float targetScore)
    {
        EnemyModel bestCandidate = baseEnemy.Clone(); // Necesitamos clonar otro enemigo
        float bestScore = bestCandidate.CalculateTotalScore(fitnessVersion, difficultyWeight, balanceWeight);

        PriorityQueue<EnemyModel> openList = new PriorityQueue<EnemyModel>();
        HashSet<string> closedSet = new HashSet<string>();

        openList.Enqueue(bestCandidate, bestScore);

        int iteration = 0;
        while (openList.Count > 0 && iteration < 50)
        {
            iteration++;

            EnemyModel current = openList.Dequeue();
            float currentScore = current.CalculateTotalScore(fitnessVersion, difficultyWeight, balanceWeight);

            closedSet.Add(current.GetHashCode().ToString());

            // Revisamos si hay vecinos y hacemos mutaciones leves
            List<EnemyModel> neighbors = GenerateNeighbors(current);

            foreach (var neighbor in neighbors)
            {
                string hash = neighbor.GetHashCode().ToString();
                if (closedSet.Contains(hash)) continue;

                float neighborScore = neighbor.CalculateTotalScore(fitnessVersion, difficultyWeight, balanceWeight);
                openList.Enqueue(neighbor, neighborScore);

                if (Mathf.Abs(neighborScore - targetScore) < Mathf.Abs(bestScore - targetScore))
                {
                    bestScore = neighborScore;
                    bestCandidate = neighbor;
                }
            }

            if (openList.PeekPriority() + 0.05f < bestScore)
                break;
        }

        return bestCandidate;
    }

    private List<EnemyModel> GenerateNeighbors(EnemyModel enemy)
    {
        // Mutaciones! Cambiamos unas stats u listo
        List<EnemyModel> neighbors = new List<EnemyModel>();

        // Por ahora HP; se me va el tiempo
        float step = 10f;
        if (enemy.HP + step <= 100) neighbors.Add(enemy.MutateHP(enemy.HP + step));
        if (enemy.HP - step >= 10) neighbors.Add(enemy.MutateHP(enemy.HP - step));

        // Otras mutaciones pa despues

        return neighbors;
    }
}
