using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Control: MonoBehaviour {

    private Vector3 touchOrigin = new Vector3();
    private Vector3 positionOrigin = new Vector3();
    private Vector3 deltaPos = new Vector3();
    private Vector3 t1w = new Vector3(); // converts from screen to world point 

    int id1 = -1; 
    int id2 = -2;

    private bool isBeingControlled = false;

    public float touchSensitivity = 1.2f; //for simplicity's sake, there won't be a setting in this game

    public float verticalControlArea_lowest; // From 0 to 1, to designate the area assigned to this module
    public float verticalControlArea_highest;

    private GameObject bullet;
    public GameObject bulletSource;
    private bool isChargingToFire = false;
    private float chargingDuration = 0f;
    private float minimumCharging = 0.5f;
    private float maximumCharging = 3.0f;
    private float multiplier = 30f;

    private bool isFullyCharged = false;
    public ParticleSystem chargedSparkSource;
    private ParticleSystem chargedSpark;

    private Tween bulletPosTween;

    public Text debugtext;

    public AudioSource audioSource;
    public AudioClip gunshot;
    public AudioClip loadedSound;

    void Update()  {
        for (int i = 0; i < Input.touchCount; i++) {
            Touch t = Input.GetTouch(i);



            if (Screen.height * verticalControlArea_lowest < t.position.y &&
                t.position .y < Screen.height * verticalControlArea_highest) {
                if (!isBeingControlled) {
                    id1 = t.fingerId; // marking the first touch
                }
                else if (!isChargingToFire && t.fingerId != id1) {
                    id2 = t.fingerId;
                    Debug.Log("Touch2 detected");
                }

                // Alternative block of code that also works, for reference purposes
                if (id1 == -1) {
                    id1 = t.fingerId;
                } else if (id2 == -2 && t.fingerId !=id1) {
                    id2 = t.fingerId;
                }

            }

            // Handling the first touch
            if (t.fingerId == id1) {

                t1w = Camera.main.ScreenToWorldPoint(t.position);

                switch (t.phase) {

                    case TouchPhase.Began:
                        isBeingControlled = true;
                        positionOrigin = transform.position;
                        touchOrigin = t1w;
                        break;

                    case TouchPhase.Moved:

                        // Calculating the relative movement
                        deltaPos = t1w - touchOrigin;
                        float newX = positionOrigin.x + deltaPos.x * touchSensitivity;

                        // Keeping the object inside the screen
                        Vector3 leftMost = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
                        Vector3 rightMost = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0));
                        newX = Mathf.Clamp(newX, leftMost.x + 3.5f, rightMost.x - 3.5f); //  -+ paddle_width/2
                        
                        // Final movement
                        transform.position = new Vector3(newX,transform.position.y,transform.position.z);

                        break;

                    case TouchPhase.Ended:
                        isBeingControlled = false;
                        id1 = -1;
                        break;
                }
                
            }

            //Second touch
            if (t.fingerId == id2) {
                switch(t.phase) {
                    case TouchPhase.Began:
                    
                        isChargingToFire = true;
                        startCharging();
                        break;

                    case TouchPhase.Moved:

                        charging();
                        break;

                    case TouchPhase.Stationary:

                        charging();
                        break;

                    case TouchPhase.Ended:

                        if (chargingDuration >= minimumCharging) {
                            fire();
                        }
                        else {
                            cancelCharging();
                        }

                        isChargingToFire = false;
                        isFullyCharged = false;
                        chargingDuration = 0;
                        id2 = -2;
                        break;
                }
            }

        }

        //debugtext.text = ("isBeingControl: " + isBeingControlled + " isChargingToFire: " + isChargingToFire);
        //debugtext.text = ("id1 " + id1 + " id2 " + id2);
    }

    void startCharging() {
        bullet = Instantiate(bulletSource, transform.position, transform.rotation);
        bullet.transform.parent = transform;
        bullet.transform.localScale = Vector3.zero;
        bullet.transform.localPosition = Vector3.zero;

        bulletPosTween = bullet.transform.DOLocalMove(new Vector3(0, 1f, 0), 1);
        bullet.transform.DOScale(new Vector3(1f, 1f, 1f), 1);
    }

    void charging() {
        if (chargingDuration < maximumCharging) {
            chargingDuration += Time.deltaTime;
        } else if (!isFullyCharged) {
            Debug.Log("Fully charged!");

            chargedSpark = Instantiate(chargedSparkSource, transform.position, transform.rotation);
            chargedSpark.transform.parent = transform;
            chargedSpark.transform.localPosition = new Vector3(0, 1f, 0);


            isFullyCharged = true;
            audioSource.PlayOneShot(loadedSound);
        }
    }

    void cancelCharging() {
        bullet.transform.DOLocalMove(new Vector3(0, 0, 0), 1);
        bullet.transform.DOScale(new Vector3(0, 0, 0), 1);
        Destroy(bullet, 1f);
    }

    void fire() {
        bulletPosTween.Pause();
        //Vector3 tempPosition = bullet.transform.position; 
        bullet.transform.SetParent(null, true);
        //bullet.transform.position = tempPosition;
        bullet.BroadcastMessage("move", chargingDuration*30f);

        audioSource.PlayOneShot(gunshot);
    }
}
