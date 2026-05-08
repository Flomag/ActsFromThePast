using System.Reflection;
using ActsFromThePast.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Random;

namespace ActsFromThePast.Patches.Events;

public class ShrinePatches
{
    [HarmonyPatch(typeof(ActModel), nameof(ActModel.GenerateRooms))]
    [HarmonyPriority(Priority.Low)]
    public static class ShrinePoolPatch
    {
        private static readonly FieldInfo RoomsField =
            typeof(ActModel).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);

        public static void Postfix(ActModel __instance)
        {
            var rooms = RoomsField?.GetValue(__instance) as RoomSet;
            if (rooms == null) return;

            var shrines = rooms.events.Where(e => e is IShrineEvent).ToList();
            if (shrines.Count == 0) return;

            rooms.events.RemoveAll(e => e is IShrineEvent);
            ShrineEventPool.SetPool(__instance, shrines);
        }
    }

    [HarmonyPatch(typeof(ActModel), nameof(ActModel.PullNextEvent))]
    public static class ShrineChancePatch
    {
        private const float ShrineChance = 0.25f;

        private static readonly FieldInfo RoomsField =
            typeof(ActModel).GetField("_rooms", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool Prefix(ActModel __instance, RunState runState, ref EventModel __result)
        {
            if (!ShrineEventPool.HasShrines(__instance))
                return true;

            bool tryShrineFirst = runState.Rng.Niche.NextFloat(1f) < ShrineChance;

            if (tryShrineFirst)
            {
                var shrine = ShrineEventPool.PullNextShrine(__instance, runState);
                if (shrine != null)
                {
                    __result = Hook.ModifyNextEvent((IRunState)runState, shrine);
                    runState.AddVisitedEvent(__result);
                    return false;
                }
                // Shrine pool empty, fall through to regular
            }
            else
            {
                // 75% path — check if all regular events are exhausted
                var rooms = RoomsField?.GetValue(__instance) as RoomSet;
                if (rooms != null && rooms.events.All(e =>
                        !e.IsAllowed((IRunState)runState) ||
                        runState.VisitedEventIds.Contains(e.Id)))
                {
                    var shrine = ShrineEventPool.PullNextShrine(__instance, runState);
                    if (shrine != null)
                    {
                        __result = Hook.ModifyNextEvent((IRunState)runState, shrine);
                        runState.AddVisitedEvent(__result);
                        return false;
                    }
                }
            }

            // Fall through to original: regular event (or recycled if exhausted)
            return true;
        }
    }
}