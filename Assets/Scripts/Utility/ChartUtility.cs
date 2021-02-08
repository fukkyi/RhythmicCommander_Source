using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;
using NCMB;

namespace Chart {

    public class ChartUtility {

        [SerializeField]
        public MusicStruct wirteMusic;

        [SerializeField]
        public MusicStruct setMusic;

        public string loadMusic;

        public static MusicStruct LoadMusicData(string musicName)
        {
            string json = ReadAllChartJson(musicName);

            return JsonUtility.FromJson<MusicStruct>(json);
        }

        public static string ReadAllChartJson(string name)
        {
            string path = Application.streamingAssetsPath + Constant.musicDataDirectory + "/" + name + "/" + name + Constant.chartExtension;

            return File.ReadAllText(path);
        }

        public static string GetChartJson(MusicStruct musicStruct)
        {
            return JsonUtility.ToJson(musicStruct);
        }

        public static void WirteChart(string json, string name)
        {
            string filePath = Application.streamingAssetsPath + Constant.musicDataDirectory + "/" + name;

            if (!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }

            File.WriteAllText(filePath + "/" + name + Constant.chartExtension, json);
        }

        public static void WirteMusicStructData(MusicStruct musicStruct, string sourceMusicPath)
        {
            string json = GetChartJson(musicStruct);
            string chartName = musicStruct.musicName;
            string filePath = Application.streamingAssetsPath + Constant.musicDataDirectory + "/" + chartName;

            if (!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }

            string musicFilePath = filePath + "/" + musicStruct.musicFileName + Constant.musicExtension;

            if (sourceMusicPath != string.Empty) {

                if (!File.Exists(musicFilePath)) {
                    File.Copy(sourceMusicPath, musicFilePath);
                }
            }

            string scoreDataJson = JsonUtility.ToJson(new ScoreDataStruct());
            string dataFilePath = filePath + "/" + chartName;

            File.WriteAllText(dataFilePath + Constant.scoreExtension, scoreDataJson);
            File.WriteAllText(dataFilePath + Constant.chartExtension, json);
        }

        public static ScoreDataStruct LoadScoreData(string musicName)
        {
            ScoreDataStruct scoreData = null;

            if (Constant.CompareEnv(GameEnvironment.local)) {
                string json = ReadScoreDataJson(musicName);

                if (json == string.Empty) return null;

                scoreData = JsonUtility.FromJson<ScoreDataStruct>(json);
            }
            else if (Constant.CompareEnv(GameEnvironment.webGL)) {

                scoreData = FindCacheStruct(musicName).scoreStruct;
            }

            return scoreData;
        }

        public static string ReadScoreDataJson(string name)
        {
            string path = Application.streamingAssetsPath + Constant.musicDataDirectory + "/" + name + "/" + name + Constant.scoreExtension;
            string json = string.Empty;

            if (File.Exists(path)) {
                json = File.ReadAllText(path);
            }

            return json;
        }

        /// <summary>
        /// 譜面フォルダの名前を全て取得する
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllFileNames() 
        {
            string[] musicFolderPaths = Directory.GetDirectories(Application.streamingAssetsPath + Constant.musicDataDirectory);
            string[] musicFileNames = new string[musicFolderPaths.Length];

            for (int i = 0; i < musicFileNames.Length; i++) {

                musicFileNames[i] = Path.GetFileName(musicFolderPaths[i]);
            }

            return musicFileNames;
        }

        /// <summary>
        /// スコアデータを更新する
        /// </summary>
        /// <param name="musicName"></param>
        /// <param name="score"></param>
        /// <param name="difficulty"></param>
        /// <param name="clearType"></param>
        /// <returns></returns>
        public static IEnumerator WriteScoreDataFromResult(string musicName, int score, Difficulty difficulty, ClearType clearType)
        {
            int chartElement = (int)difficulty;
            ScoreDataStruct scoreData = null;

            if (Constant.CompareEnv(GameEnvironment.local)) {

                scoreData = LoadScoreData(musicName);
            }
            else if (Constant.CompareEnv(GameEnvironment.webGL)) {

                bool isFinish = false;
                NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("MusicData");
                query.WhereEqualTo("musicName", musicName);
                query.Limit = 1;
                query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
                {
                    if (e != null) {
                        isFinish = true;
                        Debug.LogError(e);
                    }
                    else {
                        string scoreJson = Convert.ToString(objList[0]["scoreData"]);
                        scoreData = JsonUtility.FromJson<ScoreDataStruct>(scoreJson);
                        isFinish = true;
                    }
                });
                // 取得が完了するまで待機
                while(!isFinish) { yield return null; }
            }

            if (scoreData == null) yield break;

            ScoreStruct scoreStruct = scoreData.scores[chartElement];

            if (scoreData == null) scoreData = new ScoreDataStruct();

            if (scoreStruct.bestScore < score) {

                scoreData.scores[chartElement].bestScore = score;
            }
            if (scoreStruct.clearType < clearType) {

                scoreData.scores[chartElement].clearType = clearType;
            }
            if (clearType > ClearType.Failed) {
                scoreData.scores[chartElement].clearCount++;
            }

            scoreData.scores[chartElement].playCount++;
            RankingManage(ref scoreData.scores[chartElement], score, clearType);

            string scoreDataJson = JsonUtility.ToJson(scoreData);

            if (Constant.CompareEnv(GameEnvironment.local)) {

                string filePath = Application.streamingAssetsPath + Constant.musicDataDirectory + "/" + musicName;

                if (!Directory.Exists(filePath)) yield break;

                string dataFilePath = filePath + "/" + musicName + Constant.scoreExtension;

                File.WriteAllText(dataFilePath, scoreDataJson);
            }
            else if (Constant.CompareEnv(GameEnvironment.webGL)) {

                bool isFinish = false;
                NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("MusicData");
                query.WhereEqualTo("musicName", musicName);
                query.Limit = 1;
                query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
                {
                    if (e != null) {
                        isFinish = true;
                        Debug.LogError(e);
                    }
                    else {
                        objList[0]["scoreData"] = scoreDataJson;
                        objList[0].SaveAsync((NCMBException exp) =>
                        {
                            if (exp != null) {
                                isFinish = true;
                                Debug.LogError(e);
                            }
                            else {
                                isFinish = true;
                            }
                        });                        
                    }
                });
                // 取得が完了するまで待機
                while (!isFinish) { yield return null; }
                SetScoreCacheStruct(musicName, scoreData);
            }
        }

        public static int GetBestScore(string musicName, Difficulty difficulty)
        {
            int chartElement = (int)difficulty;
            ScoreDataStruct scoreData = LoadScoreData(musicName);

            return scoreData.scores[chartElement].bestScore;
        }

        public static IEnumerator LoadMusicDataFromNCMB()
        {
            string[] musicStructs = new string[0];
            string[] scoreStructs = new string[0];
            string[] musicNames = new string[0];

            float timeOutLength = 10.0f;
            bool isFinish = false;

            NCMBQuery<NCMBObject> query = new NCMBQuery<NCMBObject>("MusicData");
            query.WhereEqualTo("isActive", true);
            query.FindAsync((List<NCMBObject> objList, NCMBException e) =>
            {
                if (e != null) {
                    Debug.LogError(e);
                }
                else {

                    NCMBObject[] objs = objList.ToArray();
                    Array.Resize(ref musicStructs, objs.Length);
                    Array.Resize(ref scoreStructs, objs.Length);
                    Array.Resize(ref musicNames, objs.Length);

                    for (int i = 0; i < objs.Length; i++) {
                        musicStructs[i] = Convert.ToString(objs[i]["musicJson"]);
                        scoreStructs[i] = Convert.ToString(objs[i]["scoreData"]);
                        musicNames[i] = Convert.ToString(objs[i]["musicName"]);
                    }
                }

                isFinish = true;
            });

            float progressTime = 0;
            while(progressTime < timeOutLength && !isFinish) {

                progressTime += Time.deltaTime;
                yield return null;
            }

            if (musicStructs.Length == 0) yield break;

            Array.Resize(ref StaticValue.cacheStructs, musicStructs.Length);
            for (int i = 0; i < musicStructs.Length; i++) {
                StaticValue.cacheStructs[i] = new CacheStruct();
                StaticValue.cacheStructs[i].musicName = musicNames[i];
                StaticValue.cacheStructs[i].musicStruct = JsonUtility.FromJson<MusicStruct>(musicStructs[i]);
                StaticValue.cacheStructs[i].scoreStruct = JsonUtility.FromJson<ScoreDataStruct>(scoreStructs[i]);
            }
        }

        public static CacheStruct FindCacheStruct(string musicName)
        {
            if (StaticValue.cacheStructs == null) return null;

            CacheStruct result = null;
            foreach(CacheStruct cache in StaticValue.cacheStructs) {

                if (cache.musicName == musicName) {

                    result = new CacheStruct();
                    result.musicName = cache.musicName;
                    result.musicStruct = cache.musicStruct;
                    result.scoreStruct = cache.scoreStruct;

                    break;
                }
            }

            return result;
        }

        public static void SetScoreCacheStruct(string musicName, ScoreDataStruct scoreData)
        {
            if (StaticValue.cacheStructs == null) return;

            for (int i = 0; i < StaticValue.cacheStructs.Length; i++) {

                if (StaticValue.cacheStructs[i].musicName == musicName) {

                    StaticValue.cacheStructs[i].scoreStruct.scores = scoreData.scores;
                    break;
                }
            }
        }

        public static void RankingManage(ref ScoreStruct scoreData, int score, ClearType clear)
        {
            if (scoreData.scoreRanking == null) {

                scoreData.scoreRanking = new RankingStruct[1];
                scoreData.scoreRanking[0] = new RankingStruct(StaticValue.playerName, score, clear);

                return;
            }

            int rankingCount = scoreData.scoreRanking.Length;

            if (rankingCount < Constant.rankingLength) {

                Array.Resize(ref scoreData.scoreRanking, rankingCount + 1);

                RankingStruct rankingData = new RankingStruct(StaticValue.playerName, score, clear);
                scoreData.scoreRanking[rankingCount] = rankingData;
            }
            else if (scoreData.scoreRanking[rankingCount - 1].score < score) {

                RankingStruct rankingData = new RankingStruct(StaticValue.playerName, score, clear);
                scoreData.scoreRanking[rankingCount - 1] = rankingData;
            }

            Array.Sort(scoreData.scoreRanking, (x, y) => y.score - x.score);
        }
    }
}
