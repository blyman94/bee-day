using UnityEngine;

public class BeeBomb : Weapon
{
    [SerializeField] private AudioClip weaponClip;
    public override void FireProjectile()
	{
        GameObject projectileGO = GetProjectile();
        if (projectileGO != null)
        {
            if (canShoot)
            {
                canShoot = false;
                weaponAudio.PlayOneShot(weaponClip, 1);
                projectileGO.transform.localPosition = new Vector3(0, 0, -1);
                projectileGO.transform.parent = null;
                projectileGO.SetActive(true);
                StartCoroutine(ReloadRoutine());
            }
            else
            {
                weaponAudio.PlayOneShot(weaponEmptyClip, 1);
            }
        }
    }
}
