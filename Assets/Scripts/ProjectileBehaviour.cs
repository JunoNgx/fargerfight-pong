using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {

    public bool readyToCollide = false;
    private float velocity;
    public ParticleSystem explosion;

    void Start() {
        velocity = 0f;
        Destroy(gameObject, 10f);
    }

    void Update() {
        if (readyToCollide) transform.Translate((new Vector3(0f, velocity, 0)) * Time.deltaTime);

    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("paddle") && readyToCollide) {

            Instantiate(explosion, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            Destroy(gameObject);
            Destroy(other.gameObject);

            Debug.Log("Collided with a paddle.");
        }
    }

    void move(float vel) {
        readyToCollide = true;
        velocity = vel;
    }
}
