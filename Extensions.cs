using System;
using System.Collections.Generic;
using System.Diagnostics;           // <--- 'StackFrame' and 'StackTrace'
using System.IO;                    // <--- 'Path'
using Newtonsoft.Json;              // <--- 'JsonConvert' and 'Formatting.Indented'
using ConsoleTables;
using System.Linq;
using Newtonsoft.Json.Linq;
using MarkdownLog;
using System.Reflection;

public static class Extensions
{

    public  const String Start        = "START";
    public  const String Complete     = "COMPLETE";
    private static string currentTime = DateTime.Now.ToShortTimeString();


    public static void PrintKeysAndValues(Object obj)
    {
        foreach(PropertyInfo property in obj.GetType().GetProperties())
        {
            var propertyValue = property.GetValue(obj, null).ToString();
            Console.WriteLine($"{property.Name} --> {propertyValue}");
        }
    }


    // https://msdn.microsoft.com/en-us/library/system.consolekeyinfo(v=vs.110).aspx
    public static void ConsoleKey ()
    {
        Start.ThisMethod();

        ConsoleKeyInfo key = Console.ReadKey();
        Console.WriteLine(key);
        Console.WriteLine();
        Console.WriteLine("Character Entered: " + key.KeyChar);
        Console.WriteLine("Special Keys: " + key.Modifiers);
    }


    // retrieve high-level info about 'this'
    public static void Dig<T>(this T x)
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine("'DIG' STARTED");

        string json = JsonConvert.SerializeObject(x, Formatting.Indented);

        Console.WriteLine($"{x} --------------------------- {json} --------------------------- {x}");
        Console.WriteLine();
        Console.ResetColor();
    }


    // retrieve detailed info about 'this'
    public static void DigDeep<T>(this T x)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine("'DIG DEEP' STARTED");

        using (var writer = new System.IO.StringWriter())
        {
            ObjectDumper.Dumper.Dump(x, "Object Dumper", writer);
            Console.Write(writer.ToString());
        }
        Console.WriteLine();
        Console.ResetColor();
    }


    // set color of console message
        // example ---> valueX.WriteColor(ConsoleColor.Red)
    // public static void Spotlight<T>(this T x, string Message)
    public static void Spotlight (this string Message)
    {
        string fullMessage = JsonConvert.SerializeObject(Message, Formatting.Indented).ToUpper();

        StackFrame frame      = new StackFrame(1, true);
        var        lineNumber = frame.GetFileLineNumber();
        // var lineNumber = GetCurrentLineNumber();

        using (var writer = new System.IO.StringWriter())
        {
            // change text color
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{fullMessage} @ Line#: {lineNumber}");
            Console.Write(writer.ToString());
            Console.ResetColor();
        }
    }


    // shortcut console writer
    public static void Intro(this object Object, string String)
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;

        string     upperString = String.ToUpper();
        StackFrame frame       = new StackFrame(1, true);

        var lineNumber = frame.GetFileLineNumber();

        Console.WriteLine($"// {upperString} --> {Object} --> [@ Line# {lineNumber}]");

        Console.ResetColor();
        Console.WriteLine();
    }

    public static void TypeAndIntro(Object o, string x)
    {
        o.Intro(x);
        o.GetType().Intro($"TYPE for {x}");
    }


    // https://msdn.microsoft.com/en-us/library/system.io.path.getfilename(v=vs.110).aspx
    public static void ThisMethod(this string String)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine();

        StackTrace stackTrace = new StackTrace();

        // var methodName = GetMethodName();
        var methodName = stackTrace.GetFrame(1).GetMethod().Name;

        StackFrame frame    = new StackFrame(1, true);
        var        method   = frame.GetMethod();
        var        fileName = frame.GetFileName();

        var lineNumber = frame.GetFileLineNumber();

        string fileNameTrimmed = Path.GetFileName(fileName);

        Console.WriteLine($"--------------->|     {fileNameTrimmed} ---> {methodName} {String} [Line: {lineNumber} @ {currentTime}]     |<---------------");

        Console.ResetColor();
        Console.WriteLine();
    }

    public static void PrintJObjectItems(JObject JObjectToPrint)
    {
        var responseToJson = JObjectToPrint;

        foreach(var jsonItem in responseToJson)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine($"{jsonItem.Key.ToUpper()}");
            Console.ResetColor();
            Console.WriteLine(jsonItem.Value);
            Console.WriteLine();
        }
    }

    public static void TableIt(params object[] Object)
    {
        int countCheck = Object.Count();

        if(countCheck == 1)
        {
            var data = new[]
            {
                new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
            };
            Console.WriteLine();
            Console.Write(data.ToMarkdownTable());
            Console.WriteLine();
        }
        if(countCheck == 2)
        {
            var data = new[]
            {
                new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
            };
            Console.WriteLine();
            Console.Write(data.ToMarkdownTable());
            Console.WriteLine();
        }
        if(countCheck == 3)
        {
            var data = new[]
            {
                new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                new {Num = 3, Name = Object[2], Type = Object[2].GetType()}
            };
            Console.WriteLine();
            Console.Write(data.ToMarkdownTable());
            Console.WriteLine();
        }
        if(countCheck == 4)
        {
            var data = new[]
            {
                new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                new {Num = 3, Name = Object[2], Type = Object[2].GetType()},
                new {Num = 4, Name = Object[3], Type = Object[3].GetType()},
            };
            Console.WriteLine();
            Console.Write(data.ToMarkdownTable());
            Console.WriteLine();
        }
        if(countCheck == 5)
        {
            var data = new[]
            {
                new {Num = 1, Name = Object[0], Type = Object[0].GetType()},
                new {Num = 2, Name = Object[1], Type = Object[1].GetType()},
                new {Num = 3, Name = Object[2], Type = Object[2].GetType()},
                new {Num = 4, Name = Object[3], Type = Object[3].GetType()},
                new {Num = 5, Name = Object[4], Type = Object[4].GetType()},
            };
            Console.WriteLine();
            Console.Write(data.ToMarkdownTable());
            Console.WriteLine();
        }

        if(countCheck > 6)
        {
            Console.WriteLine("!!!!! TOO MANY THINGS TO TABLE !!!!!");
        }
        return;
    }

    /// <summary> </summary>
    /// <param name="itemsToList"> e.g., string[] planet = { "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune" };</param>
    public static void CreateNumberedList(string[] itemsToList)
    {
        Console.Write(itemsToList.ToMarkdownNumberedList());
    }
    public static void CreateBulletedList(string[] itemsToList)
    {
        Console.Write(itemsToList.ToMarkdownBulletedList());
    }


    /// <summary> </summary>
    /// <example>
        /// Step 1: var text = "Lolita, light of my life, fire of my loins. My sin, my soul.";
        /// Step 2: Extensions.CreateMarkdownParagraph(text); </example>
    /// <param name="text"></param>
    public static void CreateMarkdownParagraph(string text)
    {
        Console.Write(text.ToMarkdownParagraph());
    }


    // public static void TestTypes (Type type)
    // {
    //     Console.WriteLine("IsArray: {0}", type.IsArray);
    //     Console.WriteLine("Name: {0}", type.Name);
    //     Console.WriteLine("IsSealed: {0}", type.IsSealed);
    //     Console.WriteLine("BaseType.Name: {0}", type.BaseType.Name);
    //     Console.WriteLine();
    // }
}




public class Constants
{
    public string Start { get; set; }    = "START";
    public string Complete { get; set; } = "COMPLETE";
}