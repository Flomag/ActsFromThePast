using ActsFromThePast.Interfaces;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace ActsFromThePast;

public static class ShrineEventPool
{
    private static readonly Dictionary<ActModel, List<EventModel>> ShrinePools = new();
    private static readonly Dictionary<ActModel, int> ShrineCounters = new();

    public static void SetPool(ActModel act, List<EventModel> shrines)
    {
        ShrinePools[act] = shrines;
        ShrineCounters[act] = 0;
    }

    public static bool HasShrines(ActModel act) =>
        ShrinePools.TryGetValue(act, out var list) && list.Count > 0;

    public static EventModel? PullNextShrine(ActModel act, RunState runState)
    {
        if (!ShrinePools.TryGetValue(act, out var shrines) || shrines.Count == 0)
            return null;

        var eligible = shrines.Where(s =>
            s.IsAllowed((IRunState)runState) &&
            !(s is IShrineEvent { IsOneTimeEvent: true } &&
              runState.VisitedEventIds.Contains(s.Id))
        ).ToList();

        if (eligible.Count == 0)
            return null;

        var chosen = runState.Rng.Niche.NextItem(eligible);
        shrines.Remove(chosen);
        return chosen;
    }
}