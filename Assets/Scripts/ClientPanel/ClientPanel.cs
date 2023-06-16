using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClientPanel : MonoBehaviour
{
    public virtual void Init() { }

    public virtual void UpdateText(){}
    public virtual void SetIndex(int index)
    {
        transform.SetSiblingIndex(index);
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
        if (ClientPanelManager.historyPanels.Contains(this))
        {
            ClientPanelManager.historyPanels.Remove(this);
        }
    }
    public virtual void Back()
    {
        int ind = ClientPanelManager.historyPanels.IndexOf(this) - 1;
        if (ind > 0)
        {
            ClientPanel p = ClientPanelManager.historyPanels[ind];
            ClientPanelManager.Show(p);
        }
    }
}
