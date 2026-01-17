using System.Collections.Generic;
using System.Linq;

namespace Computer_Science_IA___Uniform_Manager
{
    /// <summary>
    /// Represents a uniform item in the inventory
    /// </summary>
    public class Uniform
    {
        public string ID { get; }
        public UniformClothing Type { get; }
        public int Size { get; }
        public bool IsCheckedOut { get; private set; }
        public Student? AssignedStudent { get; private set; }
        
        private readonly List<Condition> conditions;

        public Uniform(string id, UniformClothing uniformType, int size)
        {
            ID = id;
            Type = uniformType;
            Size = size;
            conditions = new List<Condition>();
            AssignedStudent = null;
            IsCheckedOut = false;
        }

        /// <summary>
        /// Checks out the uniform to the assigned student
        /// </summary>
        public void CheckOut()
        {
            if (AssignedStudent == null)
            {
                return;
            }
            IsCheckedOut = true;
        }

        /// <summary>
        /// Checks in the uniform from the assigned student
        /// </summary>
        public void CheckIn()
        {
            if (AssignedStudent == null)
            {
                return;
            }
            IsCheckedOut = false;
        }

        /// <summary>
        /// Assigns this uniform to a student
        /// </summary>
        public void AssignToStudent(Student student)
        {
            if (AssignedStudent != null)
            {
                return;
            }
            AssignedStudent = student;
            student.AssignUniform(this);
        }

        /// <summary>
        /// Unassigns this uniform from the current student
        /// </summary>
        public void UnassignFromStudent()
        {
            if (AssignedStudent != null)
            {
                AssignedStudent.RemoveUniform(this);
                AssignedStudent = null;
            }
            IsCheckedOut = false;
        }

        public void AddCondition(Condition condition)
        {
            if (!conditions.Contains(condition))
            {
                conditions.Add(condition);
            }
        }

        public void RemoveCondition(Condition condition)
        {
            conditions.Remove(condition);
        }

        public Condition[] GetConditions() => conditions.ToArray();

        public bool HasCondition(Condition condition) => conditions.Contains(condition);

        public bool IsInGoodCondition => conditions.Count == 0;
    }

    /// <summary>
    /// Represents the condition/problem with a uniform item
    /// </summary>
    public enum Condition
    {
        Stain,
        BrokenButton,
        BrokenZipper,
        Torn,
        Missing,
        Faded
    }

    /// <summary>
    /// Types of uniform clothing items
    /// </summary>
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
