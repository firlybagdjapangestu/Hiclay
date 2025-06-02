using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    [Header("UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private ZoneManager zoneManager;
    [SerializeField] private ZoneManager zoneManager2;

    private int currentStep = 0;
    private bool waitingForEnemiesClear = false;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Look.performed += OnLookPerformed;
        inputActions.Player.Interact.performed += OnInteractPerformed;

        ShowCurrentInstruction();
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Look.performed -= OnLookPerformed;
        inputActions.Player.Interact.performed -= OnInteractPerformed;

        inputActions.Player.Disable();
    }

    private void Update()
    {
        // Cek step 3: basmi musuh pertama
        if (currentStep == 3 && waitingForEnemiesClear && zoneManager.zoneClear)
        {
            waitingForEnemiesClear = false;
            AdvanceTutorial();
        }
        // Cek step 5: basmi musuh kedua
        else if (currentStep == 5 && waitingForEnemiesClear && zoneManager2.zoneClear)
        {
            waitingForEnemiesClear = false;
            AdvanceTutorial();
        }
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        if (currentStep == 0 && context.ReadValue<Vector2>().magnitude > 0.1f)
        {
            AdvanceTutorial();
        }
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        Vector2 lookDir = context.ReadValue<Vector2>();

        if (currentStep == 1 && lookDir.magnitude > 0.1f)
        {
            AdvanceTutorial();
        }
        else if (currentStep == 2 && lookDir.y < -0.7f)
        {
            AdvanceTutorial();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (currentStep == 4)
        {
            AdvanceTutorial();
        }
    }

    private void AdvanceTutorial()
    {
        currentStep++;

        if (currentStep > 6)
        {
            EndTutorial();
        }
        else
        {
            ShowCurrentInstruction();
        }
    }

    private void ShowCurrentInstruction()
    {
        tutorialPanel.SetActive(true);

        switch (currentStep)
        {
            case 0:
                instructionText.text = "Gerakkan joystick kiri untuk berjalan.";
                break;
            case 1:
                instructionText.text = "Arahkan joystick kanan untuk menembak.";
                break;
            case 2:
                instructionText.text = "Arahkan joystick kanan ke bawah untuk melompat.";
                break;
            case 3:
                instructionText.text = "Basmi semua musuh yang tersisa!";
                waitingForEnemiesClear = true;
                break;
            case 4:
                instructionText.text = "Dekati Mecha lalu tekan tombol untuk masuk.";
                break;
            case 5:
                instructionText.text = "Basmi musuh yang muncul setelah masuk Mecha!";
                waitingForEnemiesClear = true;
                break;
            case 6:
                instructionText.text = "Tutorial selesai. Kamu jago!";
                break;
        }
    }

    private void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial selesai. Game tamat.");
    }
}
