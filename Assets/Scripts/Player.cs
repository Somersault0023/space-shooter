using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Player : MonoBehaviour{
    [SerializeField] private float speed; // Speed of the player
    [SerializeField] private float xClampAmplitude; // Speed of the player
    [SerializeField] private float yClampAmplitude; // Speed of the player
    [SerializeField] private float forceMultiplier; // Speed of the player
    [SerializeField] private float shootingRatio; // Speed of the player
    [SerializeField] private bool forceMode; // Speed of the player
    [SerializeField] private Bullet bulletPrefab; // Prefab of the projectile to be instantiated
    [SerializeField] private int lifes = 100; // Damage of the enemy
    [SerializeField] private GameObject hpBar; // Damage of the enemy
    [SerializeField] private TextMeshProUGUI scoreText; // Damage of the enemy
    [SerializeField] private List<GameObject> spawnPoints; // List of spawnPoints
    private int spawnPointIndex = 0; // Index of the spawnPoint to be used
    private int score = 0;

    private Rigidbody2D rb;
    private float timer = 0.5f;
    private ObjectPool<Bullet> bulletPool; // Pool of bullets

    private void Awake(){
        bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet); // Initialize the bullet pool with a size of 10
    }

    public int getLifes(){ return lifes; }

    void Start(){
        rb = GetComponent<Rigidbody2D>(); // Get the Rigidbody2D component
        if (rb == null) {
            Debug.LogError("Rigidbody2D component is missing from the Player GameObject.");
        }

        rb.bodyType = forceMode ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
    }

    void Update(){
        movement();
        delimitMovement();
        shoot();
        updateHealthBar();
    }

    void movement(){
        var vector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(forceMode){
            transform.Translate(vector.normalized * speed * Time.deltaTime);
        }
        else{
            rb.AddForce(forceMultiplier * vector.normalized, ForceMode2D.Impulse);
        }
    }

    void delimitMovement(){
        float xClamped = Mathf.Clamp(transform.position.x, -xClampAmplitude, xClampAmplitude);
        float yClamped = Mathf.Clamp(transform.position.y, -yClampAmplitude, yClampAmplitude);
        if (!forceMode) { RemoveForcesOnClampedAxes(xClamped, yClamped); }
        transform.position = new Vector3(xClamped, yClamped, 0);
    }

    void RemoveForcesOnClampedAxes(float xClamped, float yClamped){
        if (!Mathf.Approximately(transform.position.x, xClamped)) {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        if (!Mathf.Approximately(transform.position.y, yClamped)) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    void shoot(){
        timer += Time.deltaTime;
        if(Input.GetKey(KeyCode.Space) && timer > shootingRatio){
            for (int i = 0; i < 2; i++){
                Bullet newBullet = bulletPool.Get();
                newBullet.player = this;
            }
            timer = 0;
        }
    }

    public void increaseScore(int amount){
        score += amount;
        scoreText.text = "Score: " + score;
    }

    void updateHealthBar(){
        float healthPercentage = lifes / 100f; // Assuming max health is 100
        hpBar.transform.localScale = new Vector3(healthPercentage, hpBar.transform.localScale.y, 1); // Update the scale of the health bar
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("EnemyBullet") || other.CompareTag("Enemy")){
            lifes -= 20;
            if(lifes <= 0){
                Destroy(gameObject);
#if UNITY_EDITOR
            // Stop play mode in the Unity Editor
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // Quit the application in a build
            Application.Quit();
#endif
            }
        }
    }

    private Bullet CreateBullet(){
        Bullet newBullet = Instantiate(bulletPrefab, spawnPoints[spawnPointIndex].transform.position, Quaternion.identity);
        newBullet.myPool = bulletPool;
        spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Count; // Cycle through the spawn points

        return newBullet;
    }

    private void OnGetBullet(Bullet bullet){
        bullet.transform.position = spawnPoints[spawnPointIndex].transform.position;
        spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Count;
        bullet.gameObject.SetActive(true);
    }

    private void OnReleaseBullet(Bullet bullet){
        bullet.gameObject.SetActive(false);
    }

    private void OnDestroy(){
        bulletPool.Dispose(); // Dispose of the bullet pool when the player is destroyed
    }
}