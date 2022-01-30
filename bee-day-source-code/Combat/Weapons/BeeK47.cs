using System.Collections;
using UnityEngine;

public class BeeK47 : Weapon
{
    [SerializeField] private AudioClip weaponClip;
    [HideInInspector] public float shotCooldown = 0.1f;
    private readonly int clipSize = 30;
    private int currentClip;

    #region Delegate Declarations
    public delegate void UseBullet();
    public UseBullet useBullet;
    #endregion

    #region Start Up
    protected override void Start()
    {
        base.Start();
        useShotRotation = true;
        currentClip = clipSize;
    }
    #endregion

    #region Abstract Class Implementations
    public override void FireProjectile()
    {
        GameObject projectileGO = GetProjectile();
        if (projectileGO != null)
        {
            if (canShoot && currentClip > 0)
            {
                canShoot = false;
                weaponAudio.PlayOneShot(weaponClip, 1);
                Rigidbody2D projectileRb;
                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectileGO.transform.localPosition = new Vector3(0, 0, -1);
                projectileGO.transform.rotation = Quaternion.Euler(0, 0, shotRotation);
                projectileGO.transform.parent = null;
                projectileGO.SetActive(true);
                projectileRb = projectileGO.GetComponent<Rigidbody2D>();
                projectileRb.AddForce(projectileGO.transform.up * projectile.projectileShotForce, ForceMode2D.Impulse);
                currentClip -= 1;
                useBullet?.Invoke();
                if(currentClip < 1)
                {
                    canShoot = false;
                    StartCoroutine(ReloadRoutine());
                }
                StartCoroutine(ShotCooldown());
            }
            else
            {
                if (!weaponAudio.isPlaying)
                {
                    weaponAudio.PlayOneShot(weaponEmptyClip, 1);
                }
            }
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
    }
    protected override IEnumerator ReloadRoutine()
    {
        weaponAudio.PlayOneShot(weaponReloadClip, 1);
        float elapsedTime = 0;
        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            weaponCooldownUpdate?.Invoke(elapsedTime / reloadTime);
            yield return null;
        }
        weaponCooldownUpdate?.Invoke(1.0f);
        currentClip = clipSize;
        canShoot = true;
        weaponAudio.PlayOneShot(weaponReadyClip, 1);
    }
    #endregion
}
