using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Science_IA___Uniform_Manager
{
    public class Uniform
    {
        private UniformClothing uniform;
        private Student? student;
        private string ID;
        private List<Condition> problems;
        private int size;
        private bool isCheckedOut = false;
        public Uniform(string ID, int number, UniformClothing uniform, int size)
        {
            this.ID = ID;
            this.uniform = uniform;
            this.size = size;
            this.problems = new List<Condition>();
            this.student = null;
        }
        public void CheckOutUniform() 
        {
            if (student == null)
            {
                return;
            }
            isCheckedOut = true;
        }
        public void CheckInUniform() 
        {
            if (student == null)
            {
                return;
            }
            isCheckedOut = false;
        }   
        public void AssignUniform(Student student)
        {
            if (this.student != null)
            {
                return;
            }
            this.student = student;
        }
        public void UnassignUniform()
        {
            this.student = null;
        }
        public Student? GetStudentAssignment()
        {
            return student;
        }
        public UniformClothing GetUniformType()
        {
            return uniform;
        }
        public string GetID()
        {
            return ID;
        }
        public int GetSize()
        {
            return size;
        }
        public bool IsCheckedOut()
        {
            return isCheckedOut;
        }
        public void AddCondition(Condition condition)
        {
            if (problems.Contains(condition))
            {
                return;
            }
            problems.Add(condition);
        }
        public void RemoveCondition(Condition condition)
        {
            if (!problems.Contains(condition))
            {
                return;
            }
            problems.Remove(condition);
        }
        public Condition[] GetConditions()
        {
            return problems.ToArray();
        }
    }
    public enum Condition
    {
        Stain,
        BrokenButton,
        BrokenZipper,
        Torn
    }
    public enum UniformClothing
    {
        ConcertCoat,
        DrumMajorCoat,
        Hat,
        MarchingCoat,
        MarchingShorts,
        MarchingSocks,
        Pants
    }

}
