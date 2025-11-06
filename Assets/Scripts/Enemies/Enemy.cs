using UnityEngine;

public class Enemy : MonoBehaviour
{
    //every enemy would know the player pos.
    public Transform player; // Assign player in inspector

    void Awake()
    {

        if (player == null)
        {
            // Automatically find player if not assigned
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }


    }

  
}
