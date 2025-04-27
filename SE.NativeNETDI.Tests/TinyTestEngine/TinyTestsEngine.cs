using System;
using System.Collections.Generic;
using System.Text;

namespace IngameScript
{
    //TODO: Move this to a separate project. Change the reporting system. Make it includable.
    //Rewrite it to be more generic and normal. Add BeforeStart, WhenEnd methods. TestClass.Setup() do I really need it?
    class TinyTestsEngine
    {
        private List<BaseTestClass> _testClasses = new List<BaseTestClass>();

        private StringBuilder _report = new StringBuilder();

        private Action<string> _debugEcho = null;

        public TinyTestsEngine()
        {
        }

        public TinyTestsEngine(Action<string> debugEcho)
        {
            _debugEcho = debugEcho;
        }

        public void Add(BaseTestClass testClass)
        {
            _testClasses.Add(testClass);
        }

        public void Add(params BaseTestClass[] testClasses)
        {
            _testClasses.AddRange(testClasses);
        }

        public IEnumerable<bool> Run()
        {
            int passed = 0;

            _report.AppendLine("==============================");
            _report.AppendLine($"Testing started at {DateTime.Now}");
            foreach (var testClass in _testClasses)
            {
                foreach (var test in testClass.Tests)
                {
                    try
                    {
                        _report.AppendLine("------------------------------");
                        _report.AppendLine($"Running: {test.Method.Name}");

                        if (_debugEcho != null)
                        {
                            _debugEcho($"Running: {test.Method.Name}");
                        }

                        testClass.Setup();
                        test.Invoke();

                        _report.AppendLine($"{test.Method.Name}: Passed");

                        passed++;
                    }
                    catch (Exception ex)
                    {
                        _report.AppendLine($"{test.Method.Name}: Failed");
                        _report.AppendLine($"Exception: {ex.Message}");
                    }
                    yield return false;
                }
            }
            _report.AppendLine($"Testing finished at {DateTime.Now}");

            _report = new StringBuilder($"Testing result: {passed}/{_testClasses.Count} Passed. Details:\n{_report}");
            _report.AppendLine("==============================");

            yield return true;
        }

        public string GetReport()
        {
            return _report.ToString();
        }
    }
}
