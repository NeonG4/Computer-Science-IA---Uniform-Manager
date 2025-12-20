using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Science_IA___Uniform_Manager
{
    public class Student
    {
        private string fName;
        private string lName;
        private string studentID;
        private int grade;
        private List<Uniform> uniforms;
        public Student(string fName, string lName, string studentID, int grade)
        {
            this.fName = fName;
            this.lName = lName;
            this.studentID = studentID;
            this.grade = grade;
            this.uniforms = new List<Uniform>();
        }
        public string GetFirstName()
        {
            return fName;
        }
        public string GetLastName()
        {
            return lName;
        }
        public string GetStudentID()
        {
            return studentID;
        }
        public int GetGrade()
        {
            return grade;
        }
        public Uniform[] GetUniforms()
        {
            return uniforms.ToArray();
        }
    }
}
