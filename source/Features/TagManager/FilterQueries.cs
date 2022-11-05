using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        return QueryItems("UnitDefID", "UnitDef", _filterSet.Mechs);
    }

    internal List<string> LanceIds()
    {
        return QueryItems("LanceDefID", "LanceDef", _filterSet.Lances);
    }

    internal int LancesAndMechCount => MechIds().Count + LanceIds().Count;

    private List<string> QueryItems(string idColumn, string tableName, TagManagerSettings.TagsFilter filter)
    {
        var sw = new Stopwatch();
        sw.Start();
        try
        {
            var queryString =
                $"SELECT {idColumn} FROM {tableName} d LEFT JOIN TagSetTag tst ON d.TagSetID = tst.TagSetID" +
                " WHERE tst.TagName NOT IN @Exclude";
            if (!filter.AllowByDefault)
            {
                queryString += " AND tst.TagName IN @Include";
            }

            return _mdd
                .Query<string>(queryString, new { Include = filter.Allow, Exclude = filter.Block })
                .ToList();
        }
        finally
        {
            sw.Stop();
            Control.Logger.Trace?.Log($"Query for {_filterSet.Label} in {tableName} took {sw.ElapsedMilliseconds}ms.");
        }
    }
}