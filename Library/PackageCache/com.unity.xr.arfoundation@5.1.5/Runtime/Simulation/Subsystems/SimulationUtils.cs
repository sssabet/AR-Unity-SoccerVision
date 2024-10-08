using System;
using System.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    static class SimulationUtils
    {
        internal static async Task RunWithoutCancellationExceptions(Task task)
        {
            try
            {
                await task;
            }
            catch (TaskCanceledException) { }
            catch (AggregateException aggregateException)
            {
                foreach (var e in aggregateException.InnerExceptions)
                {
                    if (e is not TaskCanceledException)
                        throw e;
                }
            }
        }

        internal static TrackableId GenerateTrackableId()
        {
            Guid.NewGuid().Decompose(out var sub1, out var sub2);
            return new TrackableId(sub1, sub2);
        }

        /// <summary>
        /// Returns <see langword="true"/> if <paramref name="gameObject"/> is in the simulation environment scene.
        /// Otherwise, returns <see langword="false"/>.
        /// </summary>
        internal static bool IsInSimulationEnvironment(GameObject gameObject)
        {
            return gameObject.scene == SimulationSessionSubsystem.simulationSceneManager.environmentScene;
        }
    }
}
