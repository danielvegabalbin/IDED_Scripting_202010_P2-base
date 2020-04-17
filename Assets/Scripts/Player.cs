using System;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class Player : MonoBehaviour
{
    public const int PLAYER_LIVES = 3;

    private const float PLAYER_RADIUS = 0.4F;

    [Header("Movement")]
    [SerializeField]
    private float moveSpeed = 1F;

    private float hVal;

    #region Bullet

    [Header("Bullet")]
    [SerializeField]
    private GameObject bullet;

    [SerializeField]
    private Transform bulletSpawnPoint;

    [SerializeField]
    private float bulletSpeed = 3F;

    #endregion Bullet

    #region BoundsReferences

    private float referencePointComponent;
    private float leftCameraBound;
    private float rightCameraBound;

    #endregion BoundsReferences
    public delegate void perderVida();
    public static event perderVida OnPlayerHit;

    public delegate void perdida();
    public static event perdida EOnPlayerDied;

    public delegate void changeScore(int scoreToAdd);
    public static event changeScore OnPlayerScoreChanged;
    #region StatsProperties

    public int Score { get; set; }
    public int Lives { get; set; }

    #endregion StatsProperties

    #region MovementProperties

    public bool ShouldMove
    {
        get =>
            (hVal != 0F && InsideCamera) ||
            (hVal > 0F && ReachedLeftBound) ||
            (hVal < 0F && ReachedRightBound);
    }

    private bool InsideCamera
    {
        get => !ReachedRightBound && !ReachedLeftBound;
    }

    private bool ReachedRightBound { get => referencePointComponent >= rightCameraBound; }
    private bool ReachedLeftBound { get => referencePointComponent <= leftCameraBound; }

    private bool CanShoot { get => bulletSpawnPoint != null && bullet != null; }

    #endregion MovementProperties
    #region Pooling
    private GameObject[] spawnedObjectsPool;
    private int MaxObjectsAllowed = 5;
    int poolPositioner = 0;
    int poolSelectioner = 0;

    #endregion

    public Action OnPlayerDied;

    // Start is called before the first frame update
    private void Start()
    {
        
        spawnedObjectsPool = new GameObject[MaxObjectsAllowed];
        leftCameraBound = Camera.main.ViewportToWorldPoint(new Vector3(
            0F, 0F, 0F)).x + PLAYER_RADIUS;

        rightCameraBound = Camera.main.ViewportToWorldPoint(new Vector3(
            1F, 0F, 0F)).x - PLAYER_RADIUS;

        Lives = PLAYER_LIVES;
    }
    public void PlayerDeathMetod() {
        
        EOnPlayerDied();


    }
    public void ScoreChangedMethod(int score) {

        Score += score;
        OnPlayerScoreChanged(score);


    }
    public void playerHitMetod() {
        Lives--;
        OnPlayerHit();
    }
    // Update is called once per frame
    private void Update()
    {
        if (Lives <= 0)
        {
            this.enabled = false;
            gameObject.SetActive(false);
        }
        else
        {
            hVal = Input.GetAxis("Horizontal");

            if (ShouldMove)
            {
                transform.Translate(transform.right * hVal * moveSpeed * Time.deltaTime);
                referencePointComponent = transform.position.x;
            }

            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                && CanShoot)
            {
                if (poolPositioner <= MaxObjectsAllowed - 1)
                {
                    Debug.Log(poolPositioner);
                    GameObject newbullet= Instantiate(bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
                    newbullet.GetComponent<Rigidbody>().AddForce(transform.up * bulletSpeed, ForceMode.Impulse);
                    spawnedObjectsPool[poolPositioner] = newbullet;
                    poolPositioner++;

                }
                else
                {
                    SpawnFromPool();
                }
               
            }
        }
    }
    public void SpawnFromPool()
    {

        if (spawnedObjectsPool[poolSelectioner] != null)
        {
            spawnedObjectsPool[poolSelectioner].transform.position = bulletSpawnPoint.position;
            spawnedObjectsPool[poolSelectioner].GetComponent<Rigidbody>().velocity = Vector3.zero;
           spawnedObjectsPool[poolSelectioner].GetComponent<Rigidbody>().AddForce(transform.up * bulletSpeed, ForceMode.Impulse);
            poolSelectioner++;
            Debug.Log("spawningFromPool");
        }
        if (poolSelectioner > MaxObjectsAllowed - 1)
        {
            Debug.Log("poolreestart");
            poolSelectioner = 0;
        }

    }
}