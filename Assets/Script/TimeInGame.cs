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
            DebugTool.printError("Obtain negativ day when substracting time:" + time1.ToString() + " and " + time2.ToString());
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
        int minute = (int)m_minutesG;
        return this.m_hoursG.ToString()+"h"+minute.ToString()+ " Day:"+this.m_dayG.ToString();
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
            DebugTool.printError("Unable to convert string" + hoursAndMinuteStr[0] + "to int");
        if (!float.TryParse(hoursAndMinuteStr[1], out minutes))
            DebugTool.printError("Unable to convert string" + hoursAndMinuteStr[1] + "to float");
        TimeInGame timeConverted = new TimeInGame(day, hours, minutes);
        return timeConverted;
    }


    //Function to print a time
    public void printTime()
    {
        Debug.Log(m_hoursG + "h" + (int)m_minutesG + " Day:" + m_dayG);
    }
}
