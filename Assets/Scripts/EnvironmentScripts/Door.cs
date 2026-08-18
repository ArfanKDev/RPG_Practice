using UnityEngine;

public class Door : MonoBehaviour
{
      public Animator animator;
     
     
     
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
     
        private void OnTriggerEnter2D(Collider2D collision)
        {
            Debug.Log("Player entered the door trigger." + collision.gameObject.name);
            if (!collision.CompareTag("Player")) return;
     
           
            animator.ResetTrigger("Close");
            animator.SetTrigger("Open");
        }
     
        private void OnTriggerExit2D(Collider2D collision)
        {
           
            if (!collision.CompareTag("Player")) return;
     
            animator.ResetTrigger("Open");
            animator.SetTrigger("Close");
        }
}
