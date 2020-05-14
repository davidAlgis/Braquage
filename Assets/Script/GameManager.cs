﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private static GameManager m_instance;


    //Debug tool
    private bool debugIsEnabled = true;

    /*-------Time---------*/
    //24h correspond to 48 real minutes.
    [SerializeField]
    private float m_durationOfDay = 48f; 
    //current Time. The 'G' at the end is for time in game
    private float m_currentTimeFloat = 0;
    //current Hours. The 'G' at the end is for time in game
    [SerializeField]
    private TimeInGame m_timeToBegin = new TimeInGame { };
    [SerializeField]
    private TimeInGame m_currentTimeInGame= new TimeInGame { };
    //To manipulate hours, if for example you want to passed time
    private int m_addHoursG = 0;
    [SerializeField]
    private int m_money = 0;


    #region getter
    public static GameManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            return m_instance;
        }
    }

    public float CurrentTimeFloat 
    {
        get => m_currentTimeFloat; set => m_currentTimeFloat = value; 
    }
    public float DurationOfDay 
    {
        get => m_durationOfDay; set => m_durationOfDay = value; 
    }
    public int AddHoursG 
    { 
        get => m_addHoursG; set => m_addHoursG += value; 
    }
    public TimeInGame CurrentTimeInGame 
    { 
        get => m_currentTimeInGame; set => m_currentTimeInGame = value; 
    }
    public int Money { get => m_money; set => m_money = value; }
    public bool DebugIsEnabled { get => debugIsEnabled; set => debugIsEnabled = value; }
    #endregion



    // Start is called before the first frame update
    void Start()
    {
        m_currentTimeInGame.DayG += m_timeToBegin.DayG;

        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Manage time in game.
        defineTime();
        //Manage Sun/Moon
        updateSunMoon();
    }

    public void updateSunMoon()
    {
        GameObject Astre = GameObject.Find("Astre");
        GameObject Sun = GameObject.Find("Sun");
        GameObject Moon = GameObject.Find("Moon");

        Vector3 myAxis = Vector3.right;
        Quaternion rot = Quaternion.AngleAxis(15.0f * m_currentTimeFloat + 180, myAxis);
        Astre.transform.rotation = rot;

        Sun.transform.LookAt(Vector3.zero);
        Moon.transform.LookAt(Vector3.zero);

    }

    public void gameOver()
    {
        StartCoroutine(gameOverCoroutine());
    }

    IEnumerator gameOverCoroutine()
    {
        UIManager.Instance.printGameOver();
        yield return new WaitForSeconds(5);
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);

    }

    public Vector3 getPlayerPosition()
    {
        return GameObject.Find("Player").GetComponent<Transform>().position;
    }

    public GameObject getActualCamera()
    {
        return GameObject.Find("Player Camera");
    }

    public Vector2 getMiddleOfCamera()
    {
        return new Vector2(Screen.width / 2, Screen.height / 2);//Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
    }

    #region time
    /*Manage time in game*/
    void defineTime()
    {
        //time that passed since start in minutes.
        float timeNow = Time.realtimeSinceStartup / 60f;
        //time in game in float in [0,24[
        m_currentTimeFloat = timeNow * 24 / m_durationOfDay - m_currentTimeInGame.DayG * 24 + m_addHoursG + m_timeToBegin.HoursG + m_timeToBegin.MinutesG / 60;
        //Now we just define the time in terms of days/hours/minutes.
        m_currentTimeInGame.HoursG= (int)(m_currentTimeFloat);
        if (m_currentTimeInGame.HoursG >= 24)
            m_currentTimeInGame.DayG += 1;
        m_currentTimeInGame.MinutesG = Math.Abs(m_currentTimeInGame.HoursG - m_currentTimeFloat) * 60;
        //If you want to print the time. 
        //CurrentTimeInGame.printTime();
    }
    #endregion
}
