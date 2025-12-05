/*
I Mingi Kang, 000818677, certify that this material is my original work. 
No other person's work has been used without suitable acknowledgment 
and I have not made my work available to anyone else.
*/

public static class DBManager
{
    public static string username;
    public static int score;
    public static int pet;

    public static string ServerBaseUrl = "https://antiquewhite-hippopotamus-744274.hostingersite.com/sqlconnect/";

    public static bool LoggedIn { get { return username != null; } }

    public static void LogOut()
    {
        username = null;
    }
}
