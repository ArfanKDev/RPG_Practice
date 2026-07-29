using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
  public float speed = 5f;
  public int facingDirection = 1;
  public Rigidbody2D rb;

  public Animator anim;

  private void Start()
   {
     rb = GetComponent<Rigidbody2D>();
   }

   private void FixedUpdate()
   {
      rb.linearVelocity = Vector2.zero; 
      float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        if (moveX > 0 && transform.localScale.x < 0 || moveX < 0 && transform.localScale.x > 0)
       {
        Flip();
       }

         anim.SetFloat("Horizontal", Mathf.Abs(moveX));
        anim.SetFloat("Vertical",Mathf.Abs(moveY));
          rb.MovePosition(rb.position + new Vector2(moveX, moveY) * speed * Time.deltaTime);
   }

   
    void Flip()
  {
    facingDirection *= -1;
    transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
  }
}