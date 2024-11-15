using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;

    [SerializeField] private PartyMemberInfo defualtPartyMember;

    private Vector3 playerPosition;
    private static GameObject instance;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this.gameObject;
            AddMemberToPartyByName(defualtPartyMember.memberName);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Length; ++i)
        {
            if (allMembers[i].memberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.memberName = allMembers[i].memberName;
                newPartyMember.level = allMembers[i].startingLevel;
                newPartyMember.baseHealth = allMembers[i].baseHealth;
                newPartyMember.currHealth = allMembers[i].baseHealth + Mathf.RoundToInt(0.5f * allMembers[i].startingLevel * allMembers[i].baseHealth);
                newPartyMember.maxHealth = newPartyMember.currHealth;
                newPartyMember.baseStrength = allMembers[i].baseStr;
                newPartyMember.strength = allMembers[i].baseStr + Mathf.RoundToInt(0.5f * allMembers[i].startingLevel * allMembers[i].baseStr);
                newPartyMember.baseInitiative = allMembers[i].baseInitiative;
                newPartyMember.initiative = allMembers[i].baseInitiative + Mathf.RoundToInt(0.5f * allMembers[i].startingLevel * allMembers[i].baseInitiative);
                newPartyMember.currExp = allMembers[i].currExp;
                newPartyMember.levelUpExp = allMembers[i].levelUpExp + Mathf.RoundToInt(0.5f * allMembers[i].startingLevel * allMembers[i].levelUpExp);
                newPartyMember.memberBattleVisualPrefab = allMembers[i].memberBattleVisualPrefab;
                newPartyMember.memberOverworldVisualPrefab = allMembers[i].memberOverworldVisualPrefab;

                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetAliveParty()
    {
        List<PartyMember> aliveParty = new List<PartyMember>();
        aliveParty = currentParty;
        for(int i = 0; i < aliveParty.Count; ++i)
        {
            if (aliveParty[i].currHealth <= 0)
            {
                aliveParty.RemoveAt(i);
            }
        }
        return aliveParty;
    }

    public List<PartyMember> GetCurrentParty()
    {
        return currentParty;
    }

    public void SaveHealth(int partyMember, int health)
    {
        currentParty[partyMember].currHealth = health;
    }

    public void SetPosition(Vector3 position)
    {
        playerPosition = position;
    }

    public Vector3 GetPosition()
    {
        return playerPosition;
    }
}

[System.Serializable]
public class PartyMember
{
    public string memberName;
    public int level;
    public int baseHealth;
    public int currHealth;
    public int maxHealth;
    public int baseStrength;
    public int strength;
    public int baseInitiative;
    public int initiative;
    public int baseExp;
    public int currExp;
    public int levelUpExp;
    public GameObject memberBattleVisualPrefab;
    public GameObject memberOverworldVisualPrefab;
}