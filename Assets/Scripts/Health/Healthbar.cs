using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalhealthBar;
    [SerializeField] private Image currenthealthBar;

    private void Start()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / 6;
    }

    private void Update()
    {
        currenthealthBar.fillAmount = playerHealth.currentHealth / 6;
    }
}
