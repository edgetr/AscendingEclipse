using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestBoss : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private float attackCoolDown;

    [SerializeField] private float range;
    [SerializeField] private int damage;

    [SerializeField] private LayerMask playerLayer;

    private float cooldownTimer = Mathf.Infinity;

    [SerializeField] private BoxCollider2D boxCollider;


    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        //Attack only when player is in sight

        if (PlayerInSight())
        {

            if (cooldownTimer >= attackCoolDown)
            {
                //Attack

            }
        }

    }

    private bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x , boxCollider.bounds.size, 0, Vector2.left, 0, playerLayer);

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x , boxCollider.bounds.size);
    }


}
