using System;
using System.Collections.Generic;
using System.Globalization;
using BattleTech;
using CustomComponents;

namespace MechEngineer
{
    internal class WorkOrderCostsHandler
    {
        public static readonly WorkOrderCostsHandler Shared = new WorkOrderCostsHandler();

        public void ComponentInstallWorkOrder(MechComponentRef mechComponent, ChassisLocations newLocation, WorkOrderEntry_InstallComponent result)
        {
            var workOrderCosts = mechComponent.Def.GetComponent<WorkOrderCosts>();
            if (workOrderCosts == null)
            {
                return;
            }

            if (newLocation == ChassisLocations.None) // remove
            {
                if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
                {
                    ApplyCosts(result, workOrderCosts.RemoveDestroyed);
                }
                else
                {
                    ApplyCosts(result, workOrderCosts.Remove);
                }
            }
            else // install
            {
                ApplyCosts(result, workOrderCosts.Install);
            }
        }

        public void ComponentRepairWorkOrder(MechComponentRef mechComponent, bool isOnMech, WorkOrderEntry_RepairComponent result)
        {
            var workOrderCosts = mechComponent.Def.GetComponent<WorkOrderCosts>();
            if (workOrderCosts == null)
            {
                return;
            }
            
            if (mechComponent.DamageLevel == ComponentDamageLevel.Destroyed)
            {
                ApplyCosts(result, workOrderCosts.RepairDestroyed);
            }
            else
            {
                ApplyCosts(result, workOrderCosts.Repair);
            }
        }
        
        private Dictionary<string, string> TemplateVariables()
        {
            var mechDef = Global.ActiveMechDef;
            if (mechDef == null)
            {
                return null;
            }

            var variables = new Dictionary<string, string>
            {
                // this is the only variable that is of any use and stays constant
                ["Chassis.Tonnage"] = mechDef.Chassis.Tonnage.ToString(CultureInfo.InvariantCulture)
            };

            //{ // TODO can change after and before, no use until we track armor changes too
            //    variables["ArmorTonnage"] = MechDef.ArmorTonnage().ToString(CultureInfo.InvariantCulture);
            //}

            //{ // TODO can change or even be incomplete, we should use place holders and recalculate on work order pruning
            //    var Engine = MechDef.Inventory.GetEngine();
            //    if (Engine != null)
            //    {
            //        variables["Engine.Tonnage"] = Engine.EngineTonnage.ToString(CultureInfo.InvariantCulture);
            //        variables["Engine.Rating"] = Engine.Rating.ToString(CultureInfo.InvariantCulture);
            //    }
            //}

            return variables;
        }

        private void ApplyCosts(WorkOrderEntry_MechLab workOrder, WorkOrderCosts.Costs costs)
        {
            if (costs == null)
            {
                return;
            }

            var variables = TemplateVariables();
            var adapter = new WorkOrderEntry_MechLabAdapter(workOrder);
            if (!string.IsNullOrEmpty(costs.CBillCost))
            {
                adapter.CBillCost = Convert.ToInt32(FormulaEvaluator.Shared.Evaluate(costs.CBillCost, variables));
            }
            if (!string.IsNullOrEmpty(costs.TechCost))
            {
                adapter.Cost = Convert.ToInt32(FormulaEvaluator.Shared.Evaluate(costs.TechCost, variables));
            }
        }
    }
}