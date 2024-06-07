using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string memberName;
    public int startingLevel;
    public int baseHealth;
    public int baseStr;
    public int baseInitiative;
    public GameObject memberBattleVisualPrefab;
    public GameObject memberOverworldVisualPrefab;
}
