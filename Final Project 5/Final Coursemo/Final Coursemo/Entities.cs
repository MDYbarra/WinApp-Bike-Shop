using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final_Coursemo
{
    public partial class Student
    {
        public override string ToString()
        {
            return this.LastName + ", " + this.FirstName + ", "+ "(" + this.NetId + ")";
        }
    }



    public partial class Cours
    {
        public override string ToString()
        {
            return this.Dept + " " + this.CourseNum + " " + this.CRN;
        }
    }





}
