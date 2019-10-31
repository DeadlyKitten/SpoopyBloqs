using Harmony;
using System.Collections.Generic;
using UnityEngine;

namespace SpoopyBloqs.Patches
{
    [HarmonyPatch(typeof(NoteCutSoundEffectManager))]
    [HarmonyPatch("BeatmapObjectCallback")]
    internal class NoteCutSoundManager_BeatmapObjectCallback
    {
        private static bool Prefix(NoteCutSoundEffectManager __instance, BeatmapObjectData beatmapObjectData, ref AudioTimeSyncController ____audioTimeSyncController,
            ref float ____prevNoteATime, ref float ____prevNoteBTime, ref NoteCutSoundEffect ____prevNoteASoundEffect, ref NoteCutSoundEffect ____prevNoteBSoundEffect,
            ref NoteCutSoundEffect.Pool ____noteCutSoundEffectPool, ref PlayerController ____playerController, ref float ____beatAlignOffset,
            ref RandomObjectPicker<AudioClip> ____randomShortCutSoundPicker, ref RandomObjectPicker<AudioClip> ____randomLongCutSoundPicker,
            ref BeatmapObjectSpawnController ____beatmapObjectSpawnController)
        {
            if (beatmapObjectData.beatmapObjectType != BeatmapObjectType.Note)
            {
                return false;
            }
            NoteData noteData = (NoteData)beatmapObjectData;
            float timeScale = ____audioTimeSyncController.timeScale;
            if (noteData.noteType != NoteType.NoteA && noteData.noteType != NoteType.NoteB)
            {
                return false;
            }
            if ((noteData.noteType == NoteType.NoteA && ((noteData.time > ____prevNoteATime) && noteData.time < ____prevNoteATime + 0.001f)) || (noteData.noteType == NoteType.NoteB && ((noteData.time > ____prevNoteBTime) && noteData.time < ____prevNoteBTime + 0.001f)))
            {
                return false;
            }
            bool flag = false;
            if (noteData.time < ____prevNoteATime + 0.001f || noteData.time < ____prevNoteBTime + 0.001f)
            {
                if (noteData.noteType == NoteType.NoteA && ____prevNoteBSoundEffect.enabled)
                {
                    ____prevNoteBSoundEffect.volumeMultiplier = 0.9f;
                    flag = true;
                }
                else if (noteData.noteType == NoteType.NoteB && ____prevNoteASoundEffect.enabled)
                {
                    ____prevNoteASoundEffect.volumeMultiplier = 0.9f;
                    flag = true;
                }
            }
            NoteCutSoundEffect noteCutSoundEffect = ____noteCutSoundEffectPool.Spawn();
            noteCutSoundEffect.transform.SetPositionAndRotation(__instance.transform.localPosition, Quaternion.identity);
            noteCutSoundEffect.didFinishEvent += __instance.HandleCutSoundEffectDidFinish;
            Saber saber = null;
            if (noteData.noteType == NoteType.NoteA)
            {
                ____prevNoteATime = noteData.time;
                saber = ____playerController.leftSaber;
                ____prevNoteASoundEffect = noteCutSoundEffect;
            }
            else if (noteData.noteType == NoteType.NoteB)
            {
                ____prevNoteBTime = noteData.time;
                saber = ____playerController.rightSaber;
                ____prevNoteBSoundEffect = noteCutSoundEffect;
            }
            if (noteData.noteType == NoteType.NoteA || noteData.noteType == NoteType.NoteB)
            {
                bool flag2 = noteData.timeToPrevBasicNote < ____beatAlignOffset;
                AudioClip audioClip = flag2 ? ____randomShortCutSoundPicker.PickRandomObject() : ____randomLongCutSoundPicker.PickRandomObject();
                float volumeMultiplier = 1f;
                if (flag)
                {
                    volumeMultiplier = 0.9f;
                }
                else if (flag2)
                {
                    volumeMultiplier = 0.9f;
                }
                noteCutSoundEffect.Init(audioClip, (double)(noteData.time / timeScale) + ____audioTimeSyncController.dspTimeOffset, ____beatAlignOffset, ____beatmapObjectSpawnController.missedTimeOffset, noteData.timeToPrevBasicNote / timeScale, noteData.timeToNextBasicNote / timeScale, saber, noteData, __instance.handleWrongSaberTypeAsGood, volumeMultiplier, __instance.useTestAudioClip);
            }
            HashSet<NoteCutSoundEffect> activeItems = ____noteCutSoundEffectPool.activeItems;
            NoteCutSoundEffect noteCutSoundEffect2 = null;
            float num = float.MaxValue;
            foreach (NoteCutSoundEffect noteCutSoundEffect3 in activeItems)
            {
                if (noteCutSoundEffect3.noteData.time < num)
                {
                    num = noteCutSoundEffect3.noteData.time;
                    noteCutSoundEffect2 = noteCutSoundEffect3;
                }
            }
            if (activeItems.Count > 64)
            {
                noteCutSoundEffect2.StopPlayingAndFinish();
            }
            return false;
        }
    }
}
