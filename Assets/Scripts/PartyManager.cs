using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;

    [SerializeField] private PartyMemberInfo defualtPartyMember;

    private void Awake()
    {
        AddMemberToPartyByName(defualtPartyMember.memberName);
    }

    public void AddMemberToPartyByName(string memberName)
    {
        for(int i = 0; i < allMembers.Length; ++i)
        {
            if (allMembers[i].memberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();
                newPartyMember.memberName = allMembers[i].memberName;
                newPartyMember.level = allMembers[i].startingLevel;
                newPartyMember.currHealth = allMembers[i].baseHealth;
                newPartyMember.maxHealth = newPartyMember.currHealth;
                newPartyMember.strength = allMembers[i].baseStr;
                newPartyMember.initiative = allMembers[i].baseInitiative;
                newPartyMember.memberBattleVisualPrefab = allMembers[i].memberBattleVisualPrefab;
                newPartyMember.memberOverworldVisualPrefab = allMembers[i].memberOverworldVisualPrefab;

                currentParty.Add(newPartyMember);
            }
        }
    }
}

[System.Serializable]
public class PartyMember
{
    public string memberName;
    public int level;
    public int currHealth;
    public int maxHealth;
    public int strength;
    public int initiative;
    public int currExp;
    public int maxExp;
    public GameObject memberBattleVisualPrefab;
    public GameObject memberOverworldVisualPrefab;
}