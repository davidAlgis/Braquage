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

    private GameObject[] m_canvasWorldGameObject;

    private const float m_minDistanceSeeUI = 2.0f;

    private GraphicRaycaster m_Raycaster;
    private PointerEventData m_PointerEventData;
    private EventSystem m_EventSystem;
    

    #region getter

    public void Start()
    {
        m_messageBoxPanel.SetActive(true);
        Button buttonDisable = transform.Find("MessageBox/ButtonDisableMessage").GetComponent<Button>();
        buttonDisable.onClick.AddListener(delegate () { this.disableMessageBox(); });
        m_messageBoxPanel.SetActive(false);

        m_canvasWorldGameObject = GameObject.FindGameObjectsWithTag("WorldCanvas");

        m_Raycaster = GameObject.Find("Canvas_screen_digicode").GetComponent<GraphicRaycaster>();
        m_EventSystem = GetComponent<EventSystem>();

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
        transform.Find("PressKey").GetComponent<Image>().enabled = enabled;
        transform.Find("PressKey/TextKey").GetComponent<Text>().enabled = enabled;
        transform.Find("PressKey/TextKey").GetComponent<Text>().text = key;
    }

    public void enableMessageBox(string message)
    {
        m_messageBoxPanel.SetActive(true);
        Button buttonDisable = transform.Find("MessageBox/ButtonDisableMessage").GetComponent<Button>();
        buttonDisable.onClick.AddListener(delegate () { this.disableMessageBox(); });
        transform.Find("MessageBox/MessageText").GetComponent<Text>().text = message;
    }

    public void disableMessageBox()
    {
        m_messageBoxPanel.SetActive(false);
    }

    public void updateUIMoney()
    {
        transform.Find("TextMoney").GetComponent<Text>().text = GameManager.Instance.Money.ToString() + " €";
    }

    public void printGameOver()
    {
        transform.Find("GameOver").GetComponent<Text>().text = "Game Over";
    }

    // Update is called once per frame
    void Update()
    {
        transform.Find("TextTime").GetComponent<Text>().text = GameManager.Instance.CurrentTimeInGame.ToString();

        if (Input.GetMouseButtonDown(0))
            if (isNearWorldCanvas())
                clickOnButton();
    }


    bool isNearWorldCanvas()
    {
        
        if (m_canvasWorldGameObject != null)
        {
            Vector3 playerPosition = GameManager.Instance.getPlayerPosition();
            foreach (GameObject canvasWorld in m_canvasWorldGameObject)
            {
                if (Vector3.Distance(canvasWorld.transform.position, playerPosition) < m_minDistanceSeeUI)
                    return true;
            }
        }

        return false;
    }


    void clickOnButton()
    {

        m_PointerEventData = new PointerEventData(m_EventSystem);

        m_PointerEventData.position = GameManager.Instance.getMiddleOfCamera();

        
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            if(result.gameObject.TryGetComponent<Button>(out Button button))
                button.onClick.Invoke();
        }
    }
}
