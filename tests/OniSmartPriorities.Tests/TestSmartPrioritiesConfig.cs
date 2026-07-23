namespace OniSmartPriorities
{
    // Config minima: i test della policy no ga bisogno de caricar Unity.
    public sealed class SmartPrioritiesConfig
    {
        public int MinimumPriority { get; set; } = 2;

        public int EqualLevelPriority { get; set; } = 3;

        public int MaximumPriority { get; set; } = 5;
    }
}
