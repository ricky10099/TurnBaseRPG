using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    private PartyManager partyManager;
    private EnemyManager enemyManager;

    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = partyManager.GetCurrentParty();

        for (int i = 0; i < currentParty.Count; ++i)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentParty[i].memberName, currentParty[i].currHealth, currentParty[i].maxHealth, currentParty[i].initiative, currentParty[i].strength, currentParty[i].level, true);
            BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].memberBattleVisualPrefab, partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(currentParty[i].maxHealth, currentParty[i].maxHealth, currentParty[i].level);
            tempEntity.battleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = enemyManager.GetCurrentEnemies();

        for(int i = 0; i < currentEnemies.Count; ++i)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentEnemies[i].enemyName, currentEnemies[i].currHealth, currentEnemies[i].maxHealth, currentEnemies[i].initiative, currentEnemies[i].strength, currentEnemies[i].level, false);
            BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].enemyVisualPrefab, enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(currentEnemies[i].maxHealth, currentEnemies[i].maxHealth, currentEnemies[i].level);
            tempEntity.battleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}


[System.Serializable]
public class BattleEntities
{
    public string name;
    public int currHealth;
    public int maxHealth;
    public int initiative;
    public int strength;
    public int level;
    public bool isPlayer;
    public BattleVisuals battleVisuals;

    public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, int strength, int level, bool isPlayer)
    {
        this.name = name;
        this.currHealth = currHealth;
        this.maxHealth = maxHealth;
        this.initiative = initiative;
        this.strength = strength;
        this.level = level;
        this.isPlayer = isPlayer;
    }
}