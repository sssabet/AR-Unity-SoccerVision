---
uid: arcore-point-clouds
---
# Point clouds

This page is a supplement to the AR Foundation [Point clouds](xref:arfoundation-point-clouds) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

ARCore's point cloud subsystem only produces one [XRPointCloud](xref:UnityEngine.XR.ARSubsystems.XRPointCloud).

When you use the raycast subsystem to cast a ray at a point in the cloud, the returned pose orientation provides an estimate for the surface that point might represent.
