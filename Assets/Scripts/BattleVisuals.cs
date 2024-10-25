using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleVisuals : MonoBehaviour
{
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider expBar;
    [SerializeField] private TextMeshProUGUI levelText;

    private int currHealth;
    private int maxHealth;
    private int level;
    private int exp;
    private int maxExp;

    private Animator anim;

    private const string LEVEL_ABB = "Lvl: ";

    private const string IS_ATTACK_PARAM = "IsAttack";
    private const string IS_HIT_PARAM = "IsHit";
    private const string IS_DEAD_PARAM = "IsDead";

    // Start is called before the first frame update
    void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    public void SetStartingValues(int currHealth, int maxHealth, int level, int exp, int maxExp)
    {
        this.currHealth = currHealth;
        this.maxHealth = maxHealth;
        this.level = level;
        this.exp = exp;
        this.maxExp = maxExp;
        levelText.text = LEVEL_ABB + this.level.ToString();
        UpdateHealthBar();
        UpdateExpBar();
    }

    public void ChangeHealth(int currHealth)
    {
        this.currHealth = currHealth;
        if(currHealth <= 0)
        {
            PlayDeathAnimation();
            Destroy(gameObject, 1f);
        }

        UpdateHealthBar();
    }

    public void ChangeExp(int exp)
    {
        this.exp = exp;
        if(exp >= this.maxExp)
        {
            this.exp = 0;
        }

        UpdateExpBar();
    }

    // Update is called once per frame
    void UpdateHealthBar()
    {
        healthBar.maxValue = maxHealth;
        healthBar.value = currHealth;
    }

    void UpdateExpBar()
    {
        expBar.maxValue = maxExp;
        expBar.value = exp;
    }

    public void PlayAttackAnimation()
    {
        anim.SetTrigger(IS_ATTACK_PARAM);
    }

    public void PlayHitAnimation()
    {
        anim.SetTrigger(IS_HIT_PARAM);
    }

    public void PlayDeathAnimation()
    {
        anim.SetTrigger(IS_DEAD_PARAM);
    }
}
