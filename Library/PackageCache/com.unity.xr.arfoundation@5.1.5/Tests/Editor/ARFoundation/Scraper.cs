#if UNITY_CODING_ENABLED
using Unity.Coding.Editor.ApiScraping;

namespace UnityEditor.XR.ARFoundation.Tests
{
    static class Scraper
    {
        [MenuItem("AR Foundation/Tests/Scrape APIs")]
        static void ScrapeApis() => ApiScraping.Scrape();
    }
}
#endif
