using ActsFromThePast.Patches.Audio;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Audio;

namespace ActsFromThePast;

public static class LegacyBossHelper
{
    public static void OnBossVictory()
    {
        var controller = NRunMusicController.Instance;
        if (controller != null)
        {
            controller.StopMusic();
        }

        AFTPModAudio.Play("boss", "boss_victory_stinger");
        AFTPModAudio.PlayBossStinger();
        MusicPatches.LegacyActMusicPatches.SetBossStingerState();
    }
}