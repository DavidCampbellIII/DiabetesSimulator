using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimeOfDay
{
    [SerializeField, Range(0, 23),
        Tooltip("0 is midnight, 12 is noon, 23 is 11pm")]
    private int _hour;
    public int hour { get => _hour; private set => _hour = value; }

    [SerializeField, Range(0, 59)]
    private int _minute;
    public int minute { get => _minute; private set => _minute = value; }

    [SerializeField, HideInInspector] 
    private int _second;
    public int second { get => _second; private set => _second = value; }

    public TimeOfDay()
    {
        hour = 0;
        minute = 0;
        second = 0;
    }

    public TimeOfDay(int hour, int minute) : this(hour, minute, 0)
    {
    }

    public TimeOfDay(int hour, int minute, int second)
    {
        if (hour > 23 || hour < 0)
        {
            Debug.LogError("==TIME OF DAY ERROR== Hour " + hour + " is invalid.  Hour must be between 0-23 inclusive");
            return;
        }

        if (minute > 59 || minute < 0)
        {
            Debug.LogError("==TIME OF DAY ERROR== Minute " + minute + " is invalid.  Minute must be between 0-59 inclusive");
            return;
        }

        if (second > 59 || second < 0)
        {
            Debug.LogError("==TIME OF DAY ERROR== Second " + second + " is invalid.  Second must be between 0-59 inclusive");
            return;
        }

        this.hour = hour;
        this.minute = minute;
        this.second = second;
    }

    /// <summary>
    /// Given two TimeOfDays, returns the later time
    /// </summary>
    public static TimeOfDay Max(TimeOfDay time1, TimeOfDay time2)
    {
        if(time1.hour == time2.hour)
        {
            if(time1.minute == time2.minute)
            {
                return time1.second > time2.second ? time1 : time2;
            }
            return time1.minute > time2.minute ? time1 : time2;
        }
        return time1.hour > time2.hour ? time1 : time2;
    }

    public static TimeOfDay Min(TimeOfDay time1, TimeOfDay time2)
    {
        if (time1.hour == time2.hour)
        {
            if (time1.minute == time2.minute)
            {
                return time1.second < time2.second ? time1 : time2;
            }
            return time1.minute < time2.minute ? time1 : time2;
        }
        return time1.hour < time2.hour ? time1 : time2;
    }

    public static int SecondsBetween(TimeOfDay time1, TimeOfDay time2)
    {
        //find min and max
        TimeOfDay maxTime = Max(time1, time2);
        TimeOfDay minTime = maxTime == time1 ? time2 : time1;

        TimeOfDay diff = maxTime - minTime;
        return diff.ToSeconds();
    }

    public static TimeOfDay Lerp(TimeOfDay time1, TimeOfDay time2, float t)
    {
        int secondsBetween = SecondsBetween(time1, time2);
        int secondsToAdd = (int)(secondsBetween * t);
        return time1.AddSeconds(secondsToAdd);
    }

    public static TimeOfDay FromSeconds(int seconds)
    {
        int hours = seconds / 3600;
        seconds %= 3600;
        int minutes = seconds / 60;
        seconds %= 60;

        return new TimeOfDay(hours, minutes, seconds);
    }

    public int ToSeconds()
    {
        int mins = minute + hour * 60;
        return mins * 60 + second;
    }

    public static TimeOfDay operator +(TimeOfDay lhs, TimeOfDay rhs)
    {
        int secSum = lhs.second + rhs.second;
        int minSum = lhs.minute + rhs.minute;
        int hourSum = 0;

        // if sec sum goes above 60, add an extra minute and wrap around seconds
        if (secSum >= 60)
        {
            minSum += 1;
            secSum -= 60;
        }

        // if min sum goes above 60, add an extra hour and wrap around minutes
        if (minSum >= 60)
        {
            hourSum += 1;
            minSum -= 60;
        }

        hourSum += lhs.hour + rhs.hour;

        return new TimeOfDay(hourSum, minSum, secSum);
    }

    public static TimeOfDay operator -(TimeOfDay lhs, TimeOfDay rhs)
    {
        int secDiff = lhs.second - rhs.second;
        int minDiff = lhs.minute - rhs.minute;
        int hourDiff = 0;

        // if sec diff goes below zero, subtract an extra minute and wrap around seconds
        if (secDiff < 0)
        {
            minDiff -= 1;
            secDiff += 60;
        }

        // if min diff goes below zero, subtract an extra hour and wrap around minutes
        if (minDiff < 0)
        {
            hourDiff -= 1;
            minDiff += 60;
        }

        hourDiff += lhs.hour - rhs.hour;

        return new TimeOfDay(hourDiff, minDiff, secDiff);
    }

    public TimeOfDay AddSeconds(int secondsToAdd)
    {
        int newSecond = second + secondsToAdd;
        int newMinute = minute;
        int newHour = hour;

        if(newSecond >= 60)
        {
            int extraMinutes = newSecond / 60;
            newSecond %= 60;
            newMinute += extraMinutes;

            if(newMinute >= 60)
            {
                int extraHours = newMinute / 60;
                newMinute %= 60;
                newHour += extraHours;

                newHour %= 24;
            }
        }

        return new TimeOfDay(newHour, newMinute, newSecond);
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        bool isPM = hour >= 12;
        if(hour > 12)
        {
            builder.Append((hour % 12).ToString("00"));
        }
        else
        {
            builder.Append(hour.ToString("00"));
        }


        builder.Append(":").Append(minute.ToString("00")).Append(":").Append(second.ToString("00"));
        if(isPM)
        {
            builder.Append("PM");
        }
        else
        {
            builder.Append("AM");
        }

        return builder.ToString();
    }
}
