using UnityEngine;

public class FemWarriorAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    private Animator anim;
    private PlayerMovement PlayerMovement;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        PlayerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && cooldownTimer > attackCooldown)
        {
            if (PlayerMovement.canAttack())
            {
                Attack();
            }
            else
            {
                DashAttack();
            }
        }


        cooldownTimer += Time.deltaTime;
    }

    private void Attack()
    {
        anim.SetTrigger("Attack");
        cooldownTimer = 0;
    }
    private void DashAttack()
    {
        anim.SetTrigger("DashAttack");
        cooldownTimer = 0;
    }
}
