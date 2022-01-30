using System.Collections;
using UnityEngine;

public class MultiShooter : MonoBehaviour
{
    #region Field Declarations
    [HideInInspector] public string canHitTag;
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public GameObject[] projectilePool;
    [HideInInspector] public int projectilePoolSize;
    [HideInInspector] public Transform projectileParent;
    [HideInInspector] public Transform[] shotOrigins;
    [HideInInspector] public float shotCooldown;

    private float[] shotRotations;
    [HideInInspector] public bool canShoot = true;
    #endregion

    #region Start Up
    private void Start()
    {
        shotRotations = new float[shotOrigins.Length];

        projectilePool = new GameObject[projectilePoolSize];
        for (int i = 0; i < projectilePool.Length; i++)
        {
            projectilePool[i] = Instantiate(projectilePrefab, projectileParent);
            projectilePool[i].GetComponent<Projectile>().hitTag = canHitTag;
            projectilePool[i].SetActive(false);
        }

        for(int i = 0; i < shotOrigins.Length; i++)
        {
            Vector3 shootDir =  shotOrigins[i].position - projectileParent.position;
            float rotation = (Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg) - 90.0f;
            shotRotations[i] = rotation;
        }
        
    }
    #endregion
    #region Behaviour Methods
    private GameObject[] GetProjectiles()
    {
        GameObject[] chamberedProjectiles = new GameObject[shotOrigins.Length];
        for(int i = 0; i < shotOrigins.Length; i++)
        {
            for (int j = 0; j < projectilePool.Length; j++)
            {
                if (!projectilePool[j].activeInHierarchy)
                {
                    chamberedProjectiles[i] = projectilePool[j];
                    chamberedProjectiles[i].SetActive(true);
                    break;
                }
            }
        }
        return chamberedProjectiles;
    }
    public void FireProjectiles()
    {
        if (canShoot)
        {
            canShoot = false;
            GameObject[] chamberedProjectiles = GetProjectiles();
            Projectile projectile = projectilePrefab.GetComponent<Projectile>();
            for (int i = 0; i < shotOrigins.Length; i++)
            {
                chamberedProjectiles[i].transform.parent = shotOrigins[i];
                chamberedProjectiles[i].transform.rotation = Quaternion.Euler(0, 0, shotRotations[i]);
                chamberedProjectiles[i].transform.localPosition = new Vector3(0, 0, -1);
                Rigidbody2D projectileRb;
                projectileRb = chamberedProjectiles[i].GetComponent<Rigidbody2D>();
                projectileRb.freezeRotation = true;
                chamberedProjectiles[i].SetActive(true);
                chamberedProjectiles[i].transform.parent = null;
                projectileRb.AddForce(chamberedProjectiles[i].transform.up * projectile.projectileShotForce, ForceMode2D.Impulse);
                StartCoroutine(ShotCooldown());
            }
        }
    }
    private IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(shotCooldown);
        canShoot = true;
    }
    #endregion

}
