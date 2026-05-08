using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace ActsFromThePast.Acts.TheBeyond.Encounters;

public sealed class MindBloomSlimeBoss : CustomEncounterModel
{
    public MindBloomSlimeBoss() : base(RoomType.Monster)
    {
    }
    
    public override bool IsValidForAct(ActModel act) => false;

    public override bool HasScene => true;

    public override IReadOnlyList<string> Slots => new[]
    {
        "spike_med_1", "spike_large", "spike_med_2",
        "acid_med_1", "slime_boss", "acid_large", "acid_med_2"
    };

    public override IEnumerable<MonsterModel> AllPossibleMonsters
    {
        get
        {
            return new List<MonsterModel>
            {
                ModelDb.Monster<SlimeBoss>(),
                ModelDb.Monster<SpikeSlimeLarge>(),
                ModelDb.Monster<SpikeSlimeMedium>(),
                ModelDb.Monster<AcidSlimeLarge>(),
                ModelDb.Monster<AcidSlimeMedium>(),
            };
        }
    }

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        return new List<(MonsterModel, string?)>
        {
            (ModelDb.Monster<SlimeBoss>().ToMutable(), "slime_boss")
        };
    }
}