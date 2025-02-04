#if UNITY_EDITOR

using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEditor;
using UnityEditor.SceneManagement;

public class LevelTemplatePipeline : ISceneTemplatePipeline
{
    public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
    {
        return true;
    }

    public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
    {
        
    }

    public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName)
    {
        string path = EditorUtility.SaveFilePanel("Save Scene As", "Assets/Scenes/Levels", scene.name, "unity");

        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);
        EditorSceneManager.SaveScene(scene, path);
    }
}
#endif