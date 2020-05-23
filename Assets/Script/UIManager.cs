using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;

    // ! important this have to be configure in UI !
    [SerializeField]
    private GameObject m_messageBoxPanel;

    private GameObject[] m_canvasWorldGO;

    private const float m_minDistanceSeeUI = 2.0f;

    //private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;
    

    #region getter

    public void Awake()
    {

        m_messageBoxPanel.SetActive(true);

        GameObject messageBoxGO, buttonDisableMessageGO;

        if (DebugTool.tryFindGOChildren(gameObject, "MessageBox", out messageBoxGO, LogType.Error) == false)
            return;

        if (DebugTool.tryFindGOChildren(messageBoxGO, "ButtonDisableMessage", out buttonDisableMessageGO, LogType.Error) == false)
            return;

        if (buttonDisableMessageGO.TryGetComponent(out Button buttonDisable) == false)
            Debug.LogError("Unable to find any Button componement in " + buttonDisableMessageGO.name);
        
        buttonDisable.onClick.AddListener(delegate () { this.disableMessageBox(); });
        m_messageBoxPanel.SetActive(false);

        m_canvasWorldGO = GameObject.FindGameObjectsWithTag("WorldCanvas");
    }

    void Update()
    {
        //TODO change this temporary line to avoid using find at each frame
        transform.Find("TextTime").GetComponent<Text>().text = GameManager.Instance.CurrentTimeInGame.ToString();

        if (Input.GetMouseButtonDown(0))
        {
            List<GameObject> worldCanvasGONearBy;
            if ((worldCanvasGONearBy = isNearWorldCanvas()) != null)
            {
                foreach (GameObject canvasWorld in worldCanvasGONearBy)
                {
                    if (canvasWorld.TryGetComponent(out GraphicRaycaster graphicRaycaster))
                        clickOnButton(graphicRaycaster);
                    else
                        Debug.LogWarning("Unable to find GraphicRaycaster componement in " + canvasWorld.name);
                }
                    
            }
                
        }
            
    }

    public static UIManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.Find("MainUI").GetComponent<UIManager>();
            }
            return m_instance;
        }
    }
    #endregion

    public void enableUIPressButton(bool enabled, string key = "F")
    {
        GameObject pressKeyGO, textKeyGO;
        if (DebugTool.tryFindGOChildren(gameObject, "PressKey", out pressKeyGO, LogType.Error))
        {
            if (pressKeyGO.TryGetComponent(out Image imagePressKey))
                imagePressKey.enabled = enabled;
            else
                Debug.LogError("Unable to find any Image componement in " + pressKeyGO.name);
        }

        if (DebugTool.tryFindGOChildren(pressKeyGO, "TextKey", out textKeyGO, LogType.Error))
        {
            if (textKeyGO.TryGetComponent(out Text textPressKey))
            { 
                textPressKey.enabled = enabled;
                textPressKey.text = key;
            }
            else
                Debug.LogError("Unable to find any Text componement in " + textKeyGO.name);

            
        }
    }

    public void enableMessageBox(string message)
    {
        m_messageBoxPanel.SetActive(true);

        GameObject messageBoxGO, buttonDisableMessageGO, messageTextGO;

        if (DebugTool.tryFindGOChildren(gameObject, "MessageBox", out messageBoxGO, LogType.Error))
        {
            if (DebugTool.tryFindGOChildren(messageBoxGO, "ButtonDisableMessage", out buttonDisableMessageGO, LogType.Error))
            {
                if(buttonDisableMessageGO.TryGetComponent(out Button buttonDisableMessage))
                    buttonDisableMessage.onClick.AddListener(delegate () { this.disableMessageBox(); });
                else
                    Debug.LogError("Unable to find any Button componement in " + buttonDisableMessageGO.name);
            }

            if (DebugTool.tryFindGOChildren(messageBoxGO, "MessageText", out messageTextGO, LogType.Error))
            {
                if (messageTextGO.TryGetComponent(out Text messageText))
                    messageText.text = message;
                else
                    Debug.LogError("Unable to find any Text componement in " + messageTextGO.name);
            }
        }
    }

    public void disableMessageBox()
    {
        m_messageBoxPanel.SetActive(false);
    }

    public void updateUIMoney()
    {
        GameObject textMoneyGO;
        if (DebugTool.tryFindGOChildren(gameObject, "TextMoney", out textMoneyGO, LogType.Error))
        {
            if (textMoneyGO.TryGetComponent(out Text textMoney))
                textMoney.text = GameManager.Instance.Money.ToString() + " €";
            else
                Debug.LogError("Unable to find any Text componement in " + textMoneyGO.name);
        }
    }

    public void printGameOver()
    {
        GameObject gameOverGO;
        if (DebugTool.tryFindGOChildren(gameObject, "GameOver", out gameOverGO, LogType.Error))
        {
            if (gameOverGO.TryGetComponent(out Text gameOverText))
                gameOverText.text = GameManager.Instance.Money.ToString() + " €";
            else
                Debug.LogError("Unable to find any Text componement in " + gameOverGO.name);
        }
    }

    public List<GameObject> isNearWorldCanvas()
    {
        
        if (m_canvasWorldGO != null)
        {
            List<GameObject> canvasWorldNearByGO = new List<GameObject>();
            Vector3 playerPosition = GameManager.Instance.getPlayerPosition();
            foreach (GameObject canvasWorld in m_canvasWorldGO)
            {
                if (Vector3.Distance(canvasWorld.transform.position, playerPosition) < m_minDistanceSeeUI)
                {
                    canvasWorldNearByGO.Add(canvasWorld);
                }
                
            }
            return canvasWorldNearByGO;
        }

        return null;
    }

    public void enableDisableMouse(bool enable)
    {
        if (GameManager.Instance.getActualPlayerGO().TryGetComponent(out FirstPersonAIO player))
        {
            //you need to first disable cursor before disabling enableCameraMovement
            if(player.enableCameraMovement == false)
            {
                Debug.LogWarning("You try to enable cursor, but you need to do that before turning false the enableCameraMovement attributes ");
                //turn the enableCameraMovement on
                player.enableCameraMovement = true;

                //enable or disable cursor
                player.lockAndHideCursor = !enable;
                Cursor.visible = enable;
                if (enable)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Confined;

                //return the enableCameraMovement off
                player.enableCameraMovement = false;
                return;
            }
                
            player.lockAndHideCursor = !enable;
            if(enable)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = enable;
        }
    }

    void clickOnButton(GraphicRaycaster graphicRaycaster)
    {
        
        m_PointerEventData = new PointerEventData(m_EventSystem);

        m_PointerEventData.position = GameManager.Instance.getMiddleOfCamera();

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(m_PointerEventData, results);

        //send a graphic raycaster and if it hit a button then invoke the onClick function
        foreach (RaycastResult result in results)
        {
            if(result.gameObject.TryGetComponent<Button>(out Button button))
                button.onClick.Invoke();
        }
    }
}
