namespace UnityEngine.XR.ARCore.Tests
{
    class CodeExamples
    {
        #region ArRecordingConfig_example

        void RecordExample(ARCoreSessionSubsystem subsystem, string mp4Path)
        {
            var session = subsystem.session;
            using (var config = new ArRecordingConfig(session))
            {
                config.SetMp4DatasetFilePath(session, mp4Path);
                config.SetRecordingRotation(session, 90);
                config.SetAutoStopOnPause(session, false);
                var status = subsystem.StartRecording(config);
                Debug.Log($"StartRecording to {config.GetMp4DatasetFilePath(session)} => {status}");
            }
        }
        #endregion
    }
}
