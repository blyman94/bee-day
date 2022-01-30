using UnityEngine;

public class DesertBeegle : Weapon
{
    [SerializeField] private AudioClip weaponClip;

    #region Start Up
    protected override void Start()
    {
        base.Start();
        useShotRotation = true;
    }
    #endregion

    #region Abstract Class Implementations
    public override void FireProjectile()
	{
        GameObject projectileGO = GetProjectile();
        if (projectileGO != null)
        {
            if (canShoot)
            {
                if(weaponClip != null)
                {
                    weaponAudio.PlayOneShot(weaponClip, 1);
                }
                canShoot = false;
                Rigidbody2D projectileRb;
                Projectile projectile = projectileGO.GetComponent<Projectile>();
                projectileGO.transform.localPosition = new Vector3(0, 0, -1);
                projectileGO.transform.rotation = Quaternion.Euler(0, 0, shotRotation);
                projectileGO.transform.parent = null;
                projectileGO.SetActive(true);
                projectileRb = projectileGO.GetComponent<Rigidbody2D>();
                projectileRb.AddForce(projectileGO.transform.up * projectile.projectileShotForce, ForceMode2D.Impulse);
                StartCoroutine(ReloadRoutine());
            }
            else
            {
                weaponAudio.PlayOneShot(weaponEmptyClip, 1);
            }
        }
    }
	#endregion
}
