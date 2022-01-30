using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BeeBombResourceDisplay : MonoBehaviour
{
	[SerializeField] private Image currentCooldownImage;
    [SerializeField] private BeeBomb beeBomb;

    #region Start Up / Shutdown
    public void OnEnable()
    {
        SceneManager.sceneLoaded += StartLevel;
    }
    public void StartLevel(Scene scene, LoadSceneMode mode)
    {
        beeBomb = GameObject.Find("BeeBomb").GetComponent<BeeBomb>();
        beeBomb.weaponCooldownUpdate += UpdateCooldown;
        UpdateCooldown(1);
    }
    private void OnDisable()
    {
        beeBomb.weaponCooldownUpdate -= UpdateCooldown;
    }
    #endregion

    private void UpdateCooldown(float currentCooldown)
    {
        Color currentColor = Color.white;
        currentColor.a = currentCooldown;
        currentCooldownImage.color = currentColor;
    }
}
