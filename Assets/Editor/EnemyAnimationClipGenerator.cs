using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class EnemyAnimationClipGenerator
{
    // 원본 스프라이트 시트가 들어 있는 폴더
    private const string SourceFolder = "Assets/Enemy";

    // 생성된 Animation Clip이 저장될 폴더
    private const string OutputFolder = "Assets/Animation/Enemy";

    // 1초에 재생할 프레임 수
    private const float FrameRate = 8f;

    /*
     * 현재 프레임 배치:
     *
     * 0  1  2  3   = Down
     * 4  5  6  7   = Left
     * 8  9  10 11  = Right
     * 12 13 14 15  = Up
     *
     * Wave1에서 실제로 확인한 순서가 다르다면
     * 이 숫자 배열만 바꾸면 된다.
     */
    private static readonly DirectionFrames[] DirectionSettings =
    {
        new DirectionFrames("down",  new[] { 0, 1, 2, 3 }),
        new DirectionFrames("left",  new[] { 4, 5, 6, 7 }),
        new DirectionFrames("right", new[] { 8, 9, 10, 11 }),
        new DirectionFrames("up",    new[] { 12, 13, 14, 15 })
    };

    [MenuItem("Tools/Enemy/Create All Enemy Animation Clips")]
    private static void CreateAllEnemyAnimationClips()
    {
        EnsureFolderExists(OutputFolder);

        // Assets/Enemy 폴더 안의 모든 Texture2D 검색
        string[] textureGuids = AssetDatabase.FindAssets(
            "t:Texture2D",
            new[] { SourceFolder }
        );

        if (textureGuids.Length == 0)
        {
            Debug.LogWarning(
                $"이미지를 찾지 못했습니다. 폴더 경로를 확인하세요: {SourceFolder}"
            );

            return;
        }

        int successfulTextureCount = 0;
        int createdClipCount = 0;

        try
        {
            for (int i = 0; i < textureGuids.Length; i++)
            {
                string texturePath =
                    AssetDatabase.GUIDToAssetPath(textureGuids[i]);

                EditorUtility.DisplayProgressBar(
                    "Enemy Animation 생성 중",
                    texturePath,
                    (float)i / textureGuids.Length
                );

                if (CreateClipsForTexture(texturePath))
                {
                    successfulTextureCount++;
                    createdClipCount += DirectionSettings.Length;
                }
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"완료: 이미지 {successfulTextureCount}개에서 " +
            $"애니메이션 클립 {createdClipCount}개를 생성했습니다."
        );
    }

    private static bool CreateClipsForTexture(string texturePath)
    {
        // PNG 안에 분할돼 있는 Sprite들을 전부 불러온다.
        Sprite[] sprites = AssetDatabase
            .LoadAllAssetsAtPath(texturePath)
            .OfType<Sprite>()
            .OrderBy(sprite => GetSpriteIndex(sprite.name))
            .ToArray();

        if (sprites.Length != 16)
        {
            Debug.LogWarning(
                $"16개로 분할되지 않은 파일은 건너뜁니다: {texturePath} " +
                $"(현재 Sprite 개수: {sprites.Length})"
            );

            return false;
        }

        string textureName = Path.GetFileNameWithoutExtension(texturePath);

        foreach (DirectionFrames direction in DirectionSettings)
        {
            Sprite[] directionSprites = direction.FrameIndexes
                .Select(index => sprites[index])
                .ToArray();

            string clipName = $"{textureName}_{direction.Name}";
            string clipPath = $"{OutputFolder}/{clipName}.anim";

            CreateOrReplaceAnimationClip(
                clipName,
                clipPath,
                directionSprites
            );
        }

        return true;
    }

    private static void CreateOrReplaceAnimationClip(
        string clipName,
        string clipPath,
        Sprite[] sprites)
    {
        // 같은 이름의 파일이 있으면 새 내용으로 교체
        if (AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath) != null)
        {
            AssetDatabase.DeleteAsset(clipPath);
        }

        AnimationClip clip = new AnimationClip
        {
            name = clipName,
            frameRate = FrameRate
        };

        // SpriteRenderer의 Sprite 속성을 변경하는 애니메이션 바인딩
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keyframes =
            new ObjectReferenceKeyframe[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i / FrameRate,
                value = sprites[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(
            clip,
            spriteBinding,
            keyframes
        );

        // 걷기 애니메이션이므로 계속 반복하도록 설정
        SetLoopTime(clip, true);

        AssetDatabase.CreateAsset(clip, clipPath);
    }

    private static void SetLoopTime(AnimationClip clip, bool shouldLoop)
    {
        SerializedObject serializedClip = new SerializedObject(clip);

        SerializedProperty clipSettings =
            serializedClip.FindProperty("m_AnimationClipSettings");

        if (clipSettings == null)
        {
            Debug.LogWarning(
                $"{clip.name}의 반복 설정을 찾지 못했습니다."
            );

            return;
        }

        SerializedProperty loopTime =
            clipSettings.FindPropertyRelative("m_LoopTime");

        loopTime.boolValue = shouldLoop;
        serializedClip.ApplyModifiedProperties();
    }

    private static int GetSpriteIndex(string spriteName)
    {
        // Wave1_0, Wave1_1 ... Wave1_15의 마지막 번호 추출
        int underscoreIndex = spriteName.LastIndexOf('_');

        if (underscoreIndex < 0)
        {
            return int.MaxValue;
        }

        string indexText = spriteName.Substring(underscoreIndex + 1);

        return int.TryParse(indexText, out int index)
            ? index
            : int.MaxValue;
    }

    private static void EnsureFolderExists(string fullFolderPath)
    {
        string[] folders = fullFolderPath.Split('/');

        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = $"{currentPath}/{folders[i]}";

            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(currentPath, folders[i]);
            }

            currentPath = nextPath;
        }
    }

    private readonly struct DirectionFrames
    {
        public string Name { get; }
        public int[] FrameIndexes { get; }

        public DirectionFrames(string name, int[] frameIndexes)
        {
            Name = name;
            FrameIndexes = frameIndexes;
        }
    }
}