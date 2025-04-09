using UnityEngine;

public class CarTrigger : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Проверяем, зажата ли клавиша Ctrl (Left или Right)
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
        {
            // Запускаем мини-игру через GameManager
            if (GameManager.Instance != null)
            {
                InstructionUI.Instance.Hide(); // скрываем инструкцию
                GameManager.Instance.StartFlyerTypingGame();
            }
        }
    }
}