using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cette classe "gère" le temps in game.


/*Cette classe n'hérite pas de MonoBehavior. En effet, elle n'utilise 
 * aucun outil d'unity. De plus cela poserait des soucis dans 
 * l'instanciation des objets si on le laisserait en monobehavior.
 */
[System.Serializable]
public class TimeInGame
{
    [SerializeField]
    private int m_hoursG = 0;
    [SerializeField]
    private float m_minutesG = 0;
    [SerializeField]
    private int m_dayG = 0;
    

    //private readonly TimeInGame m_timeBegin = new TimeInGame(0, 0, 0f);

    public TimeInGame(int day, int hours, float minutes)
    {
        m_dayG = day;
        m_hoursG = hours;
        m_minutesG = minutes;
    }

    public TimeInGame()
    {
        m_dayG = 0;
        m_hoursG = 0;
        m_minutesG = 0f;
    }

    #region getter
    public int HoursG
    {
        get => m_hoursG; set => m_hoursG = value;
    }
    public float MinutesG
    {
        get => m_minutesG; set => m_minutesG = value;
    }
    public int DayG
    {
        get => m_dayG; set => m_dayG = value;
    }
    #endregion

    #region overloading operator
    public static TimeInGame operator -(TimeInGame time1, TimeInGame time2)
    {
        int day;
        int hours;
        float minutes;
        day = time1.m_dayG - time2.m_dayG;
        hours = time1.m_hoursG - time2.m_hoursG;
        minutes = time1.m_minutesG - time2.m_minutesG;
        TimeInGame lessTime;
        if (minutes<0)
        {
            hours--;
            minutes = 60 + minutes;
        }
        if (hours<0)
        {
            day--;
            hours = 24 + hours;
        }
        //On n'accepte pas les Day négatifs
        if (day < 0)
        {
            day = 0;
            Debug.LogError("Obtain negativ day when substracting time:" + time1.ToString() + " and " + time2.ToString());
        }
        return lessTime = new TimeInGame(day,hours,minutes);
    }

    public static TimeInGame operator +(TimeInGame time1, TimeInGame time2)
    {
        int day;
        int hours;
        float minutes;
        day = time1.m_dayG + time2.m_dayG;
        hours = time1.m_hoursG + time2.m_hoursG;
        minutes = time1.m_minutesG + time2.m_minutesG;
        TimeInGame plusTime;
        if (minutes > 59)
        {
            hours++;
            minutes = minutes-60;
        }
        if (hours > 23)
        {
            day++;
            hours = hours-24;
        }
        return plusTime = new TimeInGame(day, hours, minutes);
    }

    public static bool operator ==(TimeInGame time1, TimeInGame time2)
    {
        if (time1.m_dayG == time2.m_dayG && time1.m_hoursG == time2.m_hoursG && time1.m_minutesG == time2.m_minutesG)
            return true;
        else
            return false;
    }

    public static bool operator !=(TimeInGame time1, TimeInGame time2)
    {
        if (time1.m_dayG == time2.m_dayG && time1.m_hoursG == time2.m_hoursG && time1.m_minutesG == time2.m_minutesG)
            return false;
        else
            return true;
    }

    public static bool operator >(TimeInGame time1, TimeInGame time2)
    {
        if (time1.m_dayG > time2.m_dayG)
            return true;
        else if (time1.m_dayG < time2.m_dayG)
            return false;
        else
        {
            if (time1.m_hoursG > time2.m_hoursG)
                return true;
            else if (time1.m_hoursG < time2.m_hoursG)
                return false;
            else
            {
                if (time1.m_minutesG > time2.m_minutesG)
                    return true;
                else if (time1.m_minutesG < time2.m_minutesG)
                    return false;
                else
                    return false;
            }
        }
    }

    public static bool operator <(TimeInGame time1, TimeInGame time2)
    {
        if (time1.m_dayG < time2.m_dayG)
            return true;
        else if (time1.m_dayG > time2.m_dayG)
            return false;
        else
        {
            if (time1.m_hoursG < time2.m_hoursG)
                return true;
            else if (time1.m_hoursG > time2.m_hoursG)
                return false;
            else
            {
                if (time1.m_minutesG < time2.m_minutesG)
                    return true;
                else if (time1.m_minutesG > time2.m_minutesG)
                    return false;
                else
                    return false;
            }
        }
    }

    public static bool operator >=(TimeInGame time1, TimeInGame time2)
    {
        if (time1.m_dayG > time2.m_dayG)
            return true;
        else if (time1.m_dayG < time2.m_dayG)
            return false;
        else
        {
            if (time1.m_hoursG > time2.m_hoursG)
                return true;
            else if (time1.m_hoursG < time2.m_hoursG)
                return false;
            else
            {
                if (time1.m_minutesG > time2.m_minutesG)
                    return true;
                else if (time1.m_minutesG < time2.m_minutesG)
                    return false;
                else
                    return true;
            }
        }
    }

    public static bool operator <=(TimeInGame time1, TimeInGame time2)
    {
        if (time1.m_dayG < time2.m_dayG)
            return true;
        else if (time1.m_dayG > time2.m_dayG)
            return false;
        else
        {
            if (time1.m_hoursG < time2.m_hoursG)
                return true;
            else if (time1.m_hoursG > time2.m_hoursG)
                return false;
            else
            {
                if (time1.m_minutesG < time2.m_minutesG)
                    return true;
                else if (time1.m_minutesG > time2.m_minutesG)
                    return false;
                else
                    return true;
            }
        }
    }

    public override string ToString()
    {
        return dayToDate(m_dayG).ToString() + ", " + m_hoursG.ToString() + "h" + ((int)m_minutesG).ToString();
    }

    //to avoid warning
    public override int GetHashCode()
    {
        return (int)m_minutesG + m_hoursG + m_dayG;
    }

    //to avoid warning
    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TimeInGame time2 = (TimeInGame)obj;
            return m_dayG == time2.m_dayG && m_hoursG == time2.m_hoursG && m_minutesG == time2.m_minutesG;
        }
    }

    #endregion

    //doesn't include day, it'll only convert string of type "03h12"
    public static TimeInGame strInHoursToTimeInGame(string timeStr,int day=0)
    {
        string[] hoursAndMinuteStr = timeStr.Split('h');
        int hours;
        float minutes;
        if (!int.TryParse(hoursAndMinuteStr[0], out hours))
            Debug.LogError("Unable to convert string" + hoursAndMinuteStr[0] + "to int");
        if (!float.TryParse(hoursAndMinuteStr[1], out minutes))
            Debug.LogError("Unable to convert string" + hoursAndMinuteStr[1] + "to float");
        TimeInGame timeConverted = new TimeInGame(day, hours, minutes);
        return timeConverted;
    }

    //we use the variable originDate of the gamemanager
    public static int dateToDay(Date date)
    {
        int nbrOfDay = date.Day;
        switch (date.Month)
        {
            case enumMonth.FEV:
                nbrOfDay += 31;
                break;

            case enumMonth.MAR:
                nbrOfDay += 31+29;
                break;

            case enumMonth.AVR:
                nbrOfDay += 31 + 29 + 31;
                break;

            case enumMonth.MAI:
                nbrOfDay += 31 + 29 + 31 + 30;
                break;

            case enumMonth.JUIN:
                nbrOfDay += 31 + 29 + 31 + 30 + 31;
                break;

            case enumMonth.JUIL:
                nbrOfDay += 31 + 29 + 31 + 30 + 31 + 30;
                break;

            case enumMonth.AOU:
                nbrOfDay += 31 + 29 + 31 + 30 + 31 + 30 + 31;
                break;

            case enumMonth.SEP:
                nbrOfDay += 31 + 29 + 31 + 30 + 31 + 30 + 31 + 31;
                break;

            case enumMonth.OCT:
                nbrOfDay += 31 + 29 + 31 + 30 + 31 + 30 + 31 + 31 + 30;
                break;

            case enumMonth.NOV:
                nbrOfDay += 31 + 29 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31;
                break;

            case enumMonth.DEC:
                nbrOfDay += 31 + 29 + 31 + 30 + 31 + 30 + 31 + 31 + 30 + 31 + 30;
                break;
        }
        
        return nbrOfDay + 366 * (date.Year - GameManager.Instance.OriginDate.Year);
    }

    //we use the variable originDate of the gamemanager
    public static Date dayToDate(int day)
    {
        
        Date date = new Date(GameManager.Instance.OriginDate.Day, GameManager.Instance.OriginDate.Month, GameManager.Instance.OriginDate.Year);
        date += day;
        return date;
    }

    //Function to print a time
    public void printTime()
    {
        Debug.Log(dayToDate(m_dayG).ToString() + ", " + m_hoursG + "h" + (int)m_minutesG);
    }
}


//We suppose that each year is bissextile like 2020
[System.Serializable]
public class Date
{
    private int m_day;
    private enumMonth m_month;
    private int m_year;

    public int Day { get => m_day; set => m_day = value; }
    public enumMonth Month { get => m_month; set => m_month = value; }
    public int Year { get => m_year; set => m_year = value; }

    public Date(int day, enumMonth month, int year)
    {
        
        m_day = day;
        m_month = month;
        m_year = year;


        if (checkCoherency() == false)
            m_day = 1;
    }

    public bool checkCoherency()
    {
        if (m_day > 31 || m_day < 1)
        {
            Debug.LogWarning("Invalid Date day are " + m_day + " in month " + m_month);
            return false;
        }
            
        switch (m_month)
        {
            case enumMonth.FEV:
                if (m_day > 29)
                {
                    Debug.LogWarning("Invalid Date day are " + m_day + " in month " + m_month);
                    return false;
                }
                break;
            case enumMonth.AVR:
                if (m_day > 30)
                {
                    Debug.LogWarning("Invalid Date day are " + m_day + " in month " + m_month); 
                    return false;
                }
                break;
            case enumMonth.JUIN:
                if (m_day > 30)
                {
                    Debug.LogWarning("Invalid Date day are " + m_day + " in month " + m_month);
                    return false;
                }
                break;
            case enumMonth.SEP:
                if (m_day > 30)
                {
                    Debug.LogWarning("Invalid Date day are " + m_day + " in month " + m_month);
                    return false;
                }
                break;
            case enumMonth.NOV:
                if (m_day > 30)
                {
                    Debug.LogWarning("Invalid Date day are " + m_day + " in month " + m_month);
                    return false;
                }
                break;
        }
        return true;
    }

    public static Date operator +(Date date, int dayToAdd)
    {
        for (int i = 0; i < dayToAdd; i++)
            date.addOneDay();

        return date;
    }

    //cast date into the NUMBER_FR format
    public override string ToString()
    {
        return dayToString() + "/" + monthToString() + "/" + m_year;
    }

    public string monthToString(enumFormatDate format = enumFormatDate.NUMBER_FR)
    {
        switch(m_month)
        { 
            case enumMonth.JAN:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "01";
                else
                    return "Janvier";

            case enumMonth.FEV:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "02";
                else
                    return "Février";

            case enumMonth.MAR:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "03";
                else
                    return "Mars";

            case enumMonth.AVR:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "04";
                else
                    return "Avril";

            case enumMonth.MAI:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "05";
                else
                    return "Mai";

            case enumMonth.JUIN:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "06";
                else
                    return "Juin";

            case enumMonth.JUIL:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "07";
                else
                    return "Juillet";

            case enumMonth.AOU:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "08";
                else
                    return "Août";

            case enumMonth.SEP:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "09";
                else
                    return "Septembre";

            case enumMonth.OCT:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "10";
                else
                    return "Octobre";

            case enumMonth.NOV:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "11";
                else
                    return "Novembre";

            case enumMonth.DEC:
                if (format == enumFormatDate.NUMBER_FR || format == enumFormatDate.NUMBER_EN)
                    return "12";
                else
                    return "Décembre";
        }
        return "";
    }

    public string dayToString()
    {
        if (m_day < 10)
            return "0" + m_day.ToString();
        else
            return m_day.ToString();
    }

    public string printDate(enumFormatDate format = enumFormatDate.NUMBER_FR)
    {
        switch (format)
        {
            case (enumFormatDate.NUMBER_FR):
                return dayToString() + "/" + monthToString() + "/" + m_year;
            case (enumFormatDate.NUMBER_EN):
                return monthToString() + "/" + dayToString() + "/" + m_year;
            case (enumFormatDate.LETTER):
                return monthToString() + " " + dayToString() + " " + m_year; 
        }
        return "";
    }

    public void addOneDay()
    {

        switch (m_month)
        {
            case enumMonth.JAN:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.FEV;
                }
                else
                    m_day++;
                break;

            case enumMonth.FEV:
                if (m_day >= 29)
                {
                    m_day = 1;
                    m_month = enumMonth.MAR;
                }
                else
                    m_day++;
                break;

            case enumMonth.MAR:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.AVR;
                }
                else
                    m_day++;
                break;

            case enumMonth.AVR:
                if (m_day >= 30)
                {
                    m_day = 1;
                    m_month = enumMonth.MAI;
                }
                else
                    m_day++;
                break;

            case enumMonth.MAI:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.JUIN;
                }
                else
                    m_day++;
                break;

            case enumMonth.JUIN:
                if (m_day >= 30)
                {
                    m_day = 1;
                    m_month = enumMonth.JUIL;
                }
                else
                    m_day++;
                break;

            case enumMonth.JUIL:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.AOU;
                }
                else
                    m_day++;
                break;

            case enumMonth.AOU:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.SEP;
                }
                else
                    m_day++;
                break;

            case enumMonth.SEP:
                if (m_day >= 30)
                {
                    m_day = 1;
                    m_month = enumMonth.OCT;
                }
                else
                    m_day++;
                break;

            case enumMonth.OCT:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.NOV;
                }
                else
                    m_day++;
                break;

            case enumMonth.NOV:
                if (m_day >= 30)
                {
                    m_day = 1;
                    m_month = enumMonth.DEC;
                }
                else
                    m_day++;
                break;

            case enumMonth.DEC:
                if (m_day >= 31)
                {
                    m_day = 1;
                    m_month = enumMonth.JAN;
                    m_year++;
                }
                else
                    m_day++;
                break;
        }
    }

}

public enum enumMonth
{
    JAN,
    FEV,
    MAR,
    AVR,
    MAI,
    JUIN,
    JUIL,
    AOU,
    SEP,
    OCT,
    NOV,
    DEC
}

public enum enumFormatDate
{
    //dd/mm/yyyy
    NUMBER_FR,
    //mm/dd/yyyy
    NUMBER_EN,
    //month dd yyyy
    LETTER
}