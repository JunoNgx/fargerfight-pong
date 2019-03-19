using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour {

    public bool readyToMove = false;
    public float velocity;
    public ParticleSystem explosion;

    //void Start() {
    //    velocity = 0f;
    //}

    //void Update() {
    //    transform.Translate((new Vector3(0f, velocity, 0)) * Time.deltaTime);

    //}

    //void OnTriggerEnter2D(Collider2D other) {
    //    if (other.gameObject.CompareTag("paddlePiece")) {

    //        Instantiate(explosion);
    //        Destroy(gameObject);
    //        Destroy(other);
    //    }
    //}

    //void setVelocity(float vel) {
    //    velocity = vel;
    //}
}
