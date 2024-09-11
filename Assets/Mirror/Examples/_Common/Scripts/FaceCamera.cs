// Useful for Text Meshes that should face the camera.
using UnityEngine;

namespace Mirror.Examples.Common
{
<<<<<<< HEAD
    [AddComponentMenu("")]
=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    public class FaceCamera : MonoBehaviour
    {
        // LateUpdate so that all camera updates are finished.
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
