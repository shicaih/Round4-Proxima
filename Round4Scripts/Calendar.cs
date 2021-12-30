public class Calendar
{
    // record how many days have passed
    static private int day = 0;

    public Calendar()
    {

    }

    static public int Day
    {
        get { return day; }
        set { day = value; }
    }
}
