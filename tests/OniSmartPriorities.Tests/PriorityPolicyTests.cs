using Xunit;

namespace OniSmartPriorities.Tests
{
    public sealed class PriorityPolicyTests
    {
        private readonly SmartPrioritiesConfig config = new();

        [Fact]
        public void EmptyColonyProducesNoAssignments()
        {
            var result = PriorityPolicy.Assign(
                new Dictionary<string, int>(),
                config);

            Assert.Empty(result);
        }

        [Fact]
        public void EqualLevelsKeepEveryoneAtNormalPriority()
        {
            var result = PriorityPolicy.Assign(
                new Dictionary<string, int>
                {
                    ["Ada"] = 0,
                    ["Burt"] = 0,
                    ["Camille"] = 0
                },
                config);

            Assert.All(result.Values, priority => Assert.Equal(3, priority));
        }

        [Fact]
        public void TwoLevelsUseFullRelativeRange()
        {
            var result = PriorityPolicy.Assign(
                new Dictionary<string, int>
                {
                    ["Ada"] = 9,
                    ["Burt"] = 2
                },
                config);

            Assert.Equal(5, result["Ada"]);
            Assert.Equal(2, result["Burt"]);
        }

        [Fact]
        public void FourLevelsMapFromVeryHighToLow()
        {
            var result = PriorityPolicy.Assign(
                new Dictionary<string, int>
                {
                    ["Ada"] = 12,
                    ["Burt"] = 8,
                    ["Camille"] = 4,
                    ["Devon"] = 0
                },
                config);

            Assert.Equal(5, result["Ada"]);
            Assert.Equal(4, result["Burt"]);
            Assert.Equal(3, result["Camille"]);
            Assert.Equal(2, result["Devon"]);
        }

        [Fact]
        public void TiedWorkersReceiveTheSamePriority()
        {
            var result = PriorityPolicy.Assign(
                new Dictionary<string, int>
                {
                    ["Ada"] = 10,
                    ["Burt"] = 10,
                    ["Camille"] = 2
                },
                config);

            Assert.Equal(5, result["Ada"]);
            Assert.Equal(result["Ada"], result["Burt"]);
            Assert.Equal(2, result["Camille"]);
        }

        [Fact]
        public void EveryEligibleWorkerKeepsPositiveCoverage()
        {
            var result = PriorityPolicy.Assign(
                new Dictionary<string, int>
                {
                    ["Ada"] = 20,
                    ["Burt"] = 7,
                    ["Camille"] = 3,
                    ["Devon"] = 1,
                    ["Ellie"] = 0
                },
                config);

            Assert.All(
                result.Values,
                priority => Assert.InRange(priority, 2, 5));
        }
    }
}
