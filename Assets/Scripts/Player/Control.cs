using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control: MonoBehaviour {

    private Vector3 touchOrigin = new Vector3();
    private Vector3 positionOrigin = new Vector3();
    private Vector3 deltaPos = new Vector3();
    private Vector3 t1w = new Vector3(); // converts from screen to world point 

    int id1 = -1; 
    int id2 = -2;

    private bool isBeingControlled = false;
    private bool isChargingToFire = false;

    public float touchSensitivity = 1.2f; //for simplicity's sake, there won't be a setting in this game

    public float verticalControlArea_lowest; // From 0 to 1, to designate the area assigned to this module
    public float verticalControlArea_highest;

    public GameObject bullet;

    void Start() {
    }

    void Update()  {
        for (int i = 0; i < Input.touchCount; i++) {
            Touch t = Input.GetTouch(i);



            if (Screen.height * verticalControlArea_lowest < t.position.y &&
                t.position .y < Screen.height * verticalControlArea_highest) {
                if (!isBeingControlled) {
                    id1 = t.fingerId; // marking the first touch
                    isBeingControlled = true;
                } else if (!isChargingToFire) {
                    id2 = t.fingerId;
                    Debug.Log("Touch2 detected");
                }
                
                
            }

            // Handling the first touch
            if (t.fingerId == id1) {

                t1w = Camera.main.ScreenToWorldPoint(t.position);

                switch (t.phase) {
                    case TouchPhase.Began:
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

                        bullet = Instantiate(bullet, transform.TransformPoint(Vector3.up * 1.2f), transform.rotation);
                        bullet.transform.parent = transform;
                        //bullet.transform.arent(transform, false);
                        bullet.transform.localPosition = Vector3.zero;
                        //bullet.transform.localPosition = Vector3.zero;
                        //ProjectileBehaviour bBh = bullet.GetComponent<ProjectileBehaviour>();
                        //Vector3 bBh_vel = bBh.velocity;



                        //bullet.transform.localPosition = new Vector3(10, 10, 0);
                        // bullet.transform.localPosition = new Vector3(bullet.transform.localPosition.x,
                        //Mathf.SmoothStep(bullet.transform.localPosition.y, bullet.transform.localPosition.y * 2f, 1f),
                        //bullet.transform.localPosition.z);
                        //bullet

                        // bullet.transform.localPosition = Vector3.Lerp(bullet.transform.position, transform.TransformPoint(Vector3.up *1.3f), Time.deltaTime * 2f);



                        break;

                    case TouchPhase.Ended:
                        isChargingToFire = false;
                        // fire();
                        id2 = -2;
                        break;
                }

                //if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved) {
                //    Vector3 vel = Vector3.zero;
                //    bullet.transform.localPosition = Vector3.Lerp(bullet.transform.position, bullet.transform.TransformPoint(Vector3.up * 2f), Time.deltaTime * 2f);
                //}
            }

        }
    }
}
