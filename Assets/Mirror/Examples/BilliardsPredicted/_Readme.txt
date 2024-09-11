Advanced multiplayer Billiards demo with Prediction.
<<<<<<< HEAD

Please read this first:
https://mirror-networking.gitbook.io/docs/manual/general/client-side-prediction

=======
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc
Mouse drag the white ball to apply force.
PredictedRigidbody syncInterval is intentionally set pretty high so we can see when it corrects.

If you are a beginner, start with the basic Billiards demo instead.
If you are advanced, this demo shows how to use Mirror's prediction features for physics / FPS games.

<<<<<<< HEAD
Billiards is a great example to try our Prediction algorithm, it works extremely well here!

=> We use 'Fast' Prediction mode for Billiards because we want to see exact collisions with balls/walls.
=> 'Smooth' mode would look too soft, with balls changing direction even before touching other balls/walls.
=======
The demo is work in progress.
At the moment, this is only for the Mirror team to test individual prediction features!
>>>>>>> d743f7baf8e1636f6e77565a0767ec6a5c5e24fc

Notes:
- Red/White ball Rigidbody CollisionMode needs to be ContinousDynamic to avoid white flying through red sometimes.
  even 'Continous' is not enough, we need ContinousDynamic.
