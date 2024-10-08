---
uid: arfoundation-environment-probes
---
# Environment Probes

Environment probes capture real-world imagery from a camera and organize that information into an environment texture, such as a cube map, that contains the view in all directions from a certain point in the scene. Rendering 3D objects using this environment texture allows for real-world imagery to be reflected in the rendered objects, which creates realistic reflections and lighting of virtual objects as influenced by the real-world views.

The following image illustrates the use of the environment texture from an environment probe applied to a sphere as a reflection map.

![Sphere with a reflection map from an environment probe](../images/ar-environment-probe-reflection-example.png)

## Transform and bounding volume size

Environment probes can be placed at a real-world location to capture environment texturing information. Each environment probe has a scale, orientation, position, and bounding volume size.

The scale, orientation, and position properties define the environment probe's transformation relative to the XR origin.

The bounding size defines the volume around the environment probe's position. An infinite bounding size indicates that the environment texture can be used for global lighting. A finite bounding size indicates that the environment texture captures the local lighting conditions in a specific area surrounding the probe.

## Place an environment probe

Environment probes can be placed manually, automatically, or using both methods.

### Manual placement

> [!NOTE]
> Support for manual placement and removal of environment probes depends on the underlying AR framework's capabilities. Check the [subsystem's descriptor](xref:UnityEngine.XR.ARSubsystems.XREnvironmentProbeSubsystemDescriptor) before attempting to add or destroy an environment probe.

To create an environment probe, add an [AREnvironmentProbe](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbe) component to a GameObject using [AddComponent](xref:UnityEngine.GameObject.AddComponent(System.Type)). Like [anchors](xref:arfoundation-anchors), the [AREnvironmentProbe](xref:UnityEngine.XR.ARFoundation.AREnvironmentProbe) might be in a pending state for a few frames.

To remove an environment probe, [Destroy](xref:UnityEngine.Object.Destroy(UnityEngine.Object)) it as you would any component or GameObject.

> [!TIP]
> If you want to capture the most accurate environment information for a specific virtual object, place the probe close to the location of that object. This increases the quality of the rendered object. If a virtual object is moving and you know the path of that movement, you can place multiple environment probes along the path so the rendered object can better reflect the virtual object's movement through the real-world environment.

### Automatic placement

With automatic environment probe placement, the device automatically selects suitable locations for environment probes and creates them.

Environment probes can be created in any orientation. However, Unity's reflection probes, which consume the environment probe data, only support axis-aligned orientations. This means the orientation you specify (or your application selected automatically) might not be fully respected.

> [!TIP]
> Automatically placed environment probes offer a good overall set of environment information for real-world features. However, manually placing environment probes close to key virtual scene objects allows for a better environmental rendering quality of those objects.

# AR Environment Probe Manager component

The environment probe manager is a type of [trackable manager](xref:arfoundation-managers#trackables-and-trackable-managers).

![AR Environment Probe Manager component](../images/ar-environment-probe-manager.png)

## Texture filter mode

This corresponds to the [UnityEngine.FilterMode](https://docs.unity3d.com/ScriptReference/FilterMode.html) for the cubemap that the environment probe generates.

## Debug Prefab

This Prefab is instantiated for each manually or automatically placed environment probe. This is not required, but Unity provides it for debugging purposes.
