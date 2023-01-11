using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPanelManager : MonoBehaviour
{
    ClientPanel currentPanel;
    public void Show(ClientPanel clientPanel)
    {
        if (currentPanel != null)
        {
            currentPanel.Hide();
        }
        clientPanel.Show();
    }
}
