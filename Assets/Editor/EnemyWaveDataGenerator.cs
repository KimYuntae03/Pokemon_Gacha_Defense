using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class EnemyWaveDataGenerator
{
    // 총 웨이브 수
    private const int WaveCount = 51;

    // 기존 Enemy.controller 위치
    private const string BaseControllerPath =
        "Assets/Animation/Enemy/Enemy.controller";

    // 이미 만들어둔 적 애니메이션 클립들이 있는 폴더
    private const string AnimationFolder =
        "Assets/Animation/Enemy";

    // Override Controller를 저장할 폴더
    private const string OverrideFolder =
        "Assets/Animation/Enemy/Overrides";

    // EnemyData 저장 폴더
    private const string EnemyDataFolder =
        "Assets/Data/Enemies";

    // WaveData 저장 폴더
    private const string WaveDataFolder =
        "Assets/Data/Waves";


    [MenuItem("Tools/Enemy/Generate All Wave Data")]
    private static void GenerateAllWaveData()
    {
        // 필요한 폴더들이 없으면 자동 생성
        EnsureFolderExists(OverrideFolder);
        EnsureFolderExists(EnemyDataFolder);
        EnsureFolderExists(WaveDataFolder);

        // 공통 Enemy.controller 불러오기
        RuntimeAnimatorController baseController =
            AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                BaseControllerPath
            );

        if (baseController == null)
        {
            Debug.LogError(
                $"Enemy.controller를 찾을 수 없습니다.\n" +
                $"현재 경로: {BaseControllerPath}"
            );

            return;
        }


        /*
         * Assets/Animation/Enemy 안에 있는
         *
         * Wave1_left
         * Wave1_down
         * Wave1_right
         * Wave1_up
         *
         * 같은 클립들을 전부 검색한다.
         */
        Dictionary<string, AnimationClip> clipLookup =
            BuildAnimationClipLookup();


        int successCount = 0;


        for (int waveNumber = 1;
             waveNumber <= WaveCount;
             waveNumber++)
        {
            EditorUtility.DisplayProgressBar(
                "Enemy Wave 데이터 생성",
                $"Wave {waveNumber} 처리 중...",
                (float)waveNumber / WaveCount
            );


            // ------------------------------------
            // 1. 애니메이션 4개가 존재하는지 확인
            // ------------------------------------

            AnimationClip leftClip =
                GetWaveClip(
                    clipLookup,
                    waveNumber,
                    "left"
                );

            AnimationClip downClip =
                GetWaveClip(
                    clipLookup,
                    waveNumber,
                    "down"
                );

            AnimationClip rightClip =
                GetWaveClip(
                    clipLookup,
                    waveNumber,
                    "right"
                );

            AnimationClip upClip =
                GetWaveClip(
                    clipLookup,
                    waveNumber,
                    "up"
                );


            if (leftClip == null ||
                downClip == null ||
                rightClip == null ||
                upClip == null)
            {
                Debug.LogWarning(
                    $"Wave {waveNumber}: " +
                    $"4방향 애니메이션 중 일부가 없습니다. " +
                    $"이 Wave는 건너뜁니다."
                );

                continue;
            }


            // ------------------------------------
            // 2. 기존 WaveData가 있는지 확인
            // ------------------------------------

            string waveDataPath =
                $"{WaveDataFolder}/Wave{waveNumber:00}.asset";

            WaveData waveData =
                AssetDatabase.LoadAssetAtPath<WaveData>(
                    waveDataPath
                );


            bool isNewWaveData = false;


            if (waveData == null)
            {
                waveData =
                    ScriptableObject.CreateInstance<WaveData>();

                AssetDatabase.CreateAsset(
                    waveData,
                    waveDataPath
                );

                isNewWaveData = true;
            }


            // ------------------------------------
            // 3. EnemyData 준비
            // ------------------------------------

            EnemyData enemyData = waveData.enemyData;


            /*
             * Wave01 / Wave02처럼 이미
             *
             * Rattata
             * Zubat
             *
             * EnemyData가 연결되어 있다면
             * 기존 데이터를 그대로 사용한다.
             */
            if (enemyData == null)
            {
                string enemyDataPath =
                    $"{EnemyDataFolder}/Enemy{waveNumber:00}.asset";


                enemyData =
                    AssetDatabase.LoadAssetAtPath<EnemyData>(
                        enemyDataPath
                    );


                if (enemyData == null)
                {
                    enemyData =
                        ScriptableObject.CreateInstance<EnemyData>();


                    // 나중에 실제 포켓몬 이름으로 수정
                    enemyData.enemyName =
                        $"Enemy {waveNumber:00}";

                    // 기본값
                    enemyData.maxHealth = 100f;
                    enemyData.moveSpeed = 2f;


                    AssetDatabase.CreateAsset(
                        enemyData,
                        enemyDataPath
                    );
                }
            }


            // ------------------------------------
            // 4. Override Controller 준비
            // ------------------------------------

            AnimatorOverrideController overrideController;


            /*
             * 기존 Rattata / Zubat처럼
             * 이미 Override가 연결되어 있다면
             * 그걸 그대로 사용
             */
            if (enemyData.animatorOverrideController != null)
            {
                overrideController =
                    enemyData.animatorOverrideController;
            }
            else
            {
                string overridePath =
                    $"{OverrideFolder}/Wave{waveNumber:00}_Override.overrideController";


                overrideController =
                    AssetDatabase.LoadAssetAtPath
                    <AnimatorOverrideController>(
                        overridePath
                    );


                if (overrideController == null)
                {
                    overrideController =
                        new AnimatorOverrideController(
                            baseController
                        );


                    AssetDatabase.CreateAsset(
                        overrideController,
                        overridePath
                    );
                }
                else
                {
                    overrideController.runtimeAnimatorController =
                        baseController;
                }
            }


            // ------------------------------------
            // 5. 4방향 클립 Override
            // ------------------------------------

            ApplyDirectionalClips(
                overrideController,
                baseController,
                leftClip,
                downClip,
                rightClip,
                upClip
            );


            // ------------------------------------
            // 6. EnemyData에 Override 연결
            // ------------------------------------

            enemyData.animatorOverrideController =
                overrideController;


            EditorUtility.SetDirty(enemyData);


            // ------------------------------------
            // 7. WaveData와 EnemyData 연결
            // ------------------------------------

            waveData.waveNumber = waveNumber;

            waveData.enemyData = enemyData;


            /*
             * 새로 생성된 WaveData에만
             * 기본값을 넣음
             *
             * 기존 Wave01, Wave02 값은
             * 덮어쓰지 않음
             */
            if (isNewWaveData)
            {
                waveData.spawnCount = 20;
                waveData.spawnInterval = 1.5f;
                waveData.waveDuration = 30f;
            }


            EditorUtility.SetDirty(waveData);

            successCount++;
        }


        EditorUtility.ClearProgressBar();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();


        Debug.Log(
            $"Enemy Wave 데이터 생성 완료! " +
            $"{successCount}/{WaveCount} Wave 처리됨."
        );
    }


    /*
     * Enemy.controller 안에 있는
     * 기존 4방향 AnimationClip을 찾아서
     *
     * 이번 Wave의 클립으로 교체
     */
    private static void ApplyDirectionalClips(
        AnimatorOverrideController overrideController,
        RuntimeAnimatorController baseController,
        AnimationClip leftClip,
        AnimationClip downClip,
        AnimationClip rightClip,
        AnimationClip upClip)
    {
        AnimationClip[] baseClips =
            baseController.animationClips;


        AnimationClip baseLeft =
            FindDirectionClip(
                baseClips,
                "left"
            );

        AnimationClip baseDown =
            FindDirectionClip(
                baseClips,
                "down"
            );

        AnimationClip baseRight =
            FindDirectionClip(
                baseClips,
                "right"
            );

        AnimationClip baseUp =
            FindDirectionClip(
                baseClips,
                "up"
            );


        if (baseLeft == null ||
            baseDown == null ||
            baseRight == null ||
            baseUp == null)
        {
            Debug.LogError(
                "Enemy.controller에서 " +
                "left/down/right/up 클립을 찾을 수 없습니다."
            );

            return;
        }


        overrideController[baseLeft.name] =
            leftClip;

        overrideController[baseDown.name] =
            downClip;

        overrideController[baseRight.name] =
            rightClip;

        overrideController[baseUp.name] =
            upClip;


        EditorUtility.SetDirty(
            overrideController
        );
    }


    /*
     * Enemy.controller에 들어있는 클립들 중
     * _left
     * _down
     * _right
     * _up
     * 으로 끝나는 클립을 검색
     */
    private static AnimationClip FindDirectionClip(
        AnimationClip[] clips,
        string direction)
    {
        return clips.FirstOrDefault(
            clip =>
                clip != null &&
                clip.name.ToLower()
                    .EndsWith(
                        "_" + direction.ToLower()
                    )
        );
    }


    /*
     * Assets/Animation/Enemy 폴더에 있는
     * 모든 AnimationClip을 검색
     *
     * 예:
     *
     * Wave1_left
     * Wave1_down
     * Wave38(middle_boss)_left
     *
     * 같은 이름도 자동 인식
     */
    private static Dictionary<string, AnimationClip>
        BuildAnimationClipLookup()
    {
        Dictionary<string, AnimationClip> result =
            new Dictionary<string, AnimationClip>();


        string[] guids =
            AssetDatabase.FindAssets(
                "t:AnimationClip",
                new[]
                {
                    AnimationFolder
                }
            );


        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(
                    guid
                );


            AnimationClip clip =
                AssetDatabase.LoadAssetAtPath
                <AnimationClip>(
                    path
                );


            if (clip == null)
            {
                continue;
            }


            string fileName =
                Path.GetFileNameWithoutExtension(
                    path
                );

            Match match =
                Regex.Match(
                    fileName,
                    @"^Wave(\d+).*_(left|down|right|up)$",
                    RegexOptions.IgnoreCase
                );


            if (!match.Success)
            {
                continue;
            }


            int waveNumber =
                int.Parse(
                    match.Groups[1].Value
                );


            string direction =
                match.Groups[2]
                    .Value
                    .ToLower();


            string key =
                CreateClipKey(
                    waveNumber,
                    direction
                );


            result[key] = clip;
        }


        return result;
    }


    private static AnimationClip GetWaveClip(
        Dictionary<string, AnimationClip> lookup,
        int waveNumber,
        string direction)
    {
        string key =
            CreateClipKey(
                waveNumber,
                direction
            );


        lookup.TryGetValue(
            key,
            out AnimationClip clip
        );


        return clip;
    }


    private static string CreateClipKey(
        int waveNumber,
        string direction)
    {
        return $"{waveNumber}_{direction.ToLower()}";
    }


    
    //폴더가 존재하지 않으면 자동 생성
     
    private static void EnsureFolderExists(
        string fullFolderPath)
    {
        string[] folders =
            fullFolderPath.Split('/');


        string currentPath =
            folders[0];


        for (int i = 1;
             i < folders.Length;
             i++)
        {
            string nextPath =
                $"{currentPath}/{folders[i]}";


            if (!AssetDatabase.IsValidFolder(
                    nextPath))
            {
                AssetDatabase.CreateFolder(
                    currentPath,
                    folders[i]
                );
            }


            currentPath = nextPath;
        }
    }
}