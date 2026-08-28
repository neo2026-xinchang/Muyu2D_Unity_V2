using UnityEngine;
using UnityEngine.UI;

public class RuntimeChineseFont : MonoBehaviour
{
    void Awake()
    {
        string[] names = {
            "Microsoft YaHei",
            "Microsoft YaHei UI",
            "SimHei",
            "PingFang SC",
            "Noto Sans CJK SC",
            "Noto Sans SC",
            "Arial Unicode MS"
        };

        Font font = Font.CreateDynamicFontFromOSFont(names, 48);
        if (font == null)
        {
            Debug.LogWarning("没有找到中文系统字体。");
            return;
        }

        foreach (Text text in GetComponentsInChildren<Text>(true))
            text.font = font;
    }
}
