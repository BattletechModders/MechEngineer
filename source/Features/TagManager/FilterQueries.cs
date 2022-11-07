using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BattleTech;
using BattleTech.Data;

namespace MechEngineer.Features.TagManager;

internal class FilterQueries
{
    internal static List<string> AllTags()
    {
        var includeLikes = new[] { "unit\\_%" };
        var excludeLikes = new[] { "unit\\_chassis\\_%" };
        var queryString = @"SELECT DISTINCT TagName FROM TagSetTag";

        var limits = new List<string>();
        foreach (var includeLike in includeLikes)
        {
            limits.Add(@$"TagName LIKE {Quote(includeLike)} ESCAPE '\'");
        }
        foreach (var excludeLike in excludeLikes)
        {
            limits.Add(@$"TagName NOT LIKE {Quote(excludeLike)} ESCAPE '\'");
        }
        if (limits.Count > 0)
        {
            queryString += " WHERE " + string.Join(" AND ", limits);
        }

        Control.Logger.Trace?.Log(queryString);
        return MetadataDatabase.Instance.Query<string>(queryString, null).ToList();
    }

    private readonly TagManagerSettings.TagsFilterSet _filterSet;
    private readonly MetadataDatabase _mdd = MetadataDatabase.Instance;

    internal FilterQueries(TagManagerSettings.TagsFilterSet filterSet)
    {
        _filterSet = filterSet;
    }

    internal List<string> MechIds()
    {
        return QueryItems("UnitDefID", "UnitDef", _filterSet.Mechs, MechValidationRules.MechTag_Custom, "d.UnitTypeID = 1");
    }

    internal List<string> PilotIds()
    {
        return QueryItems("PilotDefID", "PilotDef", _filterSet.Pilots);
    }

    internal List<string> LanceIds()
    {
        return QueryItems("LanceDefID", "LanceDef", _filterSet.Lances, MechValidationRules.LanceTag_Custom);
    }

    internal int MechCount => MechIds().Count;

    private List<string> QueryItems(string idColumn, string tableName, TagManagerSettings.TagsFilter filter, string? forceLoadTag = null, string? customFilter = null)
    {
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            var queryString = @$"SELECT DISTINCT {idColumn} FROM {tableName} d";

            var orForceLoadTag = new List<string>();
            if (forceLoadTag != null)
            {
                orForceLoadTag.Add(ExistsIn(forceLoadTag));
            }

            {
                var andFilters = new List<string>();
                if (filter.NotContainsAny != null)
                {
                    andFilters.Add(NotExistsIn(filter.NotContainsAny));
                }

                if (filter.ContainsAny != null)
                {
                    andFilters.Add(ExistsIn(filter.ContainsAny));
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
                        andFilters.Add(@$"({termQueries})");
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

                        var orOptions = new List<string>();
                        foreach (var option in group.Options)
                        {
                            if (!option.OptionActive)
                            {
                                continue;
                            }

                            var andOptionTagsFilters = new List<string>();
                            if (option.NotContainsAny != null)
                            {
                                andOptionTagsFilters.Add(NotExistsIn(option.NotContainsAny));
                            }
                            if (option.ContainsAny != null)
                            {
                                andOptionTagsFilters.Add(ExistsIn(option.ContainsAny));
                            }
                            JoinAndAddIfNotEmpty(orOptions, " AND ", andOptionTagsFilters);
                        }
                        JoinAndAddIfNotEmpty(andFilters, " OR ", orOptions);
                    }
                }
                JoinAndAddIfNotEmpty(orForceLoadTag, " AND ", andFilters);
            }

            var andCustom = new List<string>();
            if (customFilter != null)
            {
                andCustom.Add(customFilter);
            }
            if (orForceLoadTag.Count > 0)
            {
                JoinAndAddIfNotEmpty(andCustom, " OR ", orForceLoadTag);
            }
            if (andCustom.Count > 0)
            {
                queryString += @$" WHERE {string.Join(" AND ", andCustom)}";
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
            outer.Add("(" + string.Join(separator, inner) + ")");
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