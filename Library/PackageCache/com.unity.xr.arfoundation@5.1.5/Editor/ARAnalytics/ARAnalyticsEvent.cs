using UnityEngine.Analytics;

namespace UnityEditor.XR.ARAnalytics
{
    abstract class ARAnalyticsEvent<T>
        where T: struct
    {
        protected const int k_DefaultMaxEventsPerHour = 1000;
        protected const int k_DefaultMaxElementCount = 1000;

        /// <summary>
        /// The table name for an event determines which database table it goes into in the CDP backend.
        /// All events which we want grouped into a table must share the same table name.
        /// </summary>
        protected readonly string k_TableName;

        /// <summary>
        /// The version of the schema table for an event.
        /// </summary>
        protected readonly int k_Version;

        protected int m_MaxEventsPerHour;
        protected int m_MaxElementCount;

        /// <summary>
        /// <c>True</c> if the event is already registered with Unity analytics API, otherwise <c>False</c>.
        /// </summary>
        bool m_Registered;

        protected ARAnalyticsEvent(string tableName, int version = ARAnalyticsConstants.defaultVersion, int maxEventsPerHour = k_DefaultMaxEventsPerHour, int maxElementCount = k_DefaultMaxElementCount)
        {
            k_TableName = tableName;
            k_Version = version;
            m_MaxEventsPerHour = maxEventsPerHour;
            m_MaxElementCount = maxElementCount;
        }

        public bool Register()
        {
            if (m_Registered)
                return m_Registered;

            var result = RegisterWithAnalyticsServer();

            // AnalyticsResult.TooManyRequests means that we have already registered for this tableName
            if (result != AnalyticsResult.Ok && result != AnalyticsResult.TooManyRequests)
                m_Registered = false;
            else
                m_Registered = true;

            return m_Registered;
        }

        protected abstract AnalyticsResult RegisterWithAnalyticsServer();

        public bool Send(T eventArgs) => m_Registered && SendEvent(eventArgs);

        protected abstract bool SendEvent(T eventArgs);
    }
}
