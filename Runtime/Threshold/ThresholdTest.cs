using NUnit.Framework;

namespace Aori.DSA.Threshold
{
    public class ThresholdTest
    {
        private static Threshold<string> CreateSampleThreshold()
        {
            var threshold = new Threshold<string>();
            threshold.AddThreshold("A", 0.5f);
            threshold.AddThreshold("B", 0.7f);
            threshold.AddThreshold("C", 0.9f);
            return threshold;
        }

        [Test]
        public void GetValue_InputBelowMinimum_ReturnsDefault()
        {
            var threshold = CreateSampleThreshold();

            var result = threshold.GetValue(0.49f);

            Assert.IsNull(result);
        }

        [Test]
        public void GetValue_CloseToLowerThreshold_ReturnsA()
        {
            var threshold = CreateSampleThreshold();

            var result = threshold.GetValue(0.6f);

            Assert.AreEqual("A", result);
        }

        [Test]
        public void GetValue_EqualToMiddleThreshold_ReturnsB()
        {
            var threshold = CreateSampleThreshold();

            var result = threshold.GetValue(0.7f);

            Assert.AreEqual("B", result);
        }

        [Test]
        public void GetValue_CloseToHigherThresholdButLower_ReturnsB()
        {
            var threshold = CreateSampleThreshold();

            var result = threshold.GetValue(0.8999f);

            Assert.AreEqual("B", result);
        }

        [Test]
        public void GetValue_EqualToHigherThreshold_ReturnsC()
        {
            var threshold = CreateSampleThreshold();

            var result = threshold.GetValue(0.9f);

            Assert.AreEqual("C", result);
        }
    }
}