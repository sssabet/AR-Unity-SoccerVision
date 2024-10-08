---
uid: arcore-session-recording
---
# ARCore Session Recording

ARCore allows you to record an ArSession to an `.mp4` and play it back at a later time. To support this feature, the [ARCoreSessionSubsystem](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem) exposes the following methods:

* [StartRecording](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem.StartRecording(UnityEngine.XR.ARCore.ArRecordingConfig))
* [StopRecording](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem.StopRecording)
* [StartPlayback](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem.StartPlayback(System.String))
* [StopPlayback](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem.StopPlayback)

To start a recording, supply an [ArRecordingConfig](xref:UnityEngine.XR.ARCore.ArRecordingConfig). This specifies the file name that Unity saves the recording as, as well as other options. Call `StopRecording` to stop recording. When Unity stops recording, it creates the `.mp4` file as specified in the `ArRecordingConfig`. This contains the camera feed and sensor data required by ARCore.

To play back a video, use the `StartPlayback` method, and specify an `.mp4` file created during an earlier recording.

To start or stop a recorded file in ARCore, the [ARCoreSessionSubsystem](xref:UnityEngine.XR.ARCore.ARCoreSessionSubsystem) pauses the session. Pausing and resuming a session can take between 0.5 and 1.0 seconds.

**Note**: Video recordings contain sensor data, but not the computed results. ARCore does not always produce the same output, which means trackables might not be consistent between playbacks of the same recording. For example, multiple playbacks of the same recording might give different plane detection results.
