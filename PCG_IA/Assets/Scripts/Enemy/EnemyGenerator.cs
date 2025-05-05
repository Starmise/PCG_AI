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

        // Mutacion HP;
        float step = 10f;
        if (enemy.HP + step <= 100) neighbors.Add(enemy.MutateHP(enemy.HP + step));
        if (enemy.HP - step >= 10) neighbors.Add(enemy.MutateHP(enemy.HP - step));

        // Mutar poder de ataque
        float stepAP = 1f;
        if (enemy.AttackPower + stepAP <= 10) neighbors.Add(enemy.MutateAttackPower(enemy.AttackPower + stepAP));
        if (enemy.AttackPower - stepAP >= 1) neighbors.Add(enemy.MutateAttackPower(enemy.AttackPower - stepAP));

        // Mutar velocidad de ataque
        float stepAR = 0.1f;
        if (enemy.AttackRate + stepAR <= 1) neighbors.Add(enemy.MutateAttackRate(enemy.AttackRate + stepAR));
        if (enemy.AttackRate - stepAR >= 0.1f) neighbors.Add(enemy.MutateAttackRate(enemy.AttackRate - stepAR));

        // Mutar velocidad
        float stepS = 1f;
        if (enemy.Speed + stepS <= 4) neighbors.Add(enemy.MutateSpeed(enemy.Speed + stepS));
        if (enemy.Speed - stepS >= 1) neighbors.Add(enemy.MutateSpeed(enemy.Speed - stepS));

        // Mutar rango de detección
        float stepDR = 1.5f;
        if (enemy.DetectionRange + stepDR <= 7.5f) neighbors.Add(enemy.MutateDetectionRange(enemy.HP + stepDR));
        if (enemy.DetectionRange - stepDR >= 1) neighbors.Add(enemy.MutateDetectionRange(enemy.HP - stepDR));

        return neighbors;
    }
}
