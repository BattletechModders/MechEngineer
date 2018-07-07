using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace MechEngineer
{
    public class FormulaEvaluator
    {
        public static FormulaEvaluator Shared = new FormulaEvaluator();

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

        private static readonly Regex Regex = new Regex(@"(?:\[\[([^\]]+)\]\])", RegexOptions.Singleline | RegexOptions.Compiled);

        public object Evaluate(string expression, Dictionary<string, string> variables = null)
        {
            if (variables != null)
            {
                var keys = new HashSet<string>();
                foreach (Match match in Regex.Matches(expression))
                {
                    var key = match.Groups[1].Value;
                    keys.Add(key);
                }

                foreach (var key in keys)
                {
                    if (!variables.TryGetValue(key, out var value))
                    {
                        value = "1";
                    }
                    var placeholder = "[[" + key + "]]";
                    //Control.mod.Logger.LogDebug($"key={key} value={value}");
                    expression = expression.Replace(placeholder, value);
                }
            }

            return Compute(expression);
        }
    }
}
