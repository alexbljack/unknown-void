using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class UnitEditorWindow : OdinMenuEditorWindow
{
    [MenuItem("Content/Units")]
    static void OpenWindow()
    {
        GetWindow<UnitEditorWindow>().Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.AddAllAssetsAtPath("Enemies", "Assets/Prefabs", typeof(Enemy));
        tree.AddAllAssetsAtPath("Players", "Assets/Prefabs", typeof(Marine));
        return tree;
    }
}