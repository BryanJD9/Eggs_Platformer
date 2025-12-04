using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player reached the finish line!");

            if (GameOverManager.Instance != null)
            {
                GameOverManager.Instance.WinGame();
            }
            else
            {
                Debug.LogWarning("GameOverManager instance not found in scene!");
            }
        }
    }
}
