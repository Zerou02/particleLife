using System;
using Godot;

class Utils
{
    public static string trimStr(string str, int amountChars)
    {
        var retVal = "";
        for (int i = 0; i < amountChars; i++)
        {
            if (i >= str.Length)
            {
                break;
            }
            retVal += str[i];
        }
        return retVal;
    }
    public static string trimAfterDecimalPoint(string str, int amountCharsAfterPoint)
    {
        var beforeDp = str.Split(".")[0].Length;
        var trimmed = trimStr(str, amountCharsAfterPoint + beforeDp + 1);
        if (trimmed.Contains("."))
        {
            trimmed = trimmed.TrimEnd('0');
        }
        return trimmed;
    }

    public static DateTime startTimeMeasurement()
    {
        return DateTime.Now;
    }

    public static void endTimeMeasurement(DateTime startTime, string message)
    {
        GD.Print(message + ": " + DateTime.Now.Subtract(startTime).Duration().TotalSeconds);
    }

    private static int idCount = 0;
    public static int getID()
    {
        idCount++;
        return idCount - 1;
    }
}