using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour {

    public GameObject p1;
    public GameObject p2;
    public GameObject ball;

    private PlayerBehaviour pBh;

    public int p1hp;
    public int p2hp;

    public Text p1Text;
    public Text p2Text;

    public Text[] MenuTexts;

    private int gameState = 1; //1 == menu; 2 == play; 3 == end;
    private bool readyToStart = false; //give the game 2 seconds buffer at main menu to avoid accidental press

    void Start() {
        ball.SetActive(true);
        ball.BroadcastMessage("startRolling");

        StartCoroutine(WaitTWoSecsToBeReady());
    }

    void getEntitiesReady() {

        p1.BroadcastMessage("replacePaddles");
        p1.BroadcastMessage("startHPDisplay");
        p1.transform.position = new Vector3(0f, -24, 0f);

        p2.BroadcastMessage("replacePaddles");
        p2.BroadcastMessage("startHPDisplay");
        p2.transform.position = new Vector3(0f, 24, 0f);

        ball.BroadcastMessage("stopRolling");
        ball.transform.position = Vector3.zero;
    
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

                    getEntitiesReady();
                    StartCoroutine(startTheGame());
                    ball.transform.position = new Vector3(0f, 0f, 0f);
                    ball.BroadcastMessage("stopBouncingVertically");
                    ball.BroadcastMessage("stopRolling");
                }
            break;

            case 2:
                if (ball.activeSelf) {

                    if (ball.transform.position.y > topRight.y) {
                        Debug.Log("Player 1 scores");
                        //Instatiate flash 
                        p2.BroadcastMessage("takeDamage");
                        p2hp -= 1;
                        Debug.Log("P2HP: " + p2hp);
                        if (p2hp > 0) {
                            StartCoroutine(restartTheGame());
                        }
                        else {
                            StartCoroutine(EndGame());
                            ball.SetActive(false);
                        }
                    }

                    if (ball.transform.position.y < bottomLeft.y) {
                        Debug.Log("Player 2 scores");
                        // Instantiate flash
                        p1.BroadcastMessage("takeDamage");
                        p1hp -= 1;
                        Debug.Log("P1HP: " + p1hp);
                        if (p1hp > 0) {
                            StartCoroutine(restartTheGame());
                        }
                        else {
                            StartCoroutine(EndGame());
                            ball.SetActive(false);
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

    IEnumerator startTheGame() {
        ball.SetActive(true);
        p1.SetActive(true);
        p2.SetActive(true);
        yield return new WaitForSeconds(2);
        ball.BroadcastMessage("startRolling");

    }

    IEnumerator restartTheGame() {
        Debug.Log("Restarting the game");
        ball.transform.position = new Vector3(0f, 0f, 0f);
        ball.BroadcastMessage("stopRolling");
        ball.SetActive(false); //temporarily hides the ball

        yield return new WaitForSeconds(2);

        StartCoroutine(startTheGame());
    }

    IEnumerator EndGame() {

        yield return new WaitForSeconds(2);

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
        yield return new WaitForSeconds(2);

        readyToStart = true;
    }

}
