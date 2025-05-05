using UnityEngine;
using System.Collections;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour{
    [SerializeField] private float speed;
    [SerializeField] private int cadence;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private int scorePerHit;
    public Player player { get; set; }
    public ObjectPool<Enemy> myPool { get; set; }
    public static ObjectPool<Bullet> bulletPool { get; set; }
    private Spawner spawner;

    public void setSpeed(float newSpeed){ speed = newSpeed; }
    public float getSpeed(){ return speed; }

    public void setSpawner(Spawner spawner){ this.spawner = spawner; }
    public Spawner getSpawner(){ return spawner; }

    // Update is called once per frame
    void Update(){
        transform.Translate(new Vector2(-1, 0) * speed * Time.deltaTime);
        if (transform.position.x <= -10){ deleteEnemy(); }
    }

    public void startShooting(){
        if (Enemy.bulletPool == null){ Enemy.bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet); }
        StartCoroutine(shoot());
    }

    IEnumerator shoot(){
        while(true){
            if(gameObject.activeSelf){
                Bullet newBullet = Enemy.bulletPool.Get();
                newBullet.transform.position = spawnPoint.transform.position;
                newBullet.gameObject.SetActive(true);
                yield return new WaitForSeconds(cadence);
            }
        }
    }

    public void deleteEnemy(){
        spawner.increaseFoeCounter();
        transform.position = new Vector2(transform.position.x, -10);
        myPool.Release(this);
    }

    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("PlayerBullet")){
            other.GetComponent<Bullet>().deleteBullet();
            if (gameObject.activeSelf) {
                deleteEnemy();
                if (player != null) { player.increaseScore(scorePerHit); }
            }
        }
    }

    private Bullet CreateBullet(){
        Bullet newBullet = Instantiate(bulletPrefab, spawnPoint.transform.position, Quaternion.identity);
        newBullet.myPool = bulletPool;
        return newBullet;
    }
    private void OnGetBullet(Bullet bullet){
        bullet.transform.position = new Vector2(transform.position.x, Random.Range(-4.4f, 4.4f)); // Reset the position of the enemy
    }
    private void OnReleaseBullet(Bullet bullet){
        bullet.gameObject.SetActive(false);
    }
    private void OnDestroy(){ bulletPool.Dispose(); }
}
