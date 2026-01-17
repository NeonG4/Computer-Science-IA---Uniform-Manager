using System.Collections.Generic;

namespace Computer_Science_IA___Uniform_Manager
{
    /// <summary>
    /// Represents a student in the uniform management system
    /// </summary>
    public class Student
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string StudentID { get; }
        public int Grade { get; }
        private readonly List<Uniform> uniforms;

        public Student(string firstName, string lastName, string studentID, int grade)
        {
            FirstName = firstName;
            LastName = lastName;
            StudentID = studentID;
            Grade = grade;
            uniforms = new List<Uniform>();
        }

        public string FullName => $"{FirstName} {LastName}";

        public Uniform[] GetUniforms() => uniforms.ToArray();

        public void AssignUniform(Uniform uniform)
        {
            if (!uniforms.Contains(uniform))
            {
                uniforms.Add(uniform);
            }
        }

        public void RemoveUniform(Uniform uniform)
        {
            uniforms.Remove(uniform);
        }
    }
}
