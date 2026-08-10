using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AllyAnimationClipGenerator
{
    // 아군 스프라이트 시트가 들어 있는 폴더
    private const string SourceFolder = "Assets/Ally";

    // 생성된 아군 애니메이션 클립을 저장할 폴더
    private const string OutputFolder = "Assets/Animation/Ally";

    // 초당 재생할 프레임 수
    private const float FrameRate = 8f;

    /*
     * 4 x 4 스프라이트 시트의 방향별 프레임 순서
     *
     * 0 ~ 3   : Down
     * 4 ~ 7   : Left
     * 8 ~ 11  : Right
     * 12 ~ 15 : Up
     *
     * 적 스프라이트 시트와 아군 시트의 방향 배치가 같다는 전제다.
     */
    private static readonly DirectionFrames[] DirectionSettings =
    {
        new DirectionFrames("down",  new[] { 0, 1, 2, 3 }),
        new DirectionFrames("left",  new[] { 4, 5, 6, 7 }),
        new DirectionFrames("right", new[] { 8, 9, 10, 11 }),
        new DirectionFrames("up",    new[] { 12, 13, 14, 15 })
    };

    [MenuItem("Tools/Ally/Create All Ally Animation Clips")]
    private static void CreateAllAllyAnimationClips()
    {
        EnsureFolderExists(OutputFolder);

        string[] textureGuids = AssetDatabase.FindAssets(
            "t:Texture2D",
            new[] { SourceFolder }
        );

        if (textureGuids.Length == 0)
        {
            Debug.LogWarning(
                $"아군 스프라이트 이미지를 찾지 못했습니다. 경로 확인: {SourceFolder}"
            );

            return;
        }

        int processedImageCount = 0;
        int createdClipCount = 0;

        try
        {
            for (int i = 0; i < textureGuids.Length; i++)
            {
                string texturePath =
                    AssetDatabase.GUIDToAssetPath(textureGuids[i]);

                EditorUtility.DisplayProgressBar(
                    "아군 애니메이션 생성 중",
                    texturePath,
                    (float)i / textureGuids.Length
                );

                if (CreateClipsForTexture(texturePath))
                {
                    processedImageCount++;
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
            $"아군 이미지 {processedImageCount}개 처리 완료. " +
            $"애니메이션 클립 {createdClipCount}개를 생성했습니다."
        );
    }

    private static bool CreateClipsForTexture(string texturePath)
    {
        Sprite[] sprites = AssetDatabase
            .LoadAllAssetsAtPath(texturePath)
            .OfType<Sprite>()
            .OrderBy(sprite => GetSpriteIndex(sprite.name))
            .ToArray();

        if (sprites.Length != 16)
        {
            Debug.LogWarning(
                $"스프라이트가 16개가 아니므로 건너뜁니다: {texturePath} " +
                $"현재 개수: {sprites.Length}"
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

            CreateOrReplaceClip(
                clipName,
                clipPath,
                directionSprites
            );
        }

        return true;
    }

    private static void CreateOrReplaceClip(
        string clipName,
        string clipPath,
        Sprite[] sprites)
    {
        AnimationClip existingClip =
            AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);

        if (existingClip != null)
        {
            AssetDatabase.DeleteAsset(clipPath);
        }

        AnimationClip clip = new AnimationClip
        {
            name = clipName,
            frameRate = FrameRate
        };

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

        SetLoopTime(clip, true);

        AssetDatabase.CreateAsset(clip, clipPath);
    }

    private static void SetLoopTime(
        AnimationClip clip,
        bool shouldLoop)
    {
        SerializedObject serializedClip =
            new SerializedObject(clip);

        SerializedProperty clipSettings =
            serializedClip.FindProperty("m_AnimationClipSettings");

        if (clipSettings == null)
        {
            Debug.LogWarning(
                $"{clip.name}의 Loop Time 설정을 찾지 못했습니다."
            );

            return;
        }

        SerializedProperty loopTime =
            clipSettings.FindPropertyRelative("m_LoopTime");

        if (loopTime == null)
        {
            Debug.LogWarning(
                $"{clip.name}의 m_LoopTime 속성을 찾지 못했습니다."
            );

            return;
        }

        loopTime.boolValue = shouldLoop;
        serializedClip.ApplyModifiedProperties();
    }

    private static int GetSpriteIndex(string spriteName)
    {
        /*
         * 예:
         * CHARMANDER_0
         * CHARMANDER_1
         * CHARMANDER_15
         *
         * 마지막 밑줄 뒤의 번호를 가져온다.
         */
        int underscoreIndex = spriteName.LastIndexOf('_');

        if (underscoreIndex < 0)
        {
            return int.MaxValue;
        }

        string indexText =
            spriteName.Substring(underscoreIndex + 1);

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
            string nextPath =
                $"{currentPath}/{folders[i]}";

            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(
                    currentPath,
                    folders[i]
                );
            }

            currentPath = nextPath;
        }
    }

    private readonly struct DirectionFrames
    {
        public string Name { get; }
        public int[] FrameIndexes { get; }

        public DirectionFrames(
            string name,
            int[] frameIndexes)
        {
            Name = name;
            FrameIndexes = frameIndexes;
        }
    }
}