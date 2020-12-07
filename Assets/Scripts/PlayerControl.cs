using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interaction.PlayerInteractEvent?.Invoke();
        }
    }
}
