# 2D 电子木鱼 Unity V2

推荐 Unity 2022.3 LTS。

## 使用方法

1. Unity Hub -> Open -> 选择 `Muyu2D_Unity_V2`
2. 等待资源导入
3. 如提示 TextMeshPro，导入 TMP Essential Resources
4. 顶部菜单：`Muyu -> Build V2 Demo Scene`
5. 打开：`Assets/Scenes/Main.unity`
6. 点击 Play

## V2 功能

- 点击木鱼
- 点击缩放动画
- AudioSource 音效接口
- 本轮 108 次进度
- 108 次完成提示
- 累计功德本地保存
- 清空记录按钮

## 加木鱼音效

把 `muyu.wav` 或 `muyu.mp3` 放到 `Assets/Audio/`。

然后：
- 选中 Hierarchy 里的 `Muyu`
- 找到 `Audio Source`
- 把音频拖到 `AudioClip`

## 本地保存

使用 Unity `PlayerPrefs` 保存累计功德数。
关闭游戏后再次打开仍然保留。

## 下一步

可以继续做：
- 真正的木鱼 Sprite
- 今日功德 / 历史功德
- 自动敲木鱼
- 背景雨声、钟声
- Android APK
