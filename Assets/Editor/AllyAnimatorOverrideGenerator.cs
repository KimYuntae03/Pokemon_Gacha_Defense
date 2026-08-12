using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class AllyAnimatorOverrideGenerator
{
    private const string AnimationFolder =
        "Assets/Animation/Ally";

    private const string OutputFolder =
        "Assets/Animation/Ally/Overrides";

    private const string BaseControllerPath =
        "Assets/Animation/Ally/AllyBaseAnimator.controller";


    [MenuItem("Tools/Ally/Create All Animator Overrides")]
    private static void CreateAllAnimatorOverrides()
    {
        EnsureFolderExists(OutputFolder);

        RuntimeAnimatorController baseController =
            AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(
                BaseControllerPath
            );

        if (baseController == null)
        {
            Debug.LogError(
                $"AllyBaseAnimator를 찾을 수 없습니다: {BaseControllerPath}"
            );

            return;
        }


        string[] allyNames =
        {
            "RIOLU",
            "RIOLU 1",
            "LUCARIO",
            "LUCARIO 1",
            "DRAGONITE",

            "COBALION",
            "VIRIZION",
            "TERRAKION",
            "KELDEO_1",

            "CELEBI",
            "VICTINI",

            "PALKIA",
            "DIALGA",

            "ARCEUS"
        };


        foreach (string allyName in allyNames)
        {
            CreateOverride(
                allyName,
                baseController
            );
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            "아군 Animator Override Controller 생성 완료"
        );
    }


    private static void CreateOverride(
        string allyName,
        RuntimeAnimatorController baseController
    )
    {
        AnimationClip down =
            LoadClip(allyName, "down");

        AnimationClip left =
            LoadClip(allyName, "left");

        AnimationClip right =
            LoadClip(allyName, "right");

        AnimationClip up =
            LoadClip(allyName, "up");


        if (
            down == null ||
            left == null ||
            right == null ||
            up == null
        )
        {
            Debug.LogWarning(
                $"{allyName}: 방향 애니메이션을 모두 찾지 못했습니다."
            );

            return;
        }


        string safeName =
            allyName.Replace(" ", "_");

        string outputPath =
            $"{OutputFolder}/{safeName}_Override.overrideController";


        AnimatorOverrideController overrideController =
            AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(
                outputPath
            );


        if (overrideController == null)
        {
            overrideController =
                new AnimatorOverrideController(baseController);

            AssetDatabase.CreateAsset(
                overrideController,
                outputPath
            );
        }
        else
        {
            overrideController.runtimeAnimatorController =
                baseController;
        }


        /*
         * AllyBaseAnimator에서 사용한 원본 클립을
         * 방향별 아군 클립으로 교체한다.
         *
         * Base Animator에 임시로 넣어둔 클립 이름이
         * 아래 이름과 일치해야 한다.
         */
        AnimationClip[] baseClips =
            baseController.animationClips;


        foreach (AnimationClip baseClip in baseClips)
        {
            string clipName =
                baseClip.name.ToLower();


            if (clipName.Contains("down"))
            {
                overrideController[baseClip] = down;
            }
            else if (clipName.Contains("left"))
            {
                overrideController[baseClip] = left;
            }
            else if (clipName.Contains("right"))
            {
                overrideController[baseClip] = right;
            }
            else if (clipName.Contains("up"))
            {
                overrideController[baseClip] = up;
            }
        }


        EditorUtility.SetDirty(
            overrideController
        );

        Debug.Log(
            $"{allyName} Override 생성 완료"
        );
    }


    private static AnimationClip LoadClip(
        string allyName,
        string direction
    )
    {
        string[] guids =
            AssetDatabase.FindAssets(
                $"{allyName}_{direction} t:AnimationClip",
                new[] { AnimationFolder }
            );


        foreach (string guid in guids)
        {
            string path =
                AssetDatabase.GUIDToAssetPath(guid);

            AnimationClip clip =
                AssetDatabase.LoadAssetAtPath<AnimationClip>(
                    path
                );

            if (
                clip != null &&
                clip.name.ToLower() ==
                $"{allyName}_{direction}".ToLower()
            )
            {
                return clip;
            }
        }


        return null;
    }


    private static void EnsureFolderExists(
        string folderPath
    )
    {
        if (AssetDatabase.IsValidFolder(folderPath))
        {
            return;
        }


        string parent =
            Path.GetDirectoryName(folderPath)
                .Replace("\\", "/");

        string folderName =
            Path.GetFileName(folderPath);


        EnsureFolderExists(parent);

        AssetDatabase.CreateFolder(
            parent,
            folderName
        );
    }
    [MenuItem("Tools/Ally/Assign Animator Overrides To AllyData")]
    private static void AssignOverridesToAllyData()
    {
        string allyDataFolder = "Assets/Data/Ally";

        string[] dataGuids =
            AssetDatabase.FindAssets(
                "t:AllyData",
                new[] { allyDataFolder }
            );

        int assignedCount = 0;

        foreach (string guid in dataGuids)
        {
            string dataPath =
                AssetDatabase.GUIDToAssetPath(guid);

            AllyData allyData =
                AssetDatabase.LoadAssetAtPath<AllyData>(
                    dataPath
                );

            if (allyData == null)
            {
                continue;
            }

            /*
            * AllyData의 allyName을 기준으로
            * Override Controller를 찾는다.
            *
            * 예:
            * RIOLU 1
            * → RIOLU_1_Override.overrideController
            */
            string safeName =
                allyData.allyName.Replace(" ", "_");

            string overridePath =
                $"{OutputFolder}/{safeName}_Override.overrideController";

            AnimatorOverrideController overrideController =
                AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(
                    overridePath
                );

            if (overrideController == null)
            {
                Debug.LogWarning(
                    $"{allyData.name}: Override Controller를 찾지 못했습니다.\n" +
                    $"찾은 경로: {overridePath}"
                );

                continue;
            }

            allyData.animatorOverrideController =
                overrideController;

            EditorUtility.SetDirty(allyData);

            assignedCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"AllyData Animator Override 연결 완료: {assignedCount}개"
        );
    }
}