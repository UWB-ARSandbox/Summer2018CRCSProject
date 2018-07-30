/*==== DebugConsole.cs ====================================================
 * Class for handling multi-line, multi-color debugging messages.
 * Original Author: Jeremy Hollingsworth
 * Based On: Version 1.2.1 Mar 02, 2006
 * 
 * Modified: Simon Waite
 * Date: 22 Feb 2007
 *
 * Modified: Shinsuke Sugita
 * Date: 1 Dec 2015
 * 
 * Modification to original script to allow pixel-correct line spacing
 *
 * Setting the boolean pixelCorrect changes the units in lineSpacing property
 * to pixels, so you have a pixel correct gui font in your console.
 *
 * It also checks every frame if the screen is resized to make sure the 
 * line spacing is correct (To see this; drag and let go in the editor 
 * and the text spacing will snap back)
 *
 * USAGE:
 * ::Drop in your standard assets folder (if you want to change any of the
 * default settings in the inspector, create an empty GameObject and attach
 * this script to it from you standard assets folder.  That will provide
 * access to the default settings in the inspector)
 * 
 * ::To use, call DebugConsole.functionOrProperty() where 
 * functionOrProperty = one of the following:
 * 
 * -Log(string message, string color)  Adds "message" to the list with the
 * "color" color. Color is optional and can be any of the following: "error",
 * "warning", or "normal".  Default is normal.
 * 
 * Clear() Clears all messages
 * 
 * isVisible (true,false)  Toggles the visibility of the output.  Does _not_
 * clear the messages.
 * 
 * isDraggable (true, false)  Toggles mouse drag functionality
 * =========================================================================*/


using UnityEngine;
using System.Collections;

/// <summary>
/// NOTE: Utilized by previous ASL team for unknown means. Presumably allows 
/// for a GUI debugging console that is extremely useful for interpreting debug 
/// messages during runtime that are otherwise invisible to the user. Kept for 
/// archival purposes.
/// </summary>
public class DebugConsole : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The GUI that will be duplicated.
    /// </summary>
    public GameObject DebugGui = null;

    /// <summary>
    /// The default, relative position of the GUI on-screen.
    /// </summary>
    public Vector3 defaultGuiPosition = new Vector3(0.01F, 0.98F, 0F);

    /// <summary>
    /// The default size of the GUI.
    /// </summary>
    public Vector3 defaultGuiScale = new Vector3(0.5F, 0.5F, 1F);

    /// <summary>
    /// The color associated with normal GUI states.
    /// </summary>
    public Color normal = Color.green;

    /// <summary>
    /// The color associated with application warning states.
    /// </summary>
    public Color warning = Color.yellow;

    /// <summary>
    /// The color associated with application error states.
    /// </summary>
    public Color error = Color.red;

    /// <summary>
    /// The maximum number of messages that can be displayed at once.
    /// </summary>
    public int maxMessages = 30;                   // The max number of messages displayed

    /// <summary>
    /// The vertical spacing between lines of output text.
    /// </summary>
    public float lineSpacing = 0.02F;              // The amount of space between lines

    /// <summary>
    /// Local storage of GUI messages.
    /// </summary>
    public ArrayList messages = new ArrayList();

    /// <summary>
    /// Local storage of...GUIs?
    /// </summary>
    public ArrayList guis = new ArrayList();

    /// <summary>
    /// Local storage of...colors?
    /// </summary>
    public ArrayList colors = new ArrayList();

    /// <summary>
    /// "Can the output be dragged around at runtime by default?
    /// </summary>
    public bool draggable = true;

    /// <summary>
    /// Does output show on screen by default or do we have to enable it with 
    /// code?
    /// </summary>
    public bool visible = true;

    /// <summary>
    /// Set to be pixel correct linespacing.
    /// </summary>
    public bool pixelCorrect = false;
    #endregion

    #region Properties
    public static bool isVisible
    {
        get
        {
            return DebugConsole.instance.visible;
        }

        set
        {
            DebugConsole.instance.visible = value;
            if (value == true)
            {
                DebugConsole.instance.Display();
            }
            else if (value == false)
            {
                DebugConsole.instance.ClearScreen();
            }
        }
    }

    public static bool isDraggable
    {
        get
        {
            return DebugConsole.instance.draggable;
        }

        set
        {
            DebugConsole.instance.draggable = value;

        }
    }


    private static DebugConsole s_Instance = null;   // Our instance to allow this script to be called without a direct connection.
    public static DebugConsole instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                if (s_Instance == null)
                {
                    GameObject console = new GameObject();
                    console.AddComponent<DebugConsole>();
                    console.name = "DebugConsoleController";
                    s_Instance = FindObjectOfType(typeof(DebugConsole)) as DebugConsole;
                    DebugConsole.instance.InitGuis();
                }

            }

            return s_Instance;
        }
    }
    #endregion

    #region Methods...mostly...
    /// <summary>
    /// Triggers after all "Start" methods are successfully called or when this
    /// class is enabled.
    /// </summary>
    void Awake()
    {
        s_Instance = this;
        InitGuis();
    }
    
    /// <summary>
    /// Boolean to track whether the GUIs were initialized or not.
    /// </summary>
    protected bool guisCreated = false;
    /// <summary>
    /// The depth of the GUI screen in comparison to other camera elements.
    /// (Probably)
    /// </summary>
    protected float screenHeight = -1;
    /// <summary>
    /// Handles creation and initialization of GUI elements.
    /// </summary>
    public void InitGuis()
    {
        float usedLineSpacing = lineSpacing;
        screenHeight = Screen.height;
        if (pixelCorrect)
            usedLineSpacing = 1.0F / screenHeight * usedLineSpacing;

        if (guisCreated == false)
        {
            if (DebugGui == null)  // If an external GUIText is not set, provide the default GUIText
            {
                DebugGui = new GameObject();
                DebugGui.AddComponent<GUIText>();
                DebugGui.name = "DebugGUI(0)";
                DebugGui.transform.position = defaultGuiPosition;
                DebugGui.transform.localScale = defaultGuiScale;
            }

            // Create our GUI objects to our maxMessages count
            Vector3 position = DebugGui.transform.position;
            guis.Add(DebugGui);
            int x = 1;

            while (x < maxMessages)
            {
                position.y -= usedLineSpacing;
                GameObject clone = null;
                clone = (GameObject)Instantiate(DebugGui, position, transform.rotation);
                clone.name = string.Format("DebugGUI({0})", x);
                guis.Add(clone);
                position = clone.transform.position;
                x += 1;
            }

            x = 0;
            while (x < guis.Count)
            {
                GameObject temp = (GameObject)guis[x];
                temp.transform.parent = DebugGui.transform;
                x++;
            }
            guisCreated = true;
        }
        else
        {
            // we're called on a screensize change, so fiddle with sizes
            Vector3 position = DebugGui.transform.position;
            for (int x = 0; x < guis.Count; x++)
            {
                position.y -= usedLineSpacing;
                GameObject temp = (GameObject)guis[x];
                temp.transform.position = position;
            }
        }
    }


    /// <summary>
    /// Tracks whether the GUI is currently connected to a mouse pointer.
    /// </summary>
    bool connectedToMouse = false;
    /// <summary>
    /// Unity method that is called every frame. Locks the GUI position to the
    /// mouse cursor position.
    /// </summary>
    void Update()
    {
        // If we are visible and the screenHeight has changed, reset linespacing
        if (visible == true && screenHeight != Screen.height)
        {
            InitGuis();
        }
        if (draggable == true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (connectedToMouse == false && DebugGui.GetComponent<GUIText>().HitTest((Vector3)Input.mousePosition) == true)
                {
                    connectedToMouse = true;
                }
                else if (connectedToMouse == true)
                {
                    connectedToMouse = false;
                }

            }

            if (connectedToMouse == true)
            {
                float posX = DebugGui.transform.position.x;
                float posY = DebugGui.transform.position.y;
                posX = Input.mousePosition.x / Screen.width;
                posY = Input.mousePosition.y / Screen.height;
                DebugGui.transform.position = new Vector3(posX, posY, 0F);
            }
        }

    }
    #region Interface Functions
    /// <summary>
    /// Logs a debug message to the GUI.
    /// </summary>
    /// 
    /// <param name="message">
    /// The message to be logged and displayed.
    /// </param>
    /// <param name="color">
    /// The color to display the message in.
    /// </param>
    public static void Log(string message, string color)
    {
        DebugConsole.instance.AddMessage(message, color);

    }

    /// <summary>
    /// Logs a debug message in a default color.
    /// </summary>
    /// 
    /// <param name="message">
    /// The message to be logged and displayed.
    /// </param>
    public static void Log(string message)
    {
        DebugConsole.instance.AddMessage(message);
    }

    /// <summary>
    /// Clears all messages logged on the GUI.
    /// </summary>
    public static void Clear()
    {
        DebugConsole.instance.ClearMessages();
    }
    #endregion


    //---------- void AddMesage(string message, string color) ------
    //Adds a mesage to the list
    //--------------------------------------------------------------

    /// <summary>
    /// Helper method that handles adding a message to be logged to the GUI
    /// console.
    /// </summary>
    /// 
    /// <param name="message">
    /// Message to be logged and displayed.
    /// </param>
    /// <param name="color">
    /// Color to display the message in.
    /// </param>
    public void AddMessage(string message, string color)
    {
        messages.Add(message);
        colors.Add(color);
        Display();
    }
    /// <summary>
    /// Helper method that handles adding a message to be logged to the GUI
    /// console in a default color.
    /// </summary>
    /// 
    /// <param name="message">
    /// Message to be logged and displayed.
    /// </param>
    public void AddMessage(string message)
    {
        messages.Add(message);
        colors.Add("normal");
        Display();
    }
    
    /// <summary>
    /// Clears GUI debug messages from console and storage.
    /// </summary>
    public void ClearMessages()
    {
        messages.Clear();
        colors.Clear();
        ClearScreen();
    }
    
    /// <summary>
    /// Clears GUI debug messages from the GUI console.
    /// </summary>
    void ClearScreen()
    {
        if (guis.Count < maxMessages)
        {
            //do nothing as we haven't created our guis yet
        }
        else
        {
            int x = 0;
            while (x < guis.Count)
            {
                GameObject gui = (GameObject)guis[x];
                gui.GetComponent<GUIText>().text = "";
                //increment and loop
                x += 1;
            }
        }
    }
    
    /// <summary>
    /// Prunes the array to fit within the maxMessages limit.
    /// </summary>
    void Prune()
    {
        int diff;
        if (messages.Count > maxMessages)
        {
            if (messages.Count <= 0)
            {
                diff = 0;
            }
            else
            {
                diff = messages.Count - maxMessages;
            }
            messages.RemoveRange(0, (int)diff);
            colors.RemoveRange(0, (int)diff);
        }

    }
    
    /// <summary>
    /// Displays the cached list of GUI messages and handles coloring of the
    /// messages.
    /// </summary>
    void Display()
    {
        //check if we are set to display
        if (visible == false)
        {
            ClearScreen();
        }
        else if (visible == true)
        {


            if (messages.Count > maxMessages)
            {
                Prune();
            }

            // Carry on with display
            int x = 0;
            if (guis.Count < maxMessages)
            {
                //do nothing as we havent created our guis yet
            }
            else
            {
                while (x < messages.Count)
                {
                    GameObject gui = (GameObject)guis[x];

                    //set our color
                    switch ((string)colors[x])
                    {
                        case "normal":
                            gui.GetComponent<GUIText>().material.color = normal;
                            break;
                        case "warning":
                            gui.GetComponent<GUIText>().material.color = warning;
                            break;
                        case "error":
                            gui.GetComponent<GUIText>().material.color = error;
                            break;
                    }

                    //now set the text for this element
                    gui.GetComponent<GUIText>().text = (string)messages[x];

                    //increment and loop
                    x += 1;
                }
            }

        }
    }
    #endregion


}// End DebugConsole Class