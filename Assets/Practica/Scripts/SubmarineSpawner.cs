using UnityEngine;

public class SubmarineSpawner : MonoBehaviour
{
    public GameObject submarine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            submarine.SetActive(true);
        }
    }
}