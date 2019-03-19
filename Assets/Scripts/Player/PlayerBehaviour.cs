using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

    public int hp = 2;

    public GameObject paddlePiece;
    public int numberOfPaddle = 5;
    public int paddleWidth = 1;
    public float paddleMargin = 0.5f;

    void Start() {
        
    }

    public void createPaddles() {
        for (int i = 0; i < numberOfPaddle; i++) {

            GameObject paddle = Instantiate(paddlePiece,
                new Vector3(transform.position.x - (paddleWidth / 2) - (i - (numberOfPaddle - 1) / 2) * (paddleWidth + paddleMargin),
                    transform.position.y, transform.position.z),
                    transform.rotation);
            paddle.transform.parent = transform;
        }
    }

    public void destroyPaddles() {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }

    void replacePaddles() {
        destroyPaddles();
        createPaddles();
    }

    void takeDamage() {
        hp -= 1;
    }



}

