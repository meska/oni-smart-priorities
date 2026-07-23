using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace OniSmartPriorities
{
    /// <summary>
    /// Adds per-duplicant automation controls to ONI's full-screen priorities table.
    /// </summary>
    internal static class SmartPrioritiesJobsColumn
    {
        private const string ColumnId = "SmartPriorities";
        private static readonly MethodInfo RegisterColumnMethod = AccessTools.Method(
            typeof(TableScreen),
            "RegisterColumn",
            new[] { typeof(string), typeof(TableColumn) });

        private static CheckboxTableColumn column;

        public static void Register(JobsTableScreen screen)
        {
            try
            {
                var candidate = new CheckboxTableColumn(
                    LoadValue,
                    GetValue,
                    OnPressed,
                    SetValue,
                    Compare,
                    SetTooltip,
                    SetSortTooltip);

                // RegisterColumn xe protetto: Harmony ne apre la porta senza patchare ONI.
                var registered = RegisterColumnMethod != null
                    && (bool)RegisterColumnMethod.Invoke(
                        screen,
                        new object[] { ColumnId, candidate });
                if (registered)
                {
                    column = candidate;
                    Debug.Log(
                        "[Smart Priorities] Added management column to Priorities.");
                }
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    $"[Smart Priorities] Cannot add Priorities column: {exception}");
            }
        }

        private static void LoadValue(
            IAssignableIdentity identity,
            GameObject widget)
        {
            var row = widget.GetComponentInParent<TableRow>();
            if (row == null || row.rowType == TableRow.RowType.Default)
            {
                // La riga "nuovi duplicanti" resta sempre automatica, no sta confonderla.
                widget.SetActive(false);
                return;
            }

            widget.SetActive(true);
            GetToggle(widget, row.rowType)?.ChangeState(
                (int)GetValue(identity, widget));
        }

        private static TableScreen.ResultValues GetValue(
            IAssignableIdentity identity,
            GameObject widget)
        {
            var row = widget.GetComponentInParent<TableRow>();
            if (row != null && row.rowType == TableRow.RowType.Header)
            {
                var states = Components.LiveMinionIdentities.Items
                    .Where(minion => minion != null)
                    .Select(IsEnabled)
                    .Distinct()
                    .ToArray();
                if (states.Length > 1)
                {
                    return TableScreen.ResultValues.Partial;
                }

                return states.Length == 1 && states[0]
                    ? TableScreen.ResultValues.True
                    : TableScreen.ResultValues.False;
            }

            return identity is MinionIdentity minion && IsEnabled(minion)
                ? TableScreen.ResultValues.True
                : TableScreen.ResultValues.False;
        }

        private static void OnPressed(GameObject widget)
        {
            var row = widget.GetComponentInParent<TableRow>();
            if (row == null || row.rowType == TableRow.RowType.Default)
            {
                return;
            }

            if (row.rowType == TableRow.RowType.Header)
            {
                var enable = GetValue(null, widget) != TableScreen.ResultValues.True;
                foreach (var minion in Components.LiveMinionIdentities.Items)
                {
                    minion?.GetComponent<SmartPrioritiesState>()?.SetEnabled(enable);
                }
            }
            else if (row.GetIdentity() is MinionIdentity minion)
            {
                var state = minion.GetComponent<SmartPrioritiesState>();
                state?.SetEnabled(!state.Enabled);
            }

            column?.MarkDirty();
        }

        private static void SetValue(
            GameObject widget,
            TableScreen.ResultValues value)
        {
            var row = widget.GetComponentInParent<TableRow>();
            if (row?.GetIdentity() is MinionIdentity minion)
            {
                minion.GetComponent<SmartPrioritiesState>()?.SetEnabled(
                    value == TableScreen.ResultValues.True);
                column?.MarkDirty();
            }
        }

        private static int Compare(
            IAssignableIdentity left,
            IAssignableIdentity right)
        {
            var leftEnabled = left is MinionIdentity leftMinion
                && IsEnabled(leftMinion);
            var rightEnabled = right is MinionIdentity rightMinion
                && IsEnabled(rightMinion);
            return rightEnabled.CompareTo(leftEnabled);
        }

        private static bool IsEnabled(MinionIdentity minion)
        {
            return minion.GetComponent<SmartPrioritiesState>()?.Enabled ?? true;
        }

        private static MultiToggle GetToggle(
            GameObject widget,
            TableRow.RowType rowType)
        {
            if (rowType == TableRow.RowType.Header)
            {
                var references = widget.GetComponent<HierarchyReferences>();
                if (references != null && references.HasReference("Toggle"))
                {
                    return references.GetReference("Toggle") as MultiToggle;
                }
            }

            return widget.GetComponent<MultiToggle>();
        }

        private static void SetTooltip(
            IAssignableIdentity identity,
            GameObject widget,
            ToolTip tooltip)
        {
            tooltip.ClearMultiStringTooltip();
            var row = widget.GetComponentInParent<TableRow>();
            var text = row != null && row.rowType == TableRow.RowType.Header
                ? "Enable or disable Smart Priorities for all duplicants."
                : "Let Smart Priorities manage this duplicant automatically.";
            tooltip.AddMultiStringTooltip(text, null);
        }

        private static void SetSortTooltip(
            IAssignableIdentity identity,
            GameObject widget,
            ToolTip tooltip)
        {
            tooltip.ClearMultiStringTooltip();
            tooltip.AddMultiStringTooltip(
                "Sort by Smart Priorities management state.",
                null);
        }
    }
}
