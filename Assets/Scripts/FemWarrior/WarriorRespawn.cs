using UnityEngine;

public class WarriorRespawn : MonoBehaviour
{
    private Transform currentCheckpoint;
    private Health playerHealth;


    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }


    public void Respawn()
    {
        transform.position = currentCheckpoint.position;
        playerHealth.Respawn();
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Portal")
        {
            currentCheckpoint = collision.transform;
            collision.GetComponent<Collider2D>().enabled = false;
            collision.GetComponent<Animator>().SetTrigger("Appear");
        }
    }
}
