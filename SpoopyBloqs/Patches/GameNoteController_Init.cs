using Harmony;
using UnityEngine;
using System;

namespace SpoopyBloqs.Patches
{
    [HarmonyPatch(typeof(GameNoteController))]
    [HarmonyPatch("Init")]
    [HarmonyPatch(new Type[] { typeof(NoteData), typeof(Vector3), typeof(Vector3), typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float), typeof(bool), typeof(bool) })]
    internal class GameNoteController_Init
    {
        private static bool Prefix(GameNoteController __instance, NoteData noteData, Vector3 moveStartPos, Vector3 moveEndPos, Vector3 jumpEndPos,
            float moveDuration, float jumpDuration, float startTime, float jumpGravity, bool disappearingArrow, bool ghostNote,
            ref bool ____disappearingArrow, ref bool ____ghostNote, ref BoxCuttableBySaber ____bigCuttableBySaber, ref BoxCuttableBySaber ____smallCuttableBySaber)
        {
            var beatmapData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData;

            ____disappearingArrow = disappearingArrow;
            ____ghostNote = (noteData.id >= beatmapData.bombsCount + beatmapData.notesCount) ? true : ghostNote;
            ____bigCuttableBySaber.canBeCut = false;
            ____smallCuttableBySaber.canBeCut = false;

            ((NoteController)__instance).Init(noteData, moveStartPos, moveEndPos, jumpEndPos, moveDuration, jumpDuration, startTime, jumpGravity);
            return false;
        }
    }
}