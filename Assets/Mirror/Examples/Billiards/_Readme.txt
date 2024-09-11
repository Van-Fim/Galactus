Very simple multiplayer Billiards demo.
Mouse drag the white ball to apply force.

Billiards is surprisingly easy to implement, which makes this a great demo for beginners!

Hits are sent to the server with a [Command].
<<<<<<< HEAD
Server simulates physics and sends results back to the client.

While simple, this approach has a major flaw: latency.
The NetworkManager has a LatencySimulation component to see this on your own computer.
Client actions will always feel a bit delayed while waiting for the server.

The solution to this is called Prediction:
https://mirror-networking.gitbook.io/docs/manual/general/client-side-prediction

Notes:
- Red/White ball Rigidbody CollisionMode needs to be ContinousDynamic to avoid white flying through red sometimes.
  even 'Continuous' is not enough, we need ContinuousDynamic.
=======
There will always be some latency for the results to show.

To solve this, there's another BilliardsPredicted demo which uses prediction & reconciliation.
This demo however is meant for complete beginners to learn Mirror!

Notes:
- Red/White ball Rigidbody CollisionMode needs to be ContinousDynamic to avoid white flying through red sometimes.
  even 'Continous' is not enough, we need ContinousDynamic.
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
