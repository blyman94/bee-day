using System.Collections;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    #region Field Declarations
    public string canHitTag;
    public Transform shotOrigin;

    [SerializeField] protected GameObject projectilePrefab;
	[SerializeField] protected int projectilePoolSize;
	[SerializeField] protected float reloadTime;
    [SerializeField] protected AudioSource weaponAudio;
    [SerializeField] protected AudioClip weaponEmptyClip;
    [SerializeField] protected AudioClip weaponReloadClip;
    [SerializeField] protected AudioClip weaponReadyClip;
    [HideInInspector] public float shotRotation;

    protected GameObject[] projectilePool;
    public bool useShotRotation = false;
    public bool canShoot = true;
    #endregion

    #region Delegate Declarations
    public delegate void WeaponCooldownUpdate(float currentCooldownPercentage);
    public WeaponCooldownUpdate weaponCooldownUpdate;
    #endregion

    #region Start Up
    protected virtual void Start()
    {
        projectilePool = new GameObject[projectilePoolSize];
        for (int i = 0; i < projectilePool.Length; i++)
        {
            projectilePool[i] = Instantiate(projectilePrefab, shotOrigin);
            projectilePool[i].GetComponent<Projectile>().hitTag = canHitTag;
            projectilePool[i].SetActive(false);
        }
    }
    #endregion

    #region Behaviour Methods
    protected GameObject GetProjectile()
    {
        for (int i = 0; i < projectilePool.Length; i++)
        {
            if (!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }
        return null;
    }
    public abstract void FireProjectile();

    protected virtual IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        weaponAudio.PlayOneShot(weaponReloadClip, 1);
        float elapsedTime = 0;
        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            weaponCooldownUpdate?.Invoke(elapsedTime / reloadTime);
            yield return null;
        }
        weaponCooldownUpdate?.Invoke(1.0f);
        canShoot = true;
        weaponAudio.PlayOneShot(weaponReadyClip, 1);
    }
    #endregion
}
