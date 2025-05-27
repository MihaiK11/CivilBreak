using UnityEngine;

public class FactoryClick : MonoBehaviour
{
    public GameObject messageUI;

    void OnMouseDown()
    {
        Debug.Log("Ai dat click pe fabrică!");
        if (messageUI != null)
        {
            messageUI.SetActive(true);
            Invoke("HideMessage", 10f);
        }
    }

    void HideMessage()
    {
        messageUI.SetActive(false);
    }
}
