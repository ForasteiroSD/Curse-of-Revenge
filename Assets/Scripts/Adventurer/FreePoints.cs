using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FreePoints : MonoBehaviour {
// Define the key sequence
    private List<Key> targetSequence = new List<Key> { Key.F, Key.R, Key.E, Key.E, Key.P, Key.O, Key.I, Key.N, Key.T, Key.S };
    private List<Key> currentSequence = new List<Key>();

    // Reference to the Input Action
    public InputActionAsset inputActions;

    private void OnEnable()
    {
        // Enable the action map that contains the keys we care about
        inputActions.Enable();

        // Register the key press event for the specific keys
        // inputActions.FindActionMap("Gameplay").FindAction("Move").performed += OnFreePoints;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    // This method is called when a key is pressed
    private void OnFreePoints(InputAction.CallbackContext context)
    {
        Key pressedKey = context.ReadValue<Key>(); // Get the key that was pressed

        // Add the pressed key to the current sequence
        currentSequence.Add(pressedKey);

        // Check if the sequence matches the target sequence
        CheckSequence();
    }

    private void CheckSequence()
    {
        // If the current sequence length exceeds the target sequence, remove the first element
        if (currentSequence.Count > targetSequence.Count)
        {
            currentSequence.RemoveAt(0);
        }

        // If the current sequence matches the target sequence, trigger the function
        if (IsSequenceCorrect())
        {
            Debug.Log("Sequence completed!");
            CallFunction();
            currentSequence.Clear(); // Clear the sequence after successful match
        }
    }

    // Method to check if the sequence is correct
    private bool IsSequenceCorrect()
    {
        if (currentSequence.Count != targetSequence.Count)
            return false;

        for (int i = 0; i < targetSequence.Count; i++)
        {
            if (currentSequence[i] != targetSequence[i])
            {
                return false;
            }
        }

        return true;
    }

    // Function to call when sequence is completed
    private void CallFunction()
    {
        // Put your custom logic here
        Debug.Log("Calling function after sequence completion!");
    }
}
