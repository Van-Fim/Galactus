using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientInputManager : MonoBehaviour
{
    public static ClientInputManager singleton;
    public static GameInput gameInput;

    float speed;
    public static void Init()
    {
        singleton = Instantiate<ClientInputManager>(GamePrefabsManager.singleton.LoadPrefab<ClientInputManager>());
        singleton.gameObject.name = "ClientInputManager";
        gameInput = new GameInput();
        gameInput.Enable();
        DontDestroyOnLoad(singleton.gameObject);
        singleton.GlobalInputEnable();
    }
    public void GlobalInputEnable()
    {
        gameInput.Global.Toggleconsole.performed += OnConsoleToggle;
        gameInput.Global.Prevconsolecommand.performed += OnConsolePrevCommand;
        gameInput.Global.Nextconsolecommand.performed += OnConsoleNextCommand;
        gameInput.Ship.ChangeSpeed.performed += OnChangeSpeed;
    }
    public void GlobalInputDisable()
    {
        gameInput.Global.Toggleconsole.performed -= OnConsoleToggle;
        gameInput.Global.Prevconsolecommand.performed -= OnConsolePrevCommand;
        gameInput.Global.Nextconsolecommand.performed -= OnConsoleNextCommand;
        gameInput.Ship.ChangeSpeed.performed -= OnChangeSpeed;
    }
    private void OnChangeSpeed(InputAction.CallbackContext context)
    {
        //DebugConsole.Log($"Changing speed {context.ReadValue<float>()}");
    }
    private void OnConsoleToggle(InputAction.CallbackContext context)
    {
        ClientPanelManager.Show<DebugClientPanel>();
    }
    private void OnConsolePrevCommand(InputAction.CallbackContext context)
    {
        DebugConsole.PrevCommand();
    }
    private void OnConsoleNextCommand(InputAction.CallbackContext context)
    {
        DebugConsole.NextCommand();
    }
}
