using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {

    public bool readyToCollide = false;
    private float velocity;
    public ParticleSystem explosion;

    public AudioSource audioSource;
    public AudioClip hitProjectile;
    public AudioClip hitPaddle;
    public AudioClip instantiation;

    void Start() {
        velocity = 0f;
        Destroy(gameObject, 10f);

        //GameObject game = GameObject.FindGameObjectWithTag("GameManager");

        //audioSource.PlayOneShot(instantiation);

        // audioSource = game.GetComponent<AudioSource>();
    }

    void Update() {
        if (readyToCollide) transform.Translate((new Vector3(0f, velocity, 0)) * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("paddle") && readyToCollide) {

            audioSource.PlayOneShot(hitPaddle);

            Instantiate(explosion, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            Destroy(gameObject, 3f);
            Destroy(other.gameObject);

            disableObject();

            //Debug.Log("Collided with a paddle.");


            Camera.main.Shake();
        }

        if (other.gameObject.CompareTag("bullet") && readyToCollide) {

            audioSource.PlayOneShot(hitProjectile);

            Instantiate(explosion, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            Destroy(gameObject, 3f);

            disableObject();

            //Debug.Log("Two bullets have collided.");

        }


    }

    void move(float vel) {
        readyToCollide = true;
        velocity = vel;
    }

    void disableObject () { //using prefabs as audiosources turned out to be more complicated than anticipated
        readyToCollide = false;
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = false;
        
    }
}
