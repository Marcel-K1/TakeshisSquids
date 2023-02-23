using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Made by Marcel

public class PlayerUI : MonoBehaviour
{
    [Tooltip("UI Text to display Player's Points")]
    [SerializeField]
    private TextMeshProUGUI playerPointsText;
	[Tooltip("UI Text to display Player's Health")]
	[SerializeField]
	private TextMeshProUGUI playerHealthText;

	[Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;
	[SerializeField]
	private Gradient playerHealthGradient;
	[SerializeField]
	private Image fill;

	[Tooltip("UI Text to display winner")]
	[SerializeField]
	private TextMeshProUGUI winnerText;

    [SerializeField]
    private PlayerManager playerManager;

	[SerializeField]
	private ThirdPersonController playerController;

	private void OnEnable()
    {
		GameManager.Instance.TimeUpEvent += OnTimeUp;
    }

    private void Start()
    {
        playerManager = GetComponentInParent<PlayerManager>();
		playerController = GetComponentInParent<ThirdPersonController>();	
    }

    private void OnDisable()
    {
		if (GameManager.Instance != null)
        {
			GameManager.Instance.TimeUpEvent -= OnTimeUp;
        }
		winnerText.gameObject.SetActive(false);
	}

    public void SetMaxHealth(float health)
	{	
		playerHealthSlider.maxValue = health;
		playerHealthSlider.value = health;

		fill.color = playerHealthGradient.Evaluate(1f);
			
	}

	public void SetHealth(float health)
	{
		playerHealthSlider.value = health;
		fill.color = playerHealthGradient.Evaluate(playerHealthSlider.normalizedValue);
	}

	public void SetPoints(float points)
	{
		playerPointsText.text = $"Points: {points}";
	}

	public void SetHealthText(float health)
	{
		playerHealthText.text = $"Health: {health}";
	}

	//Events
    private void OnTimeUp()
    {
		//Short set up for win or loose screen
		if (!GameManager.Instance.IsSingleplay.IsSingleplay && playerManager.Pv.IsMine)
		{
			winnerText.gameObject.SetActive(true);
			winnerText.text = $"The winner is: {GameManager.Instance.Winner}!";
		}
		else if (GameManager.Instance.IsSingleplay.IsSingleplay)
		{
			winnerText.gameObject.SetActive(true);
			winnerText.text = $"You win with a score of: {GetComponentInParent<ThirdPersonController>().Points} Points!";
		}

    }

}