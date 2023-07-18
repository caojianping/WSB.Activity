using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Test
{
    class TestGenericMethods
    {
        public void SayHi<T>() where T: class
        {
            Type type = typeof(T);
            Console.WriteLine($"type.Name:{type.Name}");
        }
    }

    public class Student {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Teacher {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
    }
}
