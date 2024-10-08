using UnityEngine.Analytics;

namespace UnityEditor.XR.ARAnalytics
{
    class AREditorAnalyticsEvent<T> : ARAnalyticsEvent<T>
        where T : struct
    {
        internal AREditorAnalyticsEvent(string tableName, int version = ARAnalyticsConstants.defaultVersion, int maxEventsPerHour = k_DefaultMaxEventsPerHour, int maxElementCount = k_DefaultMaxElementCount)
        : base(tableName, version, maxEventsPerHour, maxElementCount) { }

        protected override AnalyticsResult RegisterWithAnalyticsServer() => EditorAnalytics.RegisterEventWithLimit(
            k_TableName,
            m_MaxEventsPerHour,
            m_MaxElementCount,
            ARAnalyticsConstants.arFoundationVendorKey,
            k_Version
        );

        protected override bool SendEvent(T eventArgs)
        {
            // The event shouldn't be able to report if user has disabled analytics
            // but if we know it won't be reported then lets not waste time gathering
            // all the data.
            if (!EditorAnalytics.enabled)
                return false;

            var result = EditorAnalytics.SendEventWithLimit(k_TableName, eventArgs, k_Version);
            return result == AnalyticsResult.Ok;
        }
    }
}
