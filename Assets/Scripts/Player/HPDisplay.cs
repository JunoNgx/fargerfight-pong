using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPDisplay: MonoBehaviour {

    public Image[] healthboxes;

    PlayerBehaviour pBh;
    int hpToDisplay;
    bool isDisplaying = false;

    void Start() {
        foreach (Image healthbox in healthboxes) {
            healthbox.enabled = false;
        }
    }

    void Update() {

        if (isDisplaying) {
            pBh = GetComponent<PlayerBehaviour>();
            hpToDisplay = pBh.hp;

            for (int i = 0; i < healthboxes.Length; i++) {
                if (i < hpToDisplay) {
                    healthboxes[i].enabled = true;
                } else {
                    healthboxes[i].enabled = false;
                }
            }
        }     
    }

    void startHPDisplay() {
        isDisplaying = true;
    }
}
