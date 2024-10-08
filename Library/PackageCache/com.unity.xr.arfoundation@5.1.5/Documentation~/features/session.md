---
uid: arfoundation-session
---
# AR Session component

The [ARSession](xref:UnityEngine.XR.ARFoundation.ARSession) component controls the lifecycle of an AR experience by enabling or disabling AR on the target platform.

![AR Session component](../images/ar-session.png)<br/>*AR Session component*

A *session* refers to an instance of AR. While other features like plane detection can be independently enabled or disabled, the session controls the lifecycle of all AR features. When you disable the AR Session component, the system no longer tracks features in its environment. Then if you enable it at a later time, the system attempts to recover and maintain previously-detected features.

> [!NOTE]
> Multiple AR Session components in the same scene can conflict with each other, therefore Unity recommends that you add at most one AR Session component to a scene.

## Check for device support

Some platforms have AR capabilities built into the device's operating system. On others, AR software might be able to be installed on-demand, or AR may not be supported at all. Your application needs to be able to detect support for AR Foundation so it can provide an alternative experience when AR is not supported.

The question "is AR available on this device?" might require checking a remote server for software availability, so you should call [CheckAvailability](xref:UnityEngine.XR.ARFoundation.ARSession.CheckAvailability) to determine if AR is available:


```csharp
public class MyComponent {
    [SerializeField] ARSession m_Session;

    IEnumerator Start() {
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            // Start some fallback experience for unsupported devices
        }
        else
        {
            // Start the AR session
            m_Session.enabled = true;
        }
    }
}
```

If you set **Attempt Update** to **true**, the device tries to install AR software if possible. Support for this feature is platform-dependent.

### Session state

Use [ARSession.state](xref:UnityEngine.XR.ARFoundation.ARSession.state) to get the current session state. You can also subscribe to the [ARSession.stateChanged](xref:UnityEngine.XR.ARFoundation.ARSession.stateChanged) event to receive a callback when the state changes. Refer to the table below for a list of all possible states:

| Session state | Description |
| :------------ | :---------- |
| [None](xref:UnityEngine.XR.ARFoundation.ARSessionState.None) | AR has not been initialized and availability is unknown. You can call [CheckAvailability](xref:UnityEngine.XR.ARFoundation.ARSession.CheckAvailability) to check availability of AR on the device. |
| [Unsupported](xref:UnityEngine.XR.ARFoundation.ARSessionState.Unsupported) | The device does not support AR. |
| [CheckingAvailability](xref:UnityEngine.XR.ARFoundation.ARSessionState.CheckingAvailability) | The session subsystem is currently checking availability of AR on the device. The [CheckAvailability](xref:UnityEngine.XR.ARFoundation.ARSession.CheckAvailability) coroutine has not yet completed. |
| [NeedsInstall](xref:UnityEngine.XR.ARFoundation.ARSessionState.NeedsInstall) | The device supports AR, but requires additional software to be installed. If the provider [supports runtime installation](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystemDescriptor.supportsInstall), you can call [Install](xref:UnityEngine.XR.ARFoundation.ARSession.Install) to attempt installation of AR software on the device. |
| [Installing](xref:UnityEngine.XR.ARFoundation.ARSessionState.Installing) | AR software is currently installing. The [Install](xref:UnityEngine.XR.ARFoundation.ARSession.Install) coroutine has not yet completed. |
| [Ready](xref:UnityEngine.XR.ARFoundation.ARSessionState.Ready) | The device supports AR, and any necessary software is installed. This state will automatically change to either `SessionInitializing` or `SessionTracking`. |
| [SessionInitializing](xref:UnityEngine.XR.ARFoundation.ARSessionState.SessionInitializing) | The AR session is currently initializing. This usually means AR is running, but not yet tracking successfully. |
| [SessionTracking](xref:UnityEngine.XR.ARFoundation.ARSessionState.SessionTracking) | The AR session is running and tracking successfully. The device is able to determine its position and orientation in the world. If tracking is lost during a session, this state may change to `SessionInitializing`. |

# AR Input Manager

The [ARInputManager](xref:UnityEngine.XR.ARFoundation.ARInputManager) component enables world tracking. By default, an [ARInputManager](xref:UnityEngine.XR.ARFoundation.ARInputManager) component is included on the AR Session GameObject.

> [!NOTE]
> Without the AR Input Manager component, [XROrigin](xref:Unity.XR.CoreUtils.XROrigin) can't acquire a world-space pose for the device. The AR Input Manager component is required for AR to function properly.

![AR Input Manager component](../images/ar-input-manager.png)<br/>*AR Input Manager component*

You can move the AR Input Manager component anywhere in your scene hierarchy, but you shouldn't have more than one per scene.
