using UnityEngine;

public class Parallax : MonoBehaviour{
    [SerializeField] private float speed; // Speed of the parallax effect
    [SerializeField] private float imgWidth; // Width of the image for wrapping effect
    [SerializeField] private Vector3 direction; // Direction of the parallax effect

    private Vector3 startPos; // Starting position of the object
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    // Update is called once per frame
    void Update(){
        transform.position = startPos + direction * ((speed * Time.time) % imgWidth);
    }
}
