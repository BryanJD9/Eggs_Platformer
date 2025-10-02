using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : MonoBehaviour
{
    [Header("Health UI Circles")]
    public Image[] healthCircles; // Assign 3 Image objects in the inspector
    public Color fullColor = Color.red;   // When the player has health
    public Color emptyColor = Color.white; // When the player lost health

    private void OnEnable()
    {
        PlayerController.OnHealthChanged += UpdateHealthUI;
    }

    private void OnDisable()
    {
        PlayerController.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int currentHealth)
    {
        for (int i = 0; i < healthCircles.Length; i++)
        {
            if (i < currentHealth)
                healthCircles[i].color = fullColor;
            else
                healthCircles[i].color = emptyColor;
        }
    }
}
