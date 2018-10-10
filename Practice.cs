// https://www.tutorialspoint.com/csharp/csharp_events.htm


using System;

namespace BaseballScraper
{
    public class Practice
    {
        delegate int NumberChanger(int n);

        class TestDelegate
        {
            static int num = 10;
            public static int AddNum(int p)
            {
                num += p;
                return num;
            }
            public static int MultiplyNum(int q)
            {
                num *= q;
                return num;
            }
            public static int getNum()
            {
                return num;
            }
        }


        public static void DelegatePractice()
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Initial Number --> {0}", TestDelegate.getNum());

            // BaseballScraper.Program+NumberChanger
            // Method: Int32 AddNumb(Int32)
            NumberChanger numberChangerAddNum = new NumberChanger(TestDelegate.AddNum);

            // Step 1: 'num' + number passed to delegate NumberChanger; 10 + 25 = 35
            int numberAdd = numberChangerAddNum(25);

            Console.WriteLine("----------------------------------------");
            Console.WriteLine("Add Number --> {0}", TestDelegate.getNum());
            Console.WriteLine("----------------------------------------");

            NumberChanger numberChangerMultiplyNum = new NumberChanger(TestDelegate.MultiplyNum);

            // Step 2: output of Step 1 * number passed to delegate NumberChanger; 35 * 5 = 175
            int numberMultiply = numberChangerMultiplyNum(5);

            Console.WriteLine("Multiply Number --> {0}", TestDelegate.getNum());
            Console.WriteLine("----------------------------------------");

            // Console.ReadKey();
        }


        public delegate string MyDel(string str);

        public class EventProgram
        {
            event MyDel MyEvent;

            public EventProgram()
            {
                Console.WriteLine("MarkA");
                this.MyEvent += new MyDel(this.WelcomeUser);
            }
            public string WelcomeUser(string username)
            {
                Console.WriteLine("MarkB");
                return "Welcome " + username;
            }

            public static void DelegateEventPractice ()
            {
                Console.WriteLine("MarkC");
                EventProgram obj1 = new EventProgram();
                string result = obj1.MyEvent("Chicago Cubs");
                Console.WriteLine($"Result: {result}");
            }
        }
    }
}