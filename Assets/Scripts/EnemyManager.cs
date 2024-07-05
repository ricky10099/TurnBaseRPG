using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        GenerateEnemyByName("Slime", 99);
        GenerateEnemyByName("Slime", 10);
    }

    private void GenerateEnemyByName(string enemyName, int level)
    {
        for (int i = 0; i < allEnemies.Length; ++i)
        {
            if (enemyName == allEnemies[i].enemyName)
            {
                Enemy newEnemy = new Enemy();

                newEnemy.enemyName = allEnemies[i].enemyName;
                newEnemy.level = level;
                float levelModifier = (LEVEL_MODIFIER * newEnemy.level);

                newEnemy.maxHealth = Mathf.RoundToInt(allEnemies[i].baseHealth + (allEnemies[i].baseHealth * levelModifier));
                newEnemy.currHealth = newEnemy.maxHealth;
                newEnemy.strength = Mathf.RoundToInt(allEnemies[i].baseStr + (allEnemies[i].baseStr * levelModifier));
                newEnemy.initiative = Mathf.RoundToInt(allEnemies[i].baseInitiative + (allEnemies[i].baseInitiative * levelModifier));
                newEnemy.enemyVisualPrefab = allEnemies[i].enemyVisualPrefab;

                currentEnemies.Add(newEnemy);
            }
        }
    }

    public List<Enemy> GetCurrentEnemies()
    {
        return currentEnemies;
    }
}

[System.Serializable]
public class Enemy
{
    public string enemyName;
    public int level;
    public int currHealth;
    public int maxHealth;
    public int strength;
    public int initiative;
    public GameObject enemyVisualPrefab;
}
