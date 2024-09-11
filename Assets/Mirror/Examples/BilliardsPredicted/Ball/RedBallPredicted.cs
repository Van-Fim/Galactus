<<<<<<< HEAD

=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
using UnityEngine;

namespace Mirror.Examples.BilliardsPredicted
{
<<<<<<< HEAD
    // keep the empty script so we can find out what type of ball we collided with.
    public class RedBallPredicted : NetworkBehaviour
    {
        /* ball<->pocket collisions are handled by Pockets.cs for now.
           because predicted object's rigidbodies are sometimes moved out of them.
           which means this script here wouldn't get the collision info while predicting.
           which means it's easier to check collisions from the table perspective.
=======
    public class RedBallPredicted : NetworkBehaviour
    {
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        // destroy when entering a pocket.
        // there's only one trigger in the scene (the pocket).
        [ServerCallback]
        void OnTriggerEnter(Collider other)
        {
            NetworkServer.Destroy(gameObject);
        }
<<<<<<< HEAD
        */
=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
    }
}
