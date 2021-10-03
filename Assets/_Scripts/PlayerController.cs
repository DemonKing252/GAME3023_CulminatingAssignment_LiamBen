using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    
    
    private Rigidbody2D rb;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        // Add to Diary: Didn't realize that delta time was already used in velocity 

        var horiz = Input.GetAxisRaw("Horizontal"); 
        var vert = Input.GetAxisRaw("Vertical"); 

        rb.velocity = new Vector2(horiz * speed, vert * speed);

        float lateralSpeed = new Vector2(rb.velocity.x / speed, rb.velocity.y / speed).magnitude;
        anim.SetFloat("_Speed", lateralSpeed);
        if (rb.velocity.x > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (rb.velocity.x < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        
        //Debug.Log("lateral: " + lateralSpeed.ToString());
            
    }
}
