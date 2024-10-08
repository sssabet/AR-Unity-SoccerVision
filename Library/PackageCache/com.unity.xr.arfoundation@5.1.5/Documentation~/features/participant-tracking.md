---
uid: arfoundation-participant-tracking
---
# AR Participant Manager component

The participant manager is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers).

![AR Participant Manager component](../images/ar-participant-manager.png)<br/>*AR Participant Manager component*

A *participant* is a user in a multi-user collaborative session. This feature currently has limited platform support (ARKit only at the time of writing).

Like all trackables, a participant can be identified by their `TrackableId`, which is unique to that participant. Participants can be added, updated, or removed, which corresponds to joining a collaborative session, updating the pose of the participant, and exiting a collaborative session, respectively. Participants are detected automatically, like planes or images. You can't create or destroy participants.
