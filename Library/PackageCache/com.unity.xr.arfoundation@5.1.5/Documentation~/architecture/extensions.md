---
uid: arfoundation-extensions
---
# Extending AR Foundation

Due to platform-specific implementation differences between provider plug-ins, AR Foundation may not expose the entire contents of a platform SDK via C#. Instead, AR Foundation provides a native pointer to access platform-specific functionality and data when applicable. For example, the [XRSessionSubsystem](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem) has a [nativePtr](xref:UnityEngine.XR.ARSubsystems.XRSessionSubsystem.nativePtr) property.

Each provider plug-in defines what each native pointer points to. In general, a pointer points to a struct whose first member is an `int` that contains a version number followed by the raw pointer.

In C, the `XRSessionSubsystem.nativePtr` might point to a struct like this:

```c
typedef struct UnityXRNativeSessionPtr
{
    int version;
    void* session;
} UnityXRNativeSessionPtr;
```

Structure packing and alignment rules vary by platform, so the `void* session` pointer isn't necessarily at a 4 byte offset. On a 64-bit platform, for instance, the pointer might be offset by 8 bytes to ensure the pointer is on an 8 byte boundary.

All trackables (such as planes, tracked images, or faces) provide a native pointer. You can use these pointers to access native data structures such as the native frame, session, plane, anchor, and so on.
