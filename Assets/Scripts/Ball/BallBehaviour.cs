using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour {

    public float speed = 10f;
    public float speedGain = 1.5f; //the ball will be faster after each collide with the paddle

    public float ghostRate = 0.1f; // the ball moving will leave a trail behind
    public GameObject ghost;

    public ParticleSystem impactSpark;

    public AudioSource audioSource;
    public AudioClip impactOnWall;
    public AudioClip impactOnPaddle;
    public AudioClip rolling;
  

    Vector3 velocity;

    public float ballsize = 1;
    Rigidbody2D rb;

    int xDirection = 1;
    int yDirection = 1;

    bool isRolling = true;
    bool isBouncingVertically = true;

    Vector3 bottomLeft;
    Vector3 topRight;

    void Start() {
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("leaveGhost", 0.2f, ghostRate); // immediately leaving trail causes jittering, so there's a half second delay
    }

    void startRolling() {
        flipCoin();
        velocity = new Vector3(speed * xDirection, speed * yDirection, 0);
        isRolling = true;

        audioSource.PlayOneShot(rolling);
    }

    void stopRolling() {
        isRolling = false;
    }

    void stopBouncingVertically() {
        isBouncingVertically = false;
    }



    void Update() {

        if (isRolling) { transform.Translate(velocity * Time.deltaTime); }

        bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.x - ballsize / 2 < bottomLeft.x) {
            Instantiate(impactSpark, transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
            repositionX();
            flipX();
            audioSource.PlayOneShot(impactOnWall);
        }

        if (transform.position.x + ballsize / 2 > topRight.x) {
            Instantiate(impactSpark, transform.position, Quaternion.Euler(new Vector3(0, 0, 180)));
            repositionX();
            flipX();
            audioSource.PlayOneShot(impactOnWall);
        }


        if (isBouncingVertically) {
            if (transform.position.y + ballsize / 2 < bottomLeft.y) {
                repositionY();
                flipY();
                Instantiate(impactSpark, transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));
                audioSource.PlayOneShot(impactOnWall);
            }

            if (transform.position.y - ballsize / 2 > topRight.y) {
                repositionY();
                flipY();
                Instantiate(impactSpark, transform.position, Quaternion.Euler(new Vector3(0, 0, -90)));
                audioSource.PlayOneShot(impactOnWall);
            }

        }


    }

    void repositionX() {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x + ballsize / 2, topRight.x - ballsize / 2),
            transform.position.y, transform.position.z);
    }

    void flipX() {
        velocity = new Vector3(velocity.x * -1f, velocity.y, velocity.z);
    }

    void repositionY() {
        transform.position = new Vector3(transform.position.x,
                Mathf.Clamp(transform.position.y, bottomLeft.y + ballsize / 2, topRight.y - ballsize / 2),
                transform.position.z);
    }

    void flipY() {
        velocity = new Vector3(velocity.x, velocity.y * -1f, velocity.z);
    }


    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("paddle")) {

            float newVelX;
            if (velocity.x < 0) {
                newVelX = velocity.x - speedGain;
            } else {
                newVelX = velocity.x + speedGain;
            }

            float newVelY;
            //collision resolution
            if (transform.position.y > other.gameObject.transform.position.y) {
                transform.position = new Vector3(
                    transform.position.x,
                    other.gameObject.transform.position.y + 1 + ballsize/2,
                    transform.position.z
                );
                newVelY = (velocity.y - speedGain) * -1f;

                Instantiate(impactSpark, transform.position, Quaternion.Euler(new Vector3(0, 0, 90)));
                audioSource.PlayOneShot(impactOnPaddle);
                //Debug.Log("Ball collided with bottom paddle");
            } else {
                transform.position = new Vector3(
                    transform.position.x,
                    other.gameObject.transform.position.y - 1 - ballsize/2,
                    transform.position.z
                );
                newVelY = (velocity.y + speedGain) * -1f;

                Instantiate(impactSpark, transform.position, Quaternion.Euler(new Vector3(0, 0, -90)));
                audioSource.PlayOneShot(impactOnPaddle);
                //Debug.Log("Ball collided with top paddle");
            }

            velocity = new Vector3(newVelX, newVelY, velocity.z);
            //Debug.Log(velocity.x + " " + velocity.y);

        }
    }

    void flipCoin() {
        if (Random.value < 0.5) {
            xDirection = -1;
        } else {
            xDirection = 1;
        }

        if (Random.value < 0.5) {
            yDirection = -1;
        } else {
            yDirection = 1;
        }
    }

    void leaveGhost() {
        if (isRolling) Instantiate(ghost, transform.position, transform.rotation);
    }

    public bool isTheBallRolling() {
        return isRolling;
    }

}
