using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Used by previous ASL team for unknown reasons. Likely to print debug 
/// messages to screen for built applications that are difficult or annoying 
/// to retrieve and review otherwise. Not currently used in ASL but kept for 
/// archival purposes.
/// </summary>
public class GuiTextDebug : MonoBehaviour
{
    #region Fields
    #region Private Fields
    /// <summary>
    /// The distance of the window (from the top left corner of the screen...?)
    /// </summary>
    private float windowPosition = 240.0f;

    /// <summary>
    /// ?
    /// </summary>
    private int positionCheck = 2;

    /// <summary>
    /// The text being displayed in the GUI window.
    /// </summary>
    private static string windowText = "";

    /// <summary>
    /// The direction of scrolling currently being used to scroll through GUI 
    /// text.
    /// </summary>
    private Vector2 scrollViewVector = Vector2.zero;

    /// <summary>
    /// The style associated with the Debug GUI window.
    /// </summary>
    private GUIStyle debugBoxStyle;

    /// <summary>
    /// The relative position of the left side of the GUI debug window on 
    /// the screen.
    /// </summary>
    private float leftSide = 0.0f;

    /// <summary>
    /// The width of the Debug GUI window.
    /// </summary>
    private float debugWidth = 4200.0f;
    #endregion

    #region Public Fields
    /// <summary>
    /// Switch for determining whether to actually display the Debug GUI.
    /// </summary>
    public bool debugIsOn = true;
    #endregion
    #endregion

    #region Methods
    /// <summary>
    /// Sends a debug message to the GUI window.
    /// </summary>
    /// 
    /// <param name="newString">
    /// The string to display to the GUI window.
    /// </param>
    public static void debug(string newString)
    {
        windowText = newString + "\n" + windowText;
        UnityEngine.Debug.Log(newString);
    }

    /// <summary>
    /// Unity method that is called prior to runtime. Triggers before "Awake"
    /// methods and sets Debug box style items.
    /// </summary>
    void Start()
    {
        debugBoxStyle = new GUIStyle();
        debugBoxStyle.alignment = TextAnchor.UpperLeft;
        debugBoxStyle.fontSize = 67;
        leftSide = 2120; //Screen.width - debugWidth - 3;
    }

    /// <summary>
    /// Handles logic that should be called when the GUI gets updated every
    /// frame.
    /// </summary>
    void OnGUI()
    {

        if (debugIsOn)
        {

            GUI.depth = 0;

            GUI.BeginGroup(new Rect(windowPosition, 40.0f, leftSide, 2000.0f));

            scrollViewVector = GUI.BeginScrollView(new Rect(0, 0.0f, debugWidth, 2000.0f), scrollViewVector, new Rect(0.0f, 0.0f, 4000.0f, 2000.0f));
            GUI.Box(new Rect(0, 0.0f, debugWidth - 20.0f, 2000.0f), windowText, debugBoxStyle);
            GUI.EndScrollView();

            GUI.EndGroup();



            if (GUI.Button(new Rect(leftSide, 0.0f, 75.0f, 40.0f), "Debug"))
            {
                if (positionCheck == 1)
                {
                    windowPosition = 440.0f;
                    positionCheck = 2;
                }
                else
                {
                    windowPosition = leftSide;
                    positionCheck = 1;
                }
            }

            if (GUI.Button(new Rect(leftSide + 80f, 0.0f, 75.0f, 40.0f), "Clear"))
            {
                windowText = "";
            }
        }
    }
    #endregion
}
