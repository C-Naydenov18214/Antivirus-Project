using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rx
{
    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }


        public Person(int age, string name) 
        {
            this.Name = name;
            this.Age = age;
        
        }

    }
}
