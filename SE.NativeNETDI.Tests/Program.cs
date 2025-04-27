using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using VRage.Game.GUI.TextPanel;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private readonly TinyTestsEngine _engine;

        private readonly IEnumerator<bool> _testEnumerator;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _engine = new TinyTestsEngine(Echo);

            _engine.Add(
               new ContainerTests()
            );

            _testEnumerator = _engine.Run().GetEnumerator();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (_testEnumerator.Current == true)
            {
                Runtime.UpdateFrequency = UpdateFrequency.None;

                Me.GetSurface(0).ContentType = ContentType.TEXT_AND_IMAGE;
                Me.GetSurface(0).WriteText(_engine.GetReport());

                return;
            }

            _testEnumerator.MoveNext();
        }
    }
}
