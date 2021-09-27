using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    
    
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        // Add to Diary: Didn't realize that delta time was already used in velocity 

        var horiz = Input.GetAxisRaw("Horizontal"); 
        var vert = Input.GetAxisRaw("Vertical"); 

        rb.velocity = new Vector2(horiz * speed, vert * speed);
    }
}
