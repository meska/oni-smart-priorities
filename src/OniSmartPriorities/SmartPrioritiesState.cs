using KSerialization;

namespace OniSmartPriorities
{
    [SerializationConfig(MemberSerialization.OptIn)]
    public sealed class SmartPrioritiesState : KMonoBehaviour, ISaveLoadable
    {
        [Serialize]
        private bool automaticManagementEnabled = true;

        public bool Enabled => automaticManagementEnabled;

        public void SetEnabled(bool value)
        {
            if (automaticManagementEnabled == value)
            {
                return;
            }

            automaticManagementEnabled = value;
            // El prossimo giro ricalcola subito la classifica senza spetar un minuto.
            SmartPrioritiesController.RequestRebalance();
        }
    }
}
