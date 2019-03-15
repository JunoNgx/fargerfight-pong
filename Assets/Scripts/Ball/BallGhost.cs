using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallGhost : MonoBehaviour {

    SpriteRenderer sprite;

    public float fadingSpeed = 1.5f;
    
    void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        float newAlpha = sprite.color.a - fadingSpeed * Time.deltaTime;

        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, newAlpha);

        if (sprite.color.a <= 0) Destroy(gameObject);
    }
}
