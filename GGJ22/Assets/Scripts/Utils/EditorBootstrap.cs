#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class DefaultSceneLoader
{
    static DefaultSceneLoader()
    {
        EditorApplication.playModeStateChanged += LoadDefaultScene;
    }

    static void LoadDefaultScene(PlayModeStateChange state)
    {
        if (!EditorBootstrapMenuItems.ForceBootstrap())
            return;

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // if (EditorSceneManager.GetActiveScene().name != "Bootstrap")
                // EditorSceneManager.LoadScene("Bootstrap");
        }
    }
}

[InitializeOnLoad]
public class EditorBootstrapMenuItems : MonoBehaviour
{
    static EditorBootstrapMenuItems()
    {
        _forceBootstrap = EditorPrefs.GetBool(FORCE_BOOTSTRAP_MENU, false);

        EditorApplication.delayCall += () => {
            ToggleForceBootstrap(_forceBootstrap);
        };
    }

    public static bool ForceBootstrap() { return _forceBootstrap; }

    [MenuItem(FORCE_BOOTSTRAP_MENU)]
    private static void HandleForceBootstrapMenuItem()
    {
        ToggleForceBootstrap(!_forceBootstrap);
    }
    private static void ToggleForceBootstrap(bool state)
    {
        _forceBootstrap = state;
        Menu.SetChecked(FORCE_BOOTSTRAP_MENU, state);
        EditorPrefs.SetBool(FORCE_BOOTSTRAP_MENU, state);
    }

    private const string FORCE_BOOTSTRAP_MENU = "Custom Tools/Force Bootstrap Scene";
    private static bool _forceBootstrap = false;
}
#endif
