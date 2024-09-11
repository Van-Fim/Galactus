using UnityEngine;

namespace Mirror
{
<<<<<<< HEAD
    [AddComponentMenu("Network/Network Diagnostics Debugger")]
=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    public class NetworkDiagnosticsDebugger : MonoBehaviour
    {
        public bool logInMessages = true;
        public bool logOutMessages = true;
        void OnInMessage(NetworkDiagnostics.MessageInfo msgInfo)
        {
            if (logInMessages)
                Debug.Log(msgInfo);
        }
        void OnOutMessage(NetworkDiagnostics.MessageInfo msgInfo)
        {
            if (logOutMessages)
                Debug.Log(msgInfo);
        }
        void OnEnable()
        {
            NetworkDiagnostics.InMessageEvent += OnInMessage;
            NetworkDiagnostics.OutMessageEvent += OnOutMessage;
        }
        void OnDisable()
        {
            NetworkDiagnostics.InMessageEvent -= OnInMessage;
            NetworkDiagnostics.OutMessageEvent -= OnOutMessage;
        }
    }
}
