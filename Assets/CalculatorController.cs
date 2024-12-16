using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections; // For Dictionary

using TMPro;
public class CalculatorController : MonoBehaviour
{
    public TextMeshProUGUI displayText; // Link this to a UI Text object to display results

    private string currentInput = "";
    private string previousInput = "";
    private string operation = "";
    private bool resetInput = false;

    private float memory = 0f;
    private float grandTotal = 0f;

    public float pressDepth = 0.02f; // How far buttons move down
    public float pressSpeed = 10f;   // Speed of the animation

    public float holdTime=0.01f;

    private Coroutine currentAnimation; // Tracks the current button animation
    private Dictionary<Transform, Vector3> buttonOriginalLocalPositions = new Dictionary<Transform, Vector3>();

    void Start()
    {
        // Cache all buttons' original LOCAL positions at the start
        CacheButtonLocalPositions();
    }

    void Update()
    {
        // Detect mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the clicked object is a valid button
                if (IsValidButton(hit.transform.name))
                {
                    // Interrupt any ongoing button animation
                    if (currentAnimation != null)
                    {
                        StopCoroutine(currentAnimation);
                        ResetButtonLocalPosition(hit.transform);
                    }

                    // Start the new button press animation
                    currentAnimation = StartCoroutine(AnimateButtonPress(hit.transform));
                    HandleButtonClick(hit.transform.name);
                }
            }
        }
    }

    // Cache original LOCAL positions of all buttons
    void CacheButtonLocalPositions()
    {
        foreach (Transform child in transform)
        {
            if (IsValidButton(child.name) && !buttonOriginalLocalPositions.ContainsKey(child))
            {
                buttonOriginalLocalPositions.Add(child, child.localPosition);
            }
        }
    }

    // Check if the clicked object is a valid button
    bool IsValidButton(string buttonName)
    {
        string[] validButtons = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
                                  "plus", "minus", "times", "divide", "equals",
                                  "c", "ac", "dot", "mplus", "mminus", "mrc", "gt", "plusminus" };

        foreach (string valid in validButtons)
        {
            if (buttonName == valid)
                return true;
        }
        return false;
    }

    // Coroutine to animate button press
    IEnumerator AnimateButtonPress(Transform buttonTransform)
    {
        Vector3 originalLocalPosition = buttonOriginalLocalPositions[buttonTransform];
        Vector3 pressedLocalPosition = originalLocalPosition - buttonTransform.up * pressDepth;

        // Move the button down
        while (Vector3.Distance(buttonTransform.localPosition, pressedLocalPosition) > 0.001f)
        {
            buttonTransform.localPosition = Vector3.Lerp(buttonTransform.localPosition, pressedLocalPosition, pressSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(holdTime); // Hold the button briefly

        // Move the button back up
        while (Vector3.Distance(buttonTransform.localPosition, originalLocalPosition) > 0.001f)
        {
            buttonTransform.localPosition = Vector3.Lerp(buttonTransform.localPosition, originalLocalPosition, pressSpeed * Time.deltaTime);
            yield return null;
        }
         buttonTransform.localPosition=originalLocalPosition;

        currentAnimation = null; // Clear current animation reference
    }

    // Reset the button's LOCAL position immediately if interrupted
    void ResetButtonLocalPosition(Transform buttonTransform)
    {
        if (buttonOriginalLocalPositions.ContainsKey(buttonTransform))
        {
            buttonTransform.localPosition = buttonOriginalLocalPositions[buttonTransform];
        }
    }

    void HandleButtonClick(string buttonName)
    {
        // Handle numeric buttons (0-9)
        if (buttonName == "0" || buttonName == "1" || buttonName == "2" ||
            buttonName == "3" || buttonName == "4" || buttonName == "5" ||
            buttonName == "6" || buttonName == "7" || buttonName == "8" ||
            buttonName == "9")
        {
            if (resetInput) { currentInput = ""; resetInput = false; }
            currentInput += buttonName;
            UpdateDisplay();
        }

        // Handle operations
        else if (buttonName == "plus" || buttonName == "minus" ||
                 buttonName == "times" || buttonName == "divide")
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                if (!string.IsNullOrEmpty(previousInput)) CalculateResult();
                operation = buttonName;
                previousInput = currentInput;
                resetInput = true;
            }
        }
        else if (buttonName == "equals")
        {
            CalculateResult();
            operation = "";
        }
        else if (buttonName == "c")
        {
            currentInput = "";
            UpdateDisplay();
        }
        else if (buttonName == "ac")
        {
            currentInput = "";
            previousInput = "";
            operation = "";
            memory = 0;
            grandTotal = 0;
            UpdateDisplay();
        }
        else if (buttonName == "dot")
        {
            if (!currentInput.Contains(".")) currentInput += ".";
            UpdateDisplay();
        }

        // Memory buttons
        if (buttonName == "mplus")
        {
            memory += float.Parse(currentInput);
        }
        else if (buttonName == "mminus")
        {
            memory -= float.Parse(currentInput);
        }
        else if (buttonName == "mrc")
        {
            if (resetInput)
            {
                memory = 0;
                resetInput = false;
            }
            else
            {
                currentInput = memory.ToString();
                UpdateDisplay();
                resetInput = true;
            }
        }
        else if (buttonName == "gt")
        {
            grandTotal += float.Parse(currentInput);
            currentInput = grandTotal.ToString();
            UpdateDisplay();
        }
        else if (buttonName == "plusminus")
        {
            if (!string.IsNullOrEmpty(currentInput))
            {
                currentInput = (-float.Parse(currentInput)).ToString();
                UpdateDisplay();
            }
        }
    }

    void CalculateResult()
    {
        if (string.IsNullOrEmpty(previousInput) || string.IsNullOrEmpty(currentInput)) return;

        float num1 = float.Parse(previousInput);
        float num2 = float.Parse(currentInput);
        float result = 0;

        switch (operation)
        {
            case "plus": result = num1 + num2; break;
            case "minus": result = num1 - num2; break;
            case "times": result = num1 * num2; break;
            case "divide": result = num1 / num2; break;
        }

        currentInput = result.ToString();
        previousInput = "";
        resetInput = true;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        displayText.text = string.IsNullOrEmpty(currentInput) ? "0" : currentInput;
    }
}
