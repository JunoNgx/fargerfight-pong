﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Game : MonoBehaviour {

    public GameObject p1;
    public GameObject p2;
    public GameObject ball;
    private BallBehaviour bBehave;

    private PlayerBehaviour pBh;


    public GameObject halfwayline;

    private int p1hp;
    private int p2hp;

    public Text p1Text;
    public Text p2Text;

    public Text[] MenuTexts;
    public Text[] Tutorial_line1;
    public Text[] Tutorial_line2;
    public Image[] Tutorial_img1;
    public Image[] Tutorial_img2;

    private int gameState = 1; //1 == menu; 2 == play; 3 == end;
    private bool readyToStart = false; //give the game 2 seconds buffer at main menu to avoid accidental press

    public ParticleSystem sSpark;

    GameObject[] bullets;

    public AudioSource audioSource;
    public AudioClip score;
    public AudioClip endgame;

    void Start() {
        //ball.SetActive(true);
        ball.BroadcastMessage("startRolling");
        bBehave = ball.GetComponent<BallBehaviour>();

        StartCoroutine(WaitTWoSecsToBeReady());
        halfwayline.SetActive(false);

        hideTutorialTexts();
    }

    void Update() {
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        //Updating score
        pBh = p1.GetComponent<PlayerBehaviour>(); // Considering removing this. It is extremely convoluted to synchronize the same int at three different places
        p1hp = pBh.hp;

        pBh = p2.GetComponent<PlayerBehaviour>();
        p2hp = pBh.hp;

        switch (gameState) {
            case 1:
                if (Input.GetMouseButtonDown(0) && readyToStart) {

                    foreach (Text item in MenuTexts) {
                        Destroy(item);
                    }
                    gameState = 2;

                    halfwayline.SetActive(true);

                    ball.BroadcastMessage("stopBouncingVertically");
                    ball.BroadcastMessage("stopRolling");
                    StartCoroutine(startTheGame());

                    showTutorial();
                }
                break;

            case 2:
                if (bBehave.isTheBallRolling()) {

                    if (ball.transform.position.y > topRight.y) {
                        //Debug.Log("Player 1 scores");

                        Camera.main.Shake();
                        Instantiate(sSpark, ball.transform.position, Quaternion.Euler(new Vector3(0, 0, 0)));
                        p2.BroadcastMessage("takeDamage");
                        p2hp -= 1;

                        audioSource.PlayOneShot(score);
                        //Debug.Log("P2HP: " + p2hp);
                        if (p2hp > 0) {
                            StartCoroutine(restartTheGame());
                        }
                        else {
                            StartCoroutine(EndGame());
                        }
                    }

                    if (ball.transform.position.y < bottomLeft.y) {
                        //Debug.Log("Player 2 scores");

                        Camera.main.Shake();
                        Instantiate(sSpark, ball.transform.position, Quaternion.Euler(new Vector3(0, 0, 180)));
                        p1.BroadcastMessage("takeDamage");
                        p1hp -= 1;

                        audioSource.PlayOneShot(score);
                        //Debug.Log("P1HP: " + p1hp);
                        if (p1hp > 0) {
                            StartCoroutine(restartTheGame());
                        }
                        else {
                            StartCoroutine(EndGame());
                        }


                    }
                    // ball.SetActive(false);
                }
                break;

            case 3:
                if (Input.GetMouseButtonDown(0)) {
                    SceneManager.LoadScene("MainScene");
                }

                break;

        }

    }

    void getEntitiesReady() {

        p1.BroadcastMessage("replacePaddles");
        p1.BroadcastMessage("startHPDisplay");
        p1.transform.position = new Vector3(0f, -24, 0f);

        p2.BroadcastMessage("replacePaddles");
        p2.BroadcastMessage("startHPDisplay");
        p2.transform.position = new Vector3(0f, 24, 0f);

        //ball.BroadcastMessage("stopRolling");
        ball.transform.position = Vector3.zero;

    }

    IEnumerator startTheGame() {

        getEntitiesReady(); //replace paddles and get the ball back

        //ball.SetActive(true);
        p1.SetActive(true);
        p2.SetActive(true);

        //ball.transform.position = new Vector3(0f, 0f, 0f);
        //ball.BroadcastMessage("stopRolling");


        yield return new WaitForSeconds(2);
        ball.BroadcastMessage("startRolling");

    }

    IEnumerator restartTheGame() {
        //Debug.Log("Restarting the game");
        //ball.transform.position = new Vector3(0f, 0f, 0f);
        ball.BroadcastMessage("stopRolling");
        //ball.SetActive(false); //temporarily hides the ball


        yield return new WaitForSeconds(2);

        destroyAllBullets();
        StartCoroutine(startTheGame());
    }

    IEnumerator EndGame() {

        //ball.SetActive(false);
        destroyAllBullets();
        ball.BroadcastMessage("stopRolling");

        yield return new WaitForSeconds(2);

        audioSource.PlayOneShot(endgame);

        p1.SetActive(false);
        p2.SetActive(false);

        if (p1hp > 0) {
            p1Text.text = "WON";
            p2Text.text = "LOST";
        } else {
            p1Text.text = "LOST";
            p2Text.text = "WON";
        }

        gameState = 3;
    }

    IEnumerator WaitTWoSecsToBeReady() {
        yield return new WaitForSeconds(1);

        readyToStart = true;
    }

    void destroyAllBullets() {
        bullets = GameObject.FindGameObjectsWithTag("bullet");
        foreach (GameObject bullet in bullets) {
            Destroy(bullet);
        }

    }

    void hideTutorialTexts() {
        foreach (Text text in Tutorial_line1) {
            text.color = new Color(0, 0, 0, 0);
        }

        foreach (Text text in Tutorial_line2) {
            text.color = new Color(0, 0, 0, 0);
        }
    }

    void showTutorial() {
        foreach (Text text in Tutorial_line1) {
            text.DOColor(new Color(0.271f, 0.353f, 0.392f, 1), 1).SetAutoKill(true);
        }

        StartCoroutine(showTutorialLines2());
    }

    IEnumerator showTutorialLines2() {

        yield return new WaitForSeconds(2);

        foreach (Text text in Tutorial_line2) {
            text.DOColor(new Color(0.271f, 0.353f, 0.392f, 1), 1).SetAutoKill(true);
        }

        StartCoroutine(hideTutorialsFadeAgain());
    }

    IEnumerator hideTutorialsFadeAgain() {
        yield return new WaitForSeconds(5);

        foreach (Text text in Tutorial_line1) {
            text.DOColor(new Color(0, 0, 0, 0), 1).SetAutoKill(true);
        }

        foreach (Text text in Tutorial_line2) {
            text.DOColor(new Color(0, 0, 0, 0), 1).SetAutoKill(true);
        }
    }

}
