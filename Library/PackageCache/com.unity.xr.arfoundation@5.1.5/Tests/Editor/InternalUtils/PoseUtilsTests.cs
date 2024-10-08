using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.ARFoundation.InternalUtils;

namespace UnityEditor.XR.ARFoundation.InternalUtils.Tests
{
    internal class PoseUtilsTests
    {
        [Test]
        public void CalculateOffset_CalculatesCorrectly()
        {
            var a = new Pose(new Vector3(5, 0, 0), Quaternion.identity);
            var b = new Pose(new Vector3(5, 5, 0), Quaternion.Euler(0, 90, 0));
            var offset = PoseUtils.CalculateOffset(a, b);

            Assert.That(offset.position.Equals(new Vector3(0, 5, 0)), "Offset position is incorrect.");
            Assert.That(offset.rotation.Equals(Quaternion.Euler(0, 90, 0)), "Offset rotation is incorrect");
        }

        [Test]
        public void CalculateOffset_AddOffset_RoundtripsCorrectly()
        {
            var a = new Pose(new Vector3(10, 20, 30), Quaternion.Euler(30, 45, 30));
            var b = new Pose(new Vector3(-75, 15, 0), Quaternion.Euler(100, 100, 100));
            var offset = PoseUtils.CalculateOffset(a, b);
            var bPrime = a.WithOffset(offset);

            Assert.That(bPrime.position.Equals(b.position),
                $"bPrime.position ({bPrime.position}) != b.Position ({b.position}).");
            
            Assert.That(bPrime.rotation == b.rotation,
                $"bPrime.rotation ({bPrime.rotation.eulerAngles}) != b.Rotation ({b.rotation.eulerAngles}).");
        }
    }
}
