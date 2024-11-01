using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class BattleSystem : MonoBehaviour
{
    [SerializeField]
    private enum BattleState
    {
        Start,
        Selection,
        Battle,
        Won,
        Lost,
        Run,
    }

    [Header("Battle State")]
    [SerializeField] private BattleState state;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] partySpawnPoints;
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("Attack Points")]
    [SerializeField] private Transform[] partyAttackPoints;
    [SerializeField] private Transform[] enemyAttackPoints;

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

    [SerializeField] private Camera mainCamera;
    private Vector3 originCamPosition = new Vector3(0, 2, -10);
    private Vector3 cameraTargetPosition;

    private const string ACTION_MESSAGE = "のターン";
    private const string WIN_MESSAGE = "バトルに勝ちました";
    private const string LOSE_MESSAGE = "バトルに負けました";
    private const string SUCCESFULLY_RAN_MESSAGE = "逃げました";
    private const string UNSUCCESFULLY_RAN_MESSAGE = "逃げられない";
    private const int TURN_DURATION = 2;
    private const int RUN_CHANCE = 50;
    private const string OVERWORLD_SCENE = "OverworldScene";
    // Start is called before the first frame update
    void Start()
    {
        partyManager = GameObject.FindFirstObjectByType<PartyManager>();
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

        cameraTargetPosition = originCamPosition;

        CreatePartyEntities();
        CreateEnemyEntities();
        ShowBattleMenu();
        DetermineBattleOrder();
    }

    private IEnumerator BattleRoutine()
    {
        enemySelectionMenu.SetActive(false);
        state = BattleState.Battle;
        bottomTextPopUp.SetActive(true);

        for (int i = 0; i < allBattlers.Count; ++i)
        {
            if (state == BattleState.Battle && allBattlers[i].currHealth > 0)
            {
                switch (allBattlers[i].battleAction)
                {
                    case BattleEntities.Action.Attack:
                        //Debug.Log(allBattlers[i].name + "is attacking:" + allBattlers[allBattlers[i].target].name);
                        yield return StartCoroutine(AttackRoutine(i));
                        break;
                    case BattleEntities.Action.Run:
                        yield return StartCoroutine(RunRoutine());
                        break;
                    default:
                        Debug.Log("Error - incorrect battle action");
                        break;
                }
            }
        }

        RemoveDeadBattlers();

        if (state == BattleState.Battle)
        {
            bottomTextPopUp.SetActive(false);
            currentPlayer = 0;
            ShowBattleMenu();
        }

        yield return null;
    }

    private IEnumerator AttackRoutine(int i)
    {
        //mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, originCamPosition, 0.1f * Time.deltaTime);
        cameraTargetPosition = originCamPosition;

        if (allBattlers[i].isPlayer)
        {
            BattleEntities currAttacker = allBattlers[i];
            if (allBattlers[currAttacker.target].currHealth <= 0)
            {
                currAttacker.SetTarget(GetRandomEnemy());
            }
            BattleEntities currTarget = allBattlers[currAttacker.target];
            AttackAction(currAttacker, currTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if (currTarget.currHealth <= 0)
            {
                bottomText.text = string.Format("{0} が {1} を倒しました", currAttacker.name, currTarget.name);
                yield return new WaitForSeconds(TURN_DURATION);
                enemyBattlers.Remove(currTarget);
                //allBattlers.Remove(currTarget);

                if (enemyBattlers.Count <= 0)
                {
                    state = BattleState.Won;
                    bottomText.text = WIN_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log("Go back to overworld scene");
                    SceneManager.LoadScene(OVERWORLD_SCENE);
                }
            }
        }

        if (i < allBattlers.Count && !allBattlers[i].isPlayer)
        {
            BattleEntities currAttacker = allBattlers[i];
            currAttacker.SetTarget(GetRandomPartyMember());
            BattleEntities currTarget = allBattlers[currAttacker.target];

            AttackAction(currAttacker, currTarget);
            yield return new WaitForSeconds(TURN_DURATION);

            if (currTarget.currHealth <= 0)
            {
                bottomText.text = string.Format("{0} が {1} を倒しました", currAttacker.name, currTarget.name);
                yield return new WaitForSeconds(TURN_DURATION);
                playerBattlers.Remove(currTarget);
                //allBattlers.Remove(currTarget);

                if (playerBattlers.Count <= 0)
                {
                    state = BattleState.Lost;
                    bottomText.text = LOSE_MESSAGE;
                    yield return new WaitForSeconds(TURN_DURATION);
                    Debug.Log("Game Over");

                }
            }
        }

    }

    private IEnumerator RunRoutine()
    {
        if(state == BattleState.Battle)
        {
            if(Random.Range(1, 101) >= RUN_CHANCE)
            {
                bottomText.text = SUCCESFULLY_RAN_MESSAGE;
                state = BattleState.Run;
                yield return new WaitForSeconds(TURN_DURATION);
                SceneManager.LoadScene(OVERWORLD_SCENE);
                yield break;
            }
            else
            {
                bottomText.text = UNSUCCESFULLY_RAN_MESSAGE;
                yield return new WaitForSeconds(TURN_DURATION);
            }
        }
    }

    private void RemoveDeadBattlers()
    {
        for(int i = 0; i < allBattlers.Count; ++i)
        {
            if (allBattlers[i].currHealth <= 0)
            {
                allBattlers.RemoveAt(i);
            }
        }
    }

    private void CreatePartyEntities()
    {
        List<PartyMember> currentParty = partyManager.GetAliveParty();

        for (int i = 0; i < currentParty.Count; ++i)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentParty[i].memberName, currentParty[i].currHealth, currentParty[i].maxHealth, currentParty[i].initiative, currentParty[i].strength, currentParty[i].level, true, i);
            Debug.Log("party " + i);
            BattleVisuals tempBattleVisuals = Instantiate(currentParty[i].memberBattleVisualPrefab, partySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(currentParty[i].currHealth, currentParty[i].maxHealth, currentParty[i].level, currentParty[i].currExp, currentParty[i].maxExp);
            tempEntity.battleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            playerBattlers.Add(tempEntity);
        }
    }

    private void CreateEnemyEntities()
    {
        List<Enemy> currentEnemies = enemyManager.GetCurrentEnemies();

        for (int i = 0; i < currentEnemies.Count; ++i)
        {
            BattleEntities tempEntity = new BattleEntities();
            tempEntity.SetEntityValues(currentEnemies[i].enemyName, currentEnemies[i].currHealth, currentEnemies[i].maxHealth, currentEnemies[i].initiative, currentEnemies[i].strength, currentEnemies[i].level, false, i);
            Debug.Log("enemy " + i);
            BattleVisuals tempBattleVisuals = Instantiate(currentEnemies[i].enemyVisualPrefab, enemySpawnPoints[i].position, Quaternion.identity).GetComponent<BattleVisuals>();

            tempBattleVisuals.SetStartingValues(currentEnemies[i].maxHealth, currentEnemies[i].maxHealth, currentEnemies[i].level, 0, 0);
            tempEntity.battleVisuals = tempBattleVisuals;

            allBattlers.Add(tempEntity);
            enemyBattlers.Add(tempEntity);
        }
    }

    public void ShowBattleMenu()
    {
        actionText.text = playerBattlers[currentPlayer].name + ACTION_MESSAGE;
        battleMenu.SetActive(true);
        Debug.Log(currentPlayer);
        cameraTargetPosition = partySpawnPoints[currentPlayer].position + new Vector3(0, 2.5f, -2.5f);
    }


    public void ShowEnemySelectionMenu()
    {
        battleMenu.SetActive(false);
        SetEnemySelectionButtons();
        enemySelectionMenu.SetActive(true);
    }

    private void SetEnemySelectionButtons()
    {
        for (int i = 0; i < enemySelectionButtons.Length; ++i)
        {
            enemySelectionButtons[i].SetActive(false);
        }

        for (int i = 0; i < enemyBattlers.Count; ++i)
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

        if (currentPlayer >= playerBattlers.Count)
        {
            StartCoroutine(BattleRoutine());
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
        currAttacker.battleVisuals.GetComponentInParent<GameObject>().transform.position = partyAttackPoints[currTarget.pointIndex].position;
        currAttacker.battleVisuals.PlayAttackAnimation();
        currTarget.currHealth -= damage;
        currTarget.battleVisuals.PlayHitAnimation();
        currTarget.UpdateUI();
        currAttacker.battleVisuals.GetComponentInParent<GameObject>().transform.position = partySpawnPoints[currAttacker.pointIndex].position;
        bottomText.text = string.Format("{0} が {1} を攻撃し、{2} ダメージを与えました", currAttacker.name, currTarget.name, damage);
        SaveHealth();
    }

    private int GetRandomPartyMember()
    {
        List<int> partyMembers = new List<int>();

        for (int i = 0; i < allBattlers.Count; ++i)
        {
            if (allBattlers[i].isPlayer && allBattlers[i].currHealth > 0)
            {
                partyMembers.Add(i);
            }
        }

        return partyMembers[Random.Range(0, partyMembers.Count)];
    }

    private int GetRandomEnemy()
    {
        List<int> enemies = new List<int>();

        for (int i = 0; i < allBattlers.Count; ++i)
        {
            if (!allBattlers[i].isPlayer && allBattlers[i].currHealth > 0)
            {
                enemies.Add(i);
            }
        }

        return enemies[Random.Range(0, enemies.Count)];
    }

    private void SaveHealth()
    {
        for(int i = 0; i < playerBattlers.Count; ++i)
        {
            partyManager.SaveHealth(i, playerBattlers[i].currHealth);
        }
    }

    private void DetermineBattleOrder()
    {
        allBattlers.Sort((bi1, bi2) => -bi1.initiative.CompareTo(bi2.initiative));
    }

    public void SelectRunAction()
    {
        state = BattleState.Selection;
        BattleEntities currentPlayerEntity = playerBattlers[currentPlayer];

        currentPlayerEntity.battleAction = BattleEntities.Action.Run;

        battleMenu.SetActive(false);
        ++currentPlayer;

        if (currentPlayer >= playerBattlers.Count)
        {
            StartCoroutine(BattleRoutine());
        }
        else
        {
            enemySelectionMenu.SetActive(false);
            ShowBattleMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTargetPosition, 2.0f * Time.deltaTime);
    }
}


[System.Serializable]
public class BattleEntities
{
    public enum Action
    {
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
    public int pointIndex;

    public void SetEntityValues(string name, int currHealth, int maxHealth, int initiative, int strength, int level, bool isPlayer, int pointIndex)
    {
        this.name = name;
        this.currHealth = currHealth;
        this.maxHealth = maxHealth;
        this.initiative = initiative;
        this.strength = strength;
        this.level = level;
        this.isPlayer = isPlayer;
        this.pointIndex = pointIndex;
    }

    public void SetTarget(int target)
    {
        this.target = target;
    }

    public void UpdateUI()
    {
        battleVisuals.ChangeHealth(currHealth);
    }
}