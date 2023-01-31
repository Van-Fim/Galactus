using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ClientPanel : MonoBehaviour
{
    public virtual void Init(){}
    public virtual void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
