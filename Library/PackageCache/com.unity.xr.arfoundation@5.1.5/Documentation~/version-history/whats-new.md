---
uid: arfoundation-whats-new
---
# What's new in version 5.1

The most significant updates in this release include:

## Added

- Added button to the XR Environment overlay that when toggled on, the Scene view displays the [XR Environment view](xref:arfoundation-simulation-xr-environment-view). When toggled off, the Scene View displays the active editing context.
- Added Camera Configuration menu to the [ARDebugMenu](xref:arfoundation-debug-menu).
- Added support for the [Environment Probes](xref:arfoundation-environment-probes) feature in XR Simulation. You can now add and remove environment probes in Play mode, add a [Simulated Environment Probe component](xref:arfoundation-simulation-environments#simulated-environment-probe-component) to XR Simulation environments, and adjust environment probe discovery settings in [XR Simulation project settings](xref:arfoundation-simulation-project-settings).
- Added support for [Anchors](xref:arfoundation-anchors) in XR Simulation.
- Added the ability for camera subsystem providers to include [EXIF](https://en.wikipedia.org/wiki/Exif) data with the `XRCameraFrame`.
- Added a set of custom [visual scripting](xref:arfoundation-visual-scripting) nodes to help you use AR Foundation in visual scripting projects.

## Fixed

- Fixed an issue where `SimulationPlaneSubsystem` would not respect the currently set `PlaneDetectionMode`, detecting updates only from planes that match the current mode.

For a full list of changes and updates in this version, see the [AR Foundation package changelog](xref:arfoundation-changelog).
