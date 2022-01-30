using UnityEngine;
public class Goal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameHandler gameHandler = ServiceLocator.GetGameHandler();
        gameHandler.LoadNextMap();
    }

    [SerializeField] private bool isLastLevel;
}
