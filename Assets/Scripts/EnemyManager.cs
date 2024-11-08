using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private EnemyInfo[] allEnemies;
    [SerializeField] private List<Enemy> currentEnemies;

    private static GameObject instance;

    private const float LEVEL_MODIFIER = 0.5f;

    private void Awake()
    {
        if(instance)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this.gameObject;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumenemies)
    {
        currentEnemies.Clear();
        int numEnemies = Random.Range(1, maxNumenemies + 1);

        for(int i = 0; i < numEnemies; ++i)
        {
            Encounter tempEncounter = encounters[Random.Range(0, encounters.Length)];
            int level = Random.Range(tempEncounter.levelMin, tempEncounter.levelMax + 1);
            GenerateEnemyByName(tempEncounter.enemy.enemyName, level);
        }
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
                newEnemy.exp = Mathf.RoundToInt(allEnemies[i].baseExp * newEnemy.level);
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
    public int exp;
    public GameObject enemyVisualPrefab;
}
