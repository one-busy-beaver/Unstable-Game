using UnityEngine;

public class Boss : Enemy
{
    [Header("Final Messages")]
    [SerializeField] private string savedMessage = "You saved the tall man.";
    [SerializeField] private string destroyedMessage = "You destroyed the tall man.";

    private InteractHandler handler;
    private bool messageDisplayed = false;

    private void Start()
    {
        // Get the handler that manages the floating text
        handler = GetComponent<InteractHandler>();
    }

    void Update()
    {
        // RUN THIS FIRST: This tells the Boss to execute Enemy.Update(), 
        // which calls HandleDeath()
        base.Update(); 

        // Check if the base Enemy script has marked this as dead
        if (isDead && !messageDisplayed)
        {
            DisplayFinalMessage();
        }
    }

    private void DisplayFinalMessage()
    {
        if (handler == null) return;

        // 1. Determine the message based on Courage
        string finalMessage = (PlayerInventory.Instance != null && PlayerInventory.Instance.HasCourage) 
            ? savedMessage 
            : destroyedMessage;

        // 2. Set the text and force the UI to show up
        handler.SetPromptText(finalMessage);
        handler.TogglePrompt(true);

        messageDisplayed = true;
    }
}