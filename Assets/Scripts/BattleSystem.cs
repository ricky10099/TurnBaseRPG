using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using TMPro;

public class BattleSystem : MonoBehaviour
{
    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Battlers")]
    [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();
    [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();

    [Header("UI")]
    [SerializeField] private GameObject[] enemySelectionButtons;
    [SerializeField] private GameObject battleMenu;
    [SerializeField] private GameObject enemySelectionMenu;
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private GameObject bottomTextPopUp;
    [SerializeField] private TextMeshProUGUI bottomText;

    private PartyManager partyManager;
    private EnemyManager enemyManager;
    private int currentPlayer;

    private const string ACTION_MESSAGE = "'s Action";

    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
        AttackAction(allBattlers[0], allBattlers[1]);
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

    public void ShowBattleMenu()
    {
        actionText.text = playerBattlers[currentPlayer].name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
    }


    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        for(int i = 0; i < enemySelectionButtons.Length; ++i)
        {
            enemySelectionButtons[i].SetActive(false);
        }

        for(int i = 0; i < enemyBattlers.Count; ++i)
        {
            enemySelectionButtons[i].SetActive(true);
            enemySelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[i].name;
        }
    }

    public void SelectEnemy(int currentEnemy)
    {
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];
        currentPlayerEntity.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));

        currentPlayerEntity.battleAction = BattleEntities.Action.Attack;
        ++currentPlayer;

        if(currentPlayer >= playerBattlers.Count)
        {
            Debug.Log(currentPlayerEntity.name);
            Debug.Log(allBattlers[currentPlayerEntity.target].name);
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }

    }

    private void AttackAction(BattleEntities currAttacker, BattleEntities currTarget)
    {
        int damage = currAttacker.strength;
        currAttacker.battleVisuals.PlayAttackAnimation();
        currTarget.currHealth -= damage;
        currTarget.battleVisuals.PlayHitAnimation();
        currTarget.UpdateUI();
        bottomText.text = string.Format("{0} attacks {1} for {2} damage", currAttacker.name, currTarget.name, damage);
    }

    // Update is called once per frame
    void Update()
    {

    }
}


[System.Serializable]
public class BattleEntities
{
    public enum Action { 
        Attack,
        Run,
    };
    public Action battleAction;

    public string name;
    public int currHealth;
    public int maxHealth;
    public int initiative;
    public int strength;
    public int level;
    public bool isPlayer;
    public BattleVisuals battleVisuals;
    public int target;

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

    public void SetTarget(int target) {
        this.target = target;
    }

    public void UpdateUI()
    {
        battleVisuals.ChangeHealth(currHealth);
    }
}