using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehaviour : MonoBehaviour {

    public float speed = 20f;
    public float speedGain = 25f; //the ball will be faster after each collide with the paddle

    public float ghostRate = 0.1f; // the ball moving will leave a trail behind
    public GameObject ghost;

    Vector3 speedVector;

    public float ballsize = 1;
    Rigidbody2D rb;

    int xDirection = 1;
    int yDirection = 1;

    private bool isRolling = true;
    private bool isBouncingVertically = true;

    void Start() {
        rb = GetComponent<Rigidbody2D>();

        InvokeRepeating("leaveGhost", 0.5f, ghostRate); // ,immediately leaving trail causes jittering, so there's a half second delay
    }

    void startRolling() {
        flipCoin();
        speedVector = new Vector3(speed * xDirection, speed * yDirection, 0);
        isRolling = true;
    }

    void stopRolling() {
        isRolling = false;
    }

    void stopBouncingVertically() {
        isBouncingVertically = false;
    }



    void Update() {
        //rb.velocity = transform.right * speed * Time.deltaTime;

        if (isRolling) { transform.Translate(speedVector * Time.deltaTime); }

        // bounce agaisnt sides of the screen
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        if (transform.position.x - ballsize / 2 < bottomLeft.x || transform.position.x + ballsize / 2 > topRight.x) {

            //reposition the ball before anything else
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, bottomLeft.x + ballsize / 2, topRight.x - ballsize / 2),
                transform.position.y, transform.position.z);

            //bounce it off
            //transform.Rotate(0, 0, transform.rotation.z + 90);
            speedVector = new Vector3(speedVector.x * -1f, speedVector.y, speedVector.z);
        }

        if (isBouncingVertically) {
            if (transform.position.y + ballsize / 2 < bottomLeft.y || transform.position.y - ballsize / 2 > topRight.y) { 
                transform.position = new Vector3(transform.position.x,
                Mathf.Clamp(transform.position.y, bottomLeft.y + ballsize / 2, topRight.y - ballsize / 2),
                transform.position.z);

                speedVector = new Vector3(speedVector.x, speedVector.y * -1f, speedVector.z);

                Debug.Log("Ball bounced vertically");
            }
  
        }


    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("paddle")) {

            //collision resolution
            if (transform.position.y > other.gameObject.transform.position.y) {
                transform.position = new Vector3(transform.position.x, other.gameObject.transform.position.y + gameObject.transform.lossyScale.y / 2 + ballsize, transform.position.z);
                speedVector = new Vector3(speedVector.x, (speedVector.y - speedGain) * -1f, speedVector.z);
                Debug.Log("Ball collided with bottom paddle");
            } else {
                transform.position = new Vector3(transform.position.x, other.gameObject.transform.position.y - gameObject.transform.lossyScale.y / 2 - ballsize, transform.position.z);
                speedVector = new Vector3(speedVector.x, (speedVector.y + speedGain) * -1f, speedVector.z);
                Debug.Log("Ball collided with top paddle");
            }
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
            yDirection = -1;
        }
    }

    void leaveGhost() {
        Instantiate(ghost, transform.position, transform.rotation);
    }
}
