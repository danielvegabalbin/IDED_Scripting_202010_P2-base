using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnObjects;

    [SerializeField]
    private float spawnRate = 1f;

    [SerializeField]
    private float firstSpawnDelay = 0f;

    [SerializeField]
    private Player player;

    private Vector3 spawnPoint;
    #region Pooling
    private GameObject[] spawnedObjectsPool;
    private int MaxObjectsAllowed=5;
    int poolPositioner = 0;
    int poolSelectioner = 0;

    #endregion
    private bool IsThereAtLeastOneObjectToSpawn
    {
        get
        {
            bool result = false;

            for (int i = 0; i < spawnObjects.Length; i++)
            {
                result = spawnObjects[i] != null;

                if (result)
                {
                    break;
                }
            }

            return result;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
     //   poolPositioner = 0;
        spawnedObjectsPool = new GameObject[MaxObjectsAllowed];
        if (spawnObjects.Length > 0 && IsThereAtLeastOneObjectToSpawn)
        {
            InvokeRepeating("SpawnObject", firstSpawnDelay, spawnRate);

            if (player != null)
            {
                player.OnPlayerDied += StopSpawning;
            }
        }
    }
    
    private void SpawnObject()
    {
        GameObject spawnGO = spawnObjects[Random.Range(0, spawnObjects.Length)];

        if (spawnGO != null)
        {
            spawnPoint = Camera.main.ViewportToWorldPoint(new Vector3(
                Random.Range(0F, 1F), 1F, transform.position.z));


            if (poolPositioner <= MaxObjectsAllowed-1)
            {
                Debug.Log(poolPositioner);
                GameObject instance = Instantiate(spawnGO, spawnPoint, Quaternion.identity);
                spawnedObjectsPool[poolPositioner] = instance;
                poolPositioner++;
                
            }
            else
            {
                SpawnFromPool();
            }
            
        }
    }
    public void SpawnFromPool() {
       
        if (spawnedObjectsPool[poolSelectioner]!= null) { 
        spawnedObjectsPool[poolSelectioner].transform.position = spawnPoint;
            spawnedObjectsPool[poolSelectioner].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        poolSelectioner++;
        Debug.Log("spawningFromPool");
        }
        if (poolSelectioner > MaxObjectsAllowed - 1) {
            Debug.Log("poolreestart");
            poolSelectioner = 0;
        }
        
    }
    private void StopSpawning()
    {
        CancelInvoke();
    }
}