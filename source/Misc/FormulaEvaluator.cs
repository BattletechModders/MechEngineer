using System;
using System.Collections.Generic;
using System.Data;

namespace MechEngineer
{
    public class FormulaEvaluator
    {
        //public static FormulaEvaluator Shared = new FormulaEvaluator();

        private readonly DataTable table;
        private FormulaEvaluator()
        {
            table =  new DataTable();
            table.Columns.Add("column", typeof(double));
            table.Rows.Add(1.0);
        }

        private object Compute(string expr)
        {
            var value = table.Compute(expr, null);
            return value != DBNull.Value ? value : null;
        }

        public object Evaluate(string expression, Dictionary<string, object> variables = null)
        {
            if (variables != null)
            {
                foreach (var keyvalue in variables)
                {
                    expression = expression.Replace("{{" + keyvalue.Key + "}}", keyvalue.Value.ToString());
                }
            }

            return Compute(expression);
        }
    }
}
