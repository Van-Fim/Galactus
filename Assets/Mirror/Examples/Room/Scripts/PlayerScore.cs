using UnityEngine;

namespace Mirror.Examples.NetworkRoom
{
<<<<<<< HEAD
    [AddComponentMenu("")]
=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    public class PlayerScore : NetworkBehaviour
    {
        [SyncVar]
        public int index;

        [SyncVar]
        public uint score;

        void OnGUI()
        {
            GUI.Box(new Rect(10f + (index * 110), 10f, 100f, 25f), $"P{index}: {score:0000000}");
        }
    }
}
