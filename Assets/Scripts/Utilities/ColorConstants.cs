using UnityEngine;


[CreateAssetMenu(fileName = "ColorConstants", menuName = "Bronz/ColorConstants")]
public class ColorConstants : ScriptableSingleton<ColorConstants>
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Bronz/ColorConstants")]
    public static void ShowInEditor()
    {
        UnityEditor.Selection.activeObject = Instance;
    }
#endif

    [Header("UI components")] [Space(20)] [SerializeField] private Color uiLockedLevel = new Color(0, 0, 0, 1);
    public static Color UiLockedLevel { get { return Instance.uiLockedLevel; } }

    public static Color UiUnlockedLevel { get { return Instance.uiUnlockedLevel; } }
    [SerializeField] private Color uiUnlockedLevel = new Color(1, 1, 1, 1);

    public static Color UiCurrentLevel { get { return Instance.uiCurrentLevel; } }
    [SerializeField] private Color uiCurrentLevel = new Color(1, 1, 1, 1);

    public static Color RedColor { get { return Instance.redColor; } }
    [SerializeField] private Color redColor = new Color(1, 1, 1, 1);
}
