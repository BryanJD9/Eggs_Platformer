using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button level1Button;
    [SerializeField] private Button bossLevelButton;

    private void Start()
    {
        // Hook up button events
        if (level1Button != null)
            level1Button.onClick.AddListener(LoadLevel1);

        if (bossLevelButton != null)
            bossLevelButton.onClick.AddListener(LoadBossLevel);
    }

    private void LoadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    private void LoadBossLevel()
    {
        SceneManager.LoadScene("BossLevel");
    }
}
