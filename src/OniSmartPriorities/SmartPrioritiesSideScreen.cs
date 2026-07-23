using PeterHan.PLib.UI;
using UnityEngine;

namespace OniSmartPriorities
{
    public sealed class SmartPrioritiesSideScreen : SideScreenContent
    {
        private GameObject checkbox;
        private SmartPrioritiesState state;

        public override string GetTitle()
        {
            return "Smart Priorities";
        }

        public override bool IsValidForTarget(GameObject target)
        {
            if (target == null)
            {
                return false;
            }

            var minion = target.GetComponent<MinionIdentity>();
            return minion != null
                && !minion.HasTag(GameTags.Dead)
                && target.GetComponent<SmartPrioritiesState>() != null;
        }

        protected override void OnPrefabInit()
        {
            base.OnPrefabInit();

            var panel = new PPanel("SmartPrioritiesPanel")
            {
                Direction = PanelDirection.Vertical,
                Alignment = TextAnchor.UpperLeft,
                Margin = new RectOffset(8, 8, 8, 8)
            };
            panel.AddChild(
                new PCheckBox("AutomaticPriorities")
                {
                    Text = "Manage this duplicant automatically",
                    ToolTip = "When disabled, Smart Priorities leaves this duplicant's "
                        + "personal work priorities untouched.",
                    CheckSize = new Vector2(20f, 20f),
                    OnChecked = OnChecked
                }.SetKleiPinkStyle().AddOnRealize(realized => checkbox = realized));

            ContentContainer = panel.AddTo(gameObject, 0);
            Refresh();
        }

        public override void SetTarget(GameObject target)
        {
            state = IsValidForTarget(target)
                ? target.GetComponent<SmartPrioritiesState>()
                : null;
            Refresh();
        }

        public override void ClearTarget()
        {
            state = null;
            base.ClearTarget();
        }

        private void OnChecked(GameObject _, int checkState)
        {
            state?.SetEnabled(checkState == PCheckBox.STATE_CHECKED);
        }

        private void Refresh()
        {
            if (checkbox == null || state == null)
            {
                return;
            }

            PCheckBox.SetCheckState(
                checkbox,
                state.Enabled
                    ? PCheckBox.STATE_CHECKED
                    : PCheckBox.STATE_UNCHECKED);
        }
    }
}
