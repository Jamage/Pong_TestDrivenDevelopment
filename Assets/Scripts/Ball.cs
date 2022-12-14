using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Ball : MonoBehaviour
{
    private PhysicsMaterial2D bounce;
    private BoxCollider2D coll;
    private Vector2 speed;

    public enum serviceDirection { left=-1, none=0, right=1 }

    void Awake() {
        coll = gameObject.AddComponent<BoxCollider2D>();
        coll.isTrigger = true;
        Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();
        rb2D.isKinematic = true;
        SpriteRenderer spRenderer = gameObject.AddComponent<SpriteRenderer>();
        spRenderer.sprite = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/whitesquare.png", typeof(Sprite));
        transform.localScale = new Vector2 (0.5f, 0.5f);
        ResetBall();
    }

    public void Move()
    {
        Vector3 delta = speed * Time.deltaTime;
        transform.position += delta * 2;

        if (Mathf.Abs(transform.position.y) > (Camera.main.orthographicSize - (transform.localScale.x/2)))
        {
            Bounce();
        }
    }

    public void Bounce()
    {
        speed *= new Vector2(1,-1);
    }

    public void SetSpeed(float x, float y)
    {
        speed = new Vector2(x,y);
    }
    public Vector2 GetSpeed()
    {
        return speed;
    }

    public void ResetBall(serviceDirection direction = serviceDirection.right) {
        transform.position = new Vector3(0,0,0);
        speed = new Vector2 (Random.Range(0.1f,1f), Random.Range(-1f,1f));
        if ( direction == serviceDirection.left )
        {
            speed *= new Vector2 (-1,0);
        }
    }

    public void Hit(float verticalSpeed = 0f)
    {
        speed *= new Vector2(-1,1);
        speed += new Vector2(0, verticalSpeed);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Paddle")
        {
            Debug.Log("hit");
            Hit(other.gameObject.GetComponent<Paddle>().GetSpeed().y);
        }
        else
        {
            //Debug.Log("wat");
        }
    }

    void Update() {
        Move();
    }
}
