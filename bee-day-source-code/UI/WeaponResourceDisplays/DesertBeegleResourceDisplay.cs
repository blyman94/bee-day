using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DesertBeegleResourceDisplay : MonoBehaviour
{
	[SerializeField] private Image currentCooldownImage;
    [SerializeField] private DesertBeegle desertBeegle;

    #region Start Up / Shutdown
    public void OnEnable()
    {
        SceneManager.sceneLoaded += StartLevel;
    }
    public void StartLevel(Scene scene, LoadSceneMode mode)
    {
        desertBeegle = GameObject.Find("DesertBeegle").GetComponent<DesertBeegle>();
        desertBeegle.weaponCooldownUpdate += UpdateCooldown;
        UpdateCooldown(1);
    }
    private void OnDisable()
    {
        desertBeegle.weaponCooldownUpdate -= UpdateCooldown;
    }
    #endregion

    private void UpdateCooldown(float currentCooldown)
    {
        Color currentColor = Color.white;
        currentColor.a = currentCooldown;
        currentCooldownImage.color = currentColor;
    }
}
