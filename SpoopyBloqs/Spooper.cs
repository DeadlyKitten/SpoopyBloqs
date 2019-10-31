using System;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace SpoopyBloqs
{
    class Spooper : MonoBehaviour
    {
        private ScoreController _scoreController;
        private BeatmapObjectSpawnController _beatmapObjectSpawnController;
        private NoteCutSoundEffectManager _noteCutSoundEffectManager;

        private int notesCount;
        private int id;

        internal static Spooper Instance { get; private set; }

        internal static void Init()
        {
            if (!Instance) Instance = new GameObject().AddComponent<Spooper>();
            BS_Utils.Gameplay.ScoreSubmission.DisableSubmission("SpoopyBloqs");
        }

        private void Awake()
        {
            StartCoroutine(Setup());
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        internal void Cleanup()
        {
            try
            {
                _scoreController.noteWasMissedEvent -= OnNoteMiss;
                _scoreController.noteWasCutEvent -= OnNoteCut;
            }
            catch (Exception e)
            {
                Logger.Log("Error unsubscribing OnNoteMiss event.", Logger.LogLevel.Error);
                Logger.Log($"{e.Message}\n{e.StackTrace}", Logger.LogLevel.Error);
            }
        }

        private void OnNoteMiss(NoteData data, int c)
        {
            if (data.id < notesCount && data.noteType != NoteType.Bomb)
                Oops(data);
        }

        private void OnNoteCut(NoteData data, NoteCutInfo info, int c)
        {
            if (data.id < notesCount && data.noteType != NoteType.Bomb && !info.allIsOK)
                Oops(data);
        }

        private void Oops(NoteData data)
        {
            var newData = new NoteData(id++, data.time + _beatmapObjectSpawnController.spawnAheadTime, data.lineIndex, data.noteLineLayer,
                data.startNoteLineLayer, data.noteType, data.cutDirection, data.timeToNextBasicNote + _beatmapObjectSpawnController.spawnAheadTime,
                data.timeToPrevBasicNote + _beatmapObjectSpawnController.spawnAheadTime);

            _beatmapObjectSpawnController.BeatmapObjectSpawnCallback(newData);
            _noteCutSoundEffectManager.BeatmapObjectCallback(newData);
        }

        IEnumerator Setup()
        {
            yield return new WaitUntil(() => _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault());
            yield return new WaitUntil(() => _beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().FirstOrDefault());
            yield return new WaitUntil(() => _noteCutSoundEffectManager = Resources.FindObjectsOfTypeAll<NoteCutSoundEffectManager>().FirstOrDefault());

            _scoreController.noteWasCutEvent += OnNoteCut;
            _scoreController.noteWasMissedEvent += OnNoteMiss;
            var beatmapData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData;
            id = notesCount = beatmapData?.notesCount + beatmapData?.bombsCount ?? 10000;
        }
    }
}
