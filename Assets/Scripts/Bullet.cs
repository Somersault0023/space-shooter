using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour{
    [SerializeField] private float speed;
    [SerializeField] private Vector2 direction; // Cadence of the enemy
    public Player player { get; set;}
    public ObjectPool<Bullet> myPool { get; set; } // Pool of bullets

    // Update is called once per frame
    void Update(){
        bool isPlayerBulletOffscreen = this.CompareTag("PlayerBullet") && transform.position.x >=  10;
        bool isEnemyBulletOffscreen  = this.CompareTag("EnemyBullet")  && transform.position.x <= -10;
        if (isPlayerBulletOffscreen || isEnemyBulletOffscreen){
            transform.position = new Vector2(transform.position.x, -10);
            if (myPool != null) { deleteBullet(); }
        }
        else{ transform.Translate(direction * speed * Time.deltaTime); }
    }

    public void deleteBullet(){ myPool.Release(this); }
}
