using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using TMPro;

public class Spawner : MonoBehaviour{
    [SerializeField] private Enemy enemyPrefab; // Prefab of the enemy to be instantiated
    [SerializeField] private Player playerPrefab;
    [SerializeField] private TextMeshProUGUI textoOleadas;

    private int foeCounter = 0; // Counter of enemies spawned

    private ObjectPool<Enemy> enemyPool;

    void Start(){
        enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy); // Initialize the enemy pool with a size of 10
        StartCoroutine(SpawnEnemies());
    }

    public void increaseFoeCounter(){
        foeCounter++;
        Debug.Log("Foe Counter: " + foeCounter);
    }

    IEnumerator SpawnEnemies(){
        for(int i = 0; i < 5; i++){
            for(int j = 0; j < 3; j++){
                textoOleadas.text = "Nivel " + (i+1) + " - Oleada " + (j+1);
                foeCounter = 0;
                yield return new WaitForSeconds(2f);
                textoOleadas.text = "";
                for(int k = 0; k < 5+3*j+2*i; k++){
                    Enemy enemy = enemyPool.Get();
                    enemy.setSpawner(this);
                    enemy.player = playerPrefab;
                    enemy.setSpeed(1f + (j*0.1f));
                    yield return new WaitForSeconds(0.5f);
                }
                while(foeCounter < 5+3*j+2*i){ yield return new WaitForSeconds(0.5f); }
                textoOleadas.text = "¡Oleada Completa!";
                yield return new WaitForSeconds(2f);

                int bonus = playerPrefab.getLifes();
                playerPrefab.increaseScore(bonus);
                textoOleadas.text = "¡+"+ bonus + " puntos extra por la vida restante!";

                yield return new WaitForSeconds(3f);
            }
            textoOleadas.text = "¡NUEVO NIVEL!";
            yield return new WaitForSeconds(3f);
        }
    }

    private Enemy CreateEnemy(){
        Vector2 randomPoint = new Vector2(transform.position.x, Random.Range(-4.4f, 4.4f));
        Enemy enemy = Instantiate(enemyPrefab, randomPoint, Quaternion.identity);
        enemy.myPool = enemyPool;
        return enemy; // Return the instantiated enemy
    }

    private void OnGetEnemy(Enemy enemy){
        enemy.transform.position = new Vector2(transform.position.x, Random.Range(-4.4f, 4.4f)); // Reset the position of the enemy
        enemy.gameObject.SetActive(true);
        enemy.startShooting();
    }
    private void OnReleaseEnemy(Enemy enemy){
        enemy.gameObject.SetActive(false);
    }

    private void OnDestroy(){ enemyPool.Dispose(); }
}
