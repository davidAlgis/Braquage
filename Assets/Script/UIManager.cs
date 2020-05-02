using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager m_instance;

    // ! important this have to be configure in UI !
    [SerializeField]
    private GameObject m_messageBoxPanel;

    #region getter

    public void Start()
    {
        m_messageBoxPanel.SetActive(true);
        Button buttonDisable = transform.Find("MessageBox/ButtonDisableMessage").GetComponent<Button>();
        buttonDisable.onClick.AddListener(delegate () { this.disableMessageBox(); });
        m_messageBoxPanel.SetActive(false);
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

    public void enableUIPressButton(bool enabled, string key="F")
    {
        transform.Find("PressKey").GetComponent<Image>().enabled= enabled;
        transform.Find("PressKey/TextKey").GetComponent<Text>().enabled = enabled;
        transform.Find("PressKey/TextKey").GetComponent<Text>().text = key;
    }

    public void enableMessageBox(string message)
    {
        m_messageBoxPanel.SetActive(true);
        Button  buttonDisable = transform.Find("MessageBox/ButtonDisableMessage").GetComponent<Button>();
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
    }
}
