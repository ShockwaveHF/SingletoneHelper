using System;
using System.Collections.Generic;
using System.Management.Instrumentation;
using System.Reflection;

namespace ConsoleApp1
{
    public class Program
    {
        public static SingletoneHelper SingletoneInstance = new SingletoneHelper();

        private static void Main()
        {
            Program.SingletoneInstance.OnObjectCreateEvent += SingletoneInstance_OnObjectCreateEvent;
            Program.SingletoneInstance.OnGetInstanceEvent += SingletoneInstance_OnGetInstanceEvent; ;
            Program.SingletoneInstance.OnObjectRemovedEvent += SingletoneInstance_OnObjectRemovedEvent;

            Test1 test = new Test1();
            test.Method();

            Test2 test2 = new Test2();
            test2.Method();
        }

        private static void SingletoneInstance_OnGetInstanceEvent(Type obj)
        {
            Console.WriteLine($"Get instance {obj.Name}");
        }

        private static void SingletoneInstance_OnObjectCreateEvent(Type obj)
        {
            Console.WriteLine($"New object create {obj.Name}");
        }

        private static void SingletoneInstance_OnObjectRemovedEvent(Type obj)
        {
            Console.WriteLine($"Object removed {obj.Name}");
        }
    }

    internal class SuperClasss
    {
        private int count = 0;

        public void Add()
        {
            count = count + 1;
        }

        public void Print()
        {
            Console.WriteLine(count);
        }
    }

    public class Test1
    {
        public void Method()
        {
            Program.SingletoneInstance.GetInstance<SuperClasss>().Add();
            Program.SingletoneInstance.GetInstance<SuperClasss>().Print();

            //SuperClasss SuperClass = Program.SingletoneInstance.GetInstance<SuperClasss>();
            //SuperClass.Add();
        }
    }

    public class Test2
    {
        private SuperClasss SuperClass = Program.SingletoneInstance.GetInstance<SuperClasss>();
        private Test1 test = Program.SingletoneInstance.GetInstance<Test1>();

        public void Method()
        {
            SuperClass.Add();
            SuperClass.Print();
            Program.SingletoneInstance.RemoveClass<SuperClasss>();
        }
    }
}