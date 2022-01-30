using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Fields
    [HideInInspector] public float maxHealth;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public bool isPoisoned;
    [HideInInspector] public AudioSource damageAudio;
    [HideInInspector] public AudioClip damageClip;

    private float currentHealth;
    private readonly int damageBlinkCount = 1;
    private readonly float damageBlinkSpeed = 0.1f;
    #endregion

    #region Delegate Declarations
    public delegate void Changed();
    public Changed changed;
    public delegate void Full();
    public Changed full;
    public delegate void Recovered();
    public Recovered recovered;
    public delegate void Empty();
    public Empty empty;
    #endregion

    #region Start Up
    private void Start()
    {
        currentHealth = maxHealth;
        full?.Invoke();
    }
    #endregion

    #region Behavior Methods
    public float GetCurrentHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    public void TakeDamage(float damage)
    {
        damageAudio.PlayOneShot(damageClip, 1);
        if(currentHealth - damage > 0)
        {
            currentHealth -= damage;
        }
        else
        {
            currentHealth = 0;
            empty?.Invoke();
        }
        changed?.Invoke();
        StartCoroutine(DamageRoutine());
    }
    public void RestoreHealth(float restoreAmount)
    {
        if (currentHealth + restoreAmount <= maxHealth)
        {
            currentHealth += restoreAmount;
        }
        else
        {
            currentHealth = maxHealth;
        }
        changed?.Invoke();
    }
    #endregion

    #region Coroutines
    public IEnumerator DamageRoutine()
    {
        Color revertColor = isPoisoned ? spriteRenderer.color : Color.white;
        for (int i = 0; i < damageBlinkCount; i++)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(damageBlinkSpeed);
            spriteRenderer.color = revertColor;
            yield return new WaitForSeconds(damageBlinkSpeed);
        }
        spriteRenderer.color = revertColor;
    }
    #endregion
}
