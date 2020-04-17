using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Target : MonoBehaviour
{
    private const float TIME_TO_DESTROY = 10F;

    [SerializeField]
    private int maxHP = 1;

    private int currentHP;

    [SerializeField]
    private int scoreAdd = 10;
    
    private void Start()
    {
        currentHP = maxHP;
        
    }

  

   
    

    private void OnCollisionEnter(Collision collision)
    {
        int collidedObjectLayer = collision.gameObject.layer;

        if (collidedObjectLayer.Equals(Utils.BulletLayer))
        {
            collision.gameObject.transform.position= new Vector3(0,-100,0);

            currentHP -= 1;

            if (currentHP <= 0)
            {
                Player player = FindObjectOfType<Player>();

                if (player != null)
                {
                    
                   
                    player.ScoreChangedMethod(scoreAdd);
                }

                gameObject.transform.position = new Vector3(0, -50, 0);
            }
        }
        else if (collidedObjectLayer.Equals(Utils.PlayerLayer) || collidedObjectLayer.Equals(Utils.KillVolumeLayer))
        {
            Player player = FindObjectOfType<Player>();

            if (player != null)
            {
                player.playerHitMetod();
              
               
                if (player.Lives <= 0 && player.OnPlayerDied != null)
                {
                    player.PlayerDeathMetod();
                }
            }

            gameObject.transform.position = new Vector3(0, -50, 0);
        }
    }
}