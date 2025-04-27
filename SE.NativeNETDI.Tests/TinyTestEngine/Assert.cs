using System;
using System.Linq;

namespace IngameScript
{
    static class Assert
    {
        public static void ForceFail(string message = "")
        {
            throw new Exception($"Test failed. Was called Fail method. {message}");
        }

        public static void AreEqual<T>(T expected, T actual, string message = "")
        {
            if (!expected.Equals(actual))
            {
                throw new Exception($"\nExpected: {expected}, \nActual: {actual}. {message}");
            }
        }

        public static void Throws<T>(Action action, string message = "") where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T)
            {
                return;
            }
            catch (Exception ex)
            {
                throw new Exception($"Expected exception of type {typeof(T).Name}, but got {ex.GetType().Name} instead. {message}");
            }
            throw new Exception($"Expected exception of type {typeof(T).Name}, but no exception was thrown. {message}");
        }

        public static void IsTrue(bool condition, string message = "")
        {
            if (!condition)
            {
                throw new Exception($"Condition is false. {message}");
            }
        }

        public static void IsFalse(bool condition, string message = "")
        {
            if (condition)
            {
                throw new Exception($"Condition is true. {message}");
            }
        }

        public static void IsNull(object obj, string message = "")
        {
            if (obj != null)
            {
                throw new Exception($"Object is not null. {message}");
            }
        }

        public static void IsNotNull(object obj, string message = "")
        {
            if (obj == null)
            {
                throw new Exception($"Object is null. {message}");
            }
        }

        public static void AreEquals<T>(T[] expected, T[] actual, string message = "")
        {
            if (!expected.SequenceEqual(actual))
            {
                var expectedElements = string.Join(", ", expected);
                var actualElements = string.Join(", ", actual);
                throw new Exception($"Expected: [{expectedElements}], Actual: [{actualElements}]. {message}");
            }
        }
    }
}
