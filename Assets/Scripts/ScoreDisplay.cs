using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    //slider for the health

    [SerializeField] Slider healthSlider;
    [SerializeField] TMPro.TextMeshProUGUI scoreText;
    [SerializeField] GameObject levelCompletePanel;
    [SerializeField] GameObject levelFailedPanel;

    Health playerHealth;


    void Awake()
    {
        playerHealth = FindObjectOfType<Health>();
    }

    void Start()
    {
        ResetScoreDisplay();
    }

    public void ResetScoreDisplay()
    {
        if (!GameManager.Instance)
        {
            Debug.LogError("GameManager not found in the scene");
            return;
        }
        if (!GameManager.Instance.ScoreKeeper)
        {
            Debug.LogError("ScoreKeeper not found in the scene");
            return;
        }
        if (!GameManager.Instance.PlayerSettings)
        {
            Debug.LogError("PlayerSettings not found in the scene");
            return;
        }
        UpdateScoreText(GameManager.Instance.ScoreKeeper.Score);
        UpdateHealth(GameManager.Instance.PlayerSettings.MaxHealthValue);

        levelCompletePanel.SetActive(false);
        levelFailedPanel.SetActive(false);
    }

    public void ShowLevelCompletedUI()
    {
        levelCompletePanel.SetActive(true);
    }

    public void ShowLevelFailedUI()
    {
        levelFailedPanel.SetActive(true);
    }

    private void UpdateScoreText(int score)
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void OnRestartLevel()
    {
        GameManager.Instance.RestartLevel();
        Debug.Log("Next Level");
    }

    public void OnNextLevel()
    {
        GameManager.Instance.NextLevel();
        Debug.Log("Next Level");
    }

    private void UpdateHealth(float health)
    {
        //display life in % of maxLifeValue on the health slider
        healthSlider.value = health / GameManager.Instance.PlayerSettings.MaxHealthValue;
    }

    void Update()
    {
        //do this once every 25 frames
        if (Time.frameCount % 25 == 0)
        {
            UpdateScoreText(GameManager.Instance.ScoreKeeper.Score);
            UpdateHealth(playerHealth.HealthValue);
        }
    }
}
