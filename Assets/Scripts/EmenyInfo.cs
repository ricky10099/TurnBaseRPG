using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Emeny")]
public class EnemyInfo : ScriptableObject
{
    public string enemyName;
    public int baseHealth;
    public int baseStr;
    public int baseInitiative;
    public int baseExp;
    public GameObject enemyVisualPrefab;
}
