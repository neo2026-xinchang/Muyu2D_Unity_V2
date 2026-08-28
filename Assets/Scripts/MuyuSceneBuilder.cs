#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class MuyuSceneBuilder
{
    [MenuItem("Muyu/Build V4 Stable Scene")]
    public static void Build()
    {
        Ensure("Scenes");
        Ensure("Audio");
        Ensure("Sprites");

        ImportMuyuSprite();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        GameObject canvasGO = new GameObject(
            "Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster),
            typeof(RuntimeChineseFont)
        );

        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        var bg = ImageGO("Background", canvasGO.transform, new Color(.95f,.92f,.84f,1));
        Stretch(bg.GetComponent<RectTransform>());

        Text title = TextGO("Title", canvasGO.transform, "电子木鱼", 70,
            new Vector2(0, 650), new Vector2(900, 120));
        title.color = new Color(.18f,.10f,.05f,1);

        // Real PNG woodfish
        GameObject muyu = new GameObject(
            "Muyu",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button),
            typeof(AudioSource),
            typeof(MuyuClick)
        );
        muyu.transform.SetParent(canvasGO.transform, false);

        RectTransform mrt = muyu.GetComponent<RectTransform>();
        mrt.sizeDelta = new Vector2(620, 620);
        mrt.anchoredPosition = new Vector2(0, 160);

        Image muyuImage = muyu.GetComponent<Image>();
        muyuImage.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/muyu.png");
        muyuImage.preserveAspect = true;
        muyuImage.color = Color.white;

        AudioSource audio = muyu.GetComponent<AudioSource>();
        audio.playOnAwake = false;

        Text count = TextGO("CountText", canvasGO.transform, "本轮功德：0 / 108", 48,
            new Vector2(0,-300), new Vector2(900,100));

        Text total = TextGO("TotalText", canvasGO.transform, "累计功德：0", 38,
            new Vector2(0,-390), new Vector2(900,90));

        Slider slider = CreateSlider(canvasGO.transform);
        RectTransform srt = slider.GetComponent<RectTransform>();
        srt.sizeDelta = new Vector2(760, 46);
        srt.anchoredPosition = new Vector2(0,-500);

        Text complete = TextGO("CompleteText", canvasGO.transform, "功德圆满", 58,
            new Vector2(0,520), new Vector2(900,100));
        complete.gameObject.SetActive(false);

        Text hint = TextGO("Hint", canvasGO.transform, "点击木鱼 · 一声一功德", 34,
            new Vector2(0,-610), new Vector2(900,80));
        hint.color = new Color(.34f,.24f,.15f,1);

        GameObject reset = ImageGO("ResetButton", canvasGO.transform,
            new Color(.80f,.74f,.64f,1));
        reset.AddComponent<Button>();
        RectTransform rrt = reset.GetComponent<RectTransform>();
        rrt.sizeDelta = new Vector2(300,86);
        rrt.anchoredPosition = new Vector2(0,-730);
        TextGO("Text", reset.transform, "清空记录", 32, Vector2.zero, new Vector2(280,80));

        MuyuClick click = muyu.GetComponent<MuyuClick>();
        click.countText = count;
        click.totalText = total;
        click.completeText = complete;
        click.progressSlider = slider;
        click.audioSource = audio;

        muyu.GetComponent<Button>().onClick.AddListener(click.ClickMuyu);
        reset.GetComponent<Button>().onClick.AddListener(click.ResetTotalCount);

        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene("Assets/Scenes/Main.unity");

        Debug.Log("V4 Stable Scene 已生成。");
    }

    static void ImportMuyuSprite()
    {
        AssetDatabase.ImportAsset("Assets/Sprites/muyu.png", ImportAssetOptions.ForceUpdate);

        TextureImporter importer =
            AssetImporter.GetAtPath("Assets/Sprites/muyu.png") as TextureImporter;

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
    }

    static void Ensure(string name)
    {
        string path = "Assets/" + name;
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets", name);
    }

    static Text TextGO(string name, Transform parent, string content,
        int fontSize, Vector2 pos, Vector2 size)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = size;
        rt.anchoredPosition = pos;

        Text text = go.GetComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(.18f,.12f,.08f,1);

        return text;
    }

    static GameObject ImageGO(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = color;
        return go;
    }

    static Slider CreateSlider(Transform parent)
    {
        GameObject root = new GameObject("ProgressSlider", typeof(RectTransform), typeof(Slider));
        root.transform.SetParent(parent, false);

        GameObject bg = ImageGO("Background", root.transform, new Color(.73f,.68f,.60f,1));
        Stretch(bg.GetComponent<RectTransform>());

        GameObject area = new GameObject("Fill Area", typeof(RectTransform));
        area.transform.SetParent(root.transform, false);
        Stretch(area.GetComponent<RectTransform>());

        GameObject fill = ImageGO("Fill", area.transform, new Color(.48f,.25f,.10f,1));
        Stretch(fill.GetComponent<RectTransform>());

        Slider s = root.GetComponent<Slider>();
        s.fillRect = fill.GetComponent<RectTransform>();
        s.minValue = 0;
        s.maxValue = 108;
        s.interactable = false;
        return s;
    }

    static void Stretch(RectTransform r)
    {
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
    }
}
#endif
