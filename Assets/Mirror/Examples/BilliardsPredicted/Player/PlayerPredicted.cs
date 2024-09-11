using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror.Examples.BilliardsPredicted
{
    // example input for this exact demo.
    // not general purpose yet.
    public struct PlayerInput
    {
        public double timestamp;
        public Vector3 force;

        public PlayerInput(double timestamp, Vector3 force)
        {
            this.timestamp = timestamp;
            this.force = force;
        }
    }

    public class PlayerPredicted : NetworkBehaviour
    {
        // white ball component
        WhiteBallPredicted whiteBall;

<<<<<<< HEAD
=======
        // keep a history of inputs with timestamp
        public int inputHistorySize = 64;
        readonly SortedList<double, PlayerInput> inputs = new SortedList<double, PlayerInput>();

>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        void Awake()
        {
            // find the white ball once
#if UNITY_2022_2_OR_NEWER
            whiteBall = FindAnyObjectByType<WhiteBallPredicted>();
#else
            // Deprecated in Unity 2023.1
            whiteBall = FindObjectOfType<WhiteBallPredicted>();
#endif
        }

        // apply force to white ball.
        // common function to ensure we apply it the same way on server & client!
        void ApplyForceToWhite(Vector3 force)
        {
            // https://docs.unity3d.com/2021.3/Documentation/ScriptReference/Rigidbody.AddForce.html
            // this is buffered until the next FixedUpdate.

<<<<<<< HEAD
            // get the white ball's Rigidbody.
            // prediction sometimes moves this out of the object for a while,
            // so we need to grab it this way:
            Rigidbody rb = whiteBall.GetComponent<PredictedRigidbody>().predictedRigidbody;

=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
            // AddForce has different force modes, see this excellent diagram:
            // https://www.reddit.com/r/Unity3D/comments/psukm1/know_the_difference_between_forcemodes_a_little/
            // for prediction it's extremely important(!) to apply the correct mode:
            //   'Force' makes server & client drift significantly here
            //   'Impulse' is correct usage with significantly less drift
<<<<<<< HEAD
            rb.AddForce(force, ForceMode.Impulse);
=======
            whiteBall.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        }

        // called when the local player dragged the white ball.
        // we reuse the white ball's OnMouseDrag and forward the event to here.
        public void OnDraggedBall(Vector3 force)
        {
<<<<<<< HEAD
=======
            // record the input for reconciliation if needed
            if (inputs.Count >= inputHistorySize) inputs.RemoveAt(0);
            inputs.Add(NetworkTime.time, new PlayerInput(NetworkTime.time, force));
            Debug.Log($"Inputs.Count={inputs.Count}");

>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
            // apply force locally immediately
            ApplyForceToWhite(force);

            // apply on server as well.
            // not necessary in host mode, otherwise we would apply it twice.
<<<<<<< HEAD
            if (!isServer) CmdApplyForce(force);
=======
            if (!isServer) CmdApplyForce(force, NetworkTime.predictedTime);
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        }

        // while prediction is applied on clients immediately,
        // we still want to validate every input on the server and reject it if necessary.
        // this way we can latency free yet cheat safe movement.
        // this should include a certain tolerance so players aren't hard corrected
        // for their local movement all the time.
        // TODO this should be on some kind of base class for reuse, but perhaps independent of parameters?
        bool IsValidMove(Vector3 force) => true;

        // TODO send over unreliable with ack, notify, etc. later
        [Command]
<<<<<<< HEAD
        void CmdApplyForce(Vector3 force)
=======
        void CmdApplyForce(Vector3 force, double predictedTime)
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
        {
            if (!IsValidMove(force))
            {
                Debug.Log($"Server rejected move: {force}");
                return;
            }

<<<<<<< HEAD
=======
            // client is on a predicted timeline.
            // double check the prediction - it should arrive at server time.
            //
            // there are multiple reasons why this may be off:
            // - time prediction may still be adjusting itself
            // - time prediction may have an issue
            // - server or client may be lagging or under heavy load temporarily
            // - unreliable vs. reliable channel latencies are signifcantly different
            //   for example, if latency simulation is only applied to one channel!
            double delta = NetworkTime.time - predictedTime;
            if (delta < -0.010)
            {
                Debug.LogWarning($"Cmd predictedTime was {(delta*1000):F0}ms behind the server time. This could occasionally happen if the time prediction is off. If it happens consistently, check that unreliable NetworkTime and reliable [Command]s have the same latency. If they are off, this will cause heavy jitter.");
            }
            else if (delta > 0.010)
            {
                // TODO consider buffering inputs which are ahead, apply next frame
                Debug.LogWarning($"Cmd predictedTime was {(delta*1000):F0}ms ahead of the server time. This could occasionally happen if the time prediction is off. If it happens consistently, check that unreliable NetworkTime and reliable [Command]s have the same latency. If they are off, this will cause heavy jitter. If reliable & unreliable latency are similar and this still happens a lot, consider buffering inputs for the next frame.");
            }
            else
            {
                Debug.Log($"Cmd predictedTime was {(delta*1000):F0}ms close to the server time.");
            }

>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
            // apply force
            ApplyForceToWhite(force);
        }
    }
}
