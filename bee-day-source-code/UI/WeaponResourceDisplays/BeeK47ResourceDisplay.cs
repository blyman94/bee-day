using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BeeK47ResourceDisplay : MonoBehaviour
{
	[SerializeField] private Image currentCooldownImage;
    [SerializeField] private BeeK47 beeK47;
    [SerializeField] private TextMeshProUGUI clipText;

    private int currentBullet = 29;

    #region Start Up / Shutdown
    public void OnEnable()
    {
        SceneManager.sceneLoaded += StartLevel;
    }
    public void StartLevel(Scene scene, LoadSceneMode mode)
    {
        beeK47 = GameObject.Find("BeeK-47").GetComponent<BeeK47>();
        beeK47.weaponCooldownUpdate += UpdateCooldown;
        beeK47.useBullet += UpdateClip;
        UpdateCooldown(1);
    }
    private void OnDisable()
    {
        beeK47.weaponCooldownUpdate -= UpdateCooldown;
        beeK47.useBullet -= UpdateClip;
    }
    #endregion

    private void UpdateCooldown(float currentCooldown)
    {
        Color currentColor = Color.white;
        currentColor.a = currentCooldown;
        currentCooldownImage.color = currentColor;
        if (currentColor.a == 1)
        {
            ResetClip();
        }
    }

    private void ResetClip()
    {
        currentBullet = 29;
        clipText.text = "x " + (currentBullet + 1);
    }

    private void UpdateClip()
    {
        currentBullet -= 1;
        clipText.text = "x " + (currentBullet + 1);
    }
}
