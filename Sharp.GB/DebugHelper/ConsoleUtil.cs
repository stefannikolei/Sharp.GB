using System;

public static class ConsoleUtil
{
    public static void PrintSeparator(int width)
    {
        Console.WriteLine("".PadLeft(width, '-'));
    }
}
