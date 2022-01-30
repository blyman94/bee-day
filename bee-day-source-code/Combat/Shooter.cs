using System.Collections;
using UnityEngine;

public class Shooter : MonoBehaviour
{
    #region Field Declarations
    [HideInInspector] public string canHitTag;
    [HideInInspector] public float projectileShotForce;
    [HideInInspector] public GameObject projectilePrefab;
    [HideInInspector] public GameObject[] projectilePool;
    [HideInInspector] public int projectilePoolSize;
    [HideInInspector] public Transform shotOrigin;
    [HideInInspector] public float shotRotation;
    [HideInInspector] public float shotCooldown;
    [HideInInspector] public bool canShoot = true;
    #endregion

    #region Behaviour Methods
    
    
    #endregion
}