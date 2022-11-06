using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer.Features.TagManager;

internal class FilterQueries
{
    private readonly TagManagerSettings.TagsFilterSet _filterSet;
    private readonly MetadataDatabase _mdd = MetadataDatabase.Instance;

    internal FilterQueries(TagManagerSettings.TagsFilterSet filterSet)
    {
        _filterSet = filterSet;
    }

    internal List<string> PilotIds()
    {
        return QueryItems("PilotDefID", "PilotDef", _filterSet.Pilots);
    }

    internal List<string> MechIds()
    {
        return QueryItems("UnitDefID", "UnitDef", _filterSet.Mechs, MechValidationRules.MechTag_Custom);
    }

    internal List<string> LanceIds()
    {
        return QueryItems("LanceDefID", "LanceDef", _filterSet.Lances, MechValidationRules.LanceTag_Custom);
    }

    internal int MechCount => MechIds().Count;

    private List<string> QueryItems(string idColumn, string tableName, TagManagerSettings.TagsFilter filter, string? forceLoadTag = null)
    {
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            var queryString = @$"SELECT DISTINCT {idColumn} FROM {tableName} d";

            var outerOr = new List<string>();
            if (forceLoadTag != null)
            {
                outerOr.Add(ExistsIn(forceLoadTag));
            }

            {
                var innerAnd = new List<string>();
                if (filter.BlockAny != null)
                {
                    innerAnd.Add(NotExistsIn(filter.BlockAny));
                }

                if (filter.AllowAny != null)
                {
                    innerAnd.Add(ExistsIn(filter.AllowAny));
                }

                if (filter.OptionsSearch != null)
                {
                    var termQueries = string.Join(" AND ",
                        filter.OptionsSearch
                            .Split(',')
                            .Select(term =>
                            {
                                if (term.StartsWith("!"))
                                {
                                    return "NOT " + ExistsLike(term.Substring(1));
                                }
                                return ExistsLike(term);
                            })
                        );

                    if (termQueries.Length > 0)
                    {
                        innerAnd.Add(@$"({termQueries})");
                    }
                }

                if (filter.OptionsGroups != null)
                {
                    foreach (var group in filter.OptionsGroups)
                    {
                        if (group.Options.All(o => o.OptionActive) || group.Options.All(o => !o.OptionActive))
                        {
                            continue;
                        }

                        var innerOr = new List<string>();
                        foreach (var option in group.Options)
                        {
                            if (!option.OptionActive)
                            {
                                continue;
                            }

                            var extremeInnerAnd = new List<string>();
                            if (option.ExcludeAny != null)
                            {
                                extremeInnerAnd.Add(NotExistsIn(option.ExcludeAny));
                            }
                            if (option.IncludeAny != null)
                            {
                                extremeInnerAnd.Add(ExistsIn(option.IncludeAny));
                            }
                            JoinAndAddIfNotEmpty(innerOr, " AND ", extremeInnerAnd);
                        }
                        JoinAndAddIfNotEmpty(innerAnd, " OR ", innerOr);
                    }
                }
                JoinAndAddIfNotEmpty(outerOr, " AND ", innerAnd);
            }

            if (outerOr.Count > 0)
            {
                queryString += @$" WHERE {string.Join(" OR ", outerOr)}";
            }

            Control.Logger.Trace?.Log(queryString);
            return _mdd.Query<string>(queryString, null).ToList();
        }
        finally
        {
            sw.Stop();
            Control.Logger.Trace?.Log($"Query for {_filterSet.Label} in {tableName} took {sw.ElapsedMilliseconds}ms.");
        }
    }

    private static void JoinAndAddIfNotEmpty(List<string> outer, string separator, List<string> inner)
    {
        if (inner.Count > 0)
        {
            outer.Add(string.Join(separator, inner));
        }
    }

    private static string ExistsLike(string term)
    {
        return @$"EXISTS ( SELECT * FROM TagSetTag t WHERE d.TagSetID = t.TagSetID AND t.TagName LIKE {QuoteLike(term)} ESCAPE '\' )";
    }

    private static string QuoteLike(string text)
    {
        return Quote('%' + text.Replace(@"%", @"\%").Replace(@"_", @"\_") + '%');
    }

    private static string ExistsIn(params string[] tags)
    {
        var quotedTags = string.Join(",", tags.Select(Quote));
        return @$"EXISTS ( SELECT * FROM TagSetTag t WHERE d.TagSetID = t.TagSetID AND t.TagName IN ({quotedTags}) )";
    }

    private static string NotExistsIn(params string[] tags)
    {
        return "NOT " + ExistsIn(tags);
    }

    private static string Quote(string text)
    {
        return "'" + text.Replace(@"'", @"''") + "'";
    }
}