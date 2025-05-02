using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {       
        private readonly IGridInfo _gridInfo;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;

            _container.RegisterSingleton<ILogger, EchoLogger>(e => new EchoLogger(Echo));
            _container.RegisterSingleton<IMyProgrammableBlock>(e => Me);
            _container.RegisterSingleton<IGridInfo, GridInfo>(e => new GridInfo(e.Resolve<ILogger>(), e.Resolve<IMyProgrammableBlock>()));

            //_gridInfo = _container.Resolve<IGridInfo>();
            _container.Resolve(_gridInfo);
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _gridInfo.GetId();
        }
    }

    class GridInfo: IGridInfo 
    {
        private readonly ILogger _logger;

        private readonly IMyProgrammableBlock _me;

        public GridInfo(ILogger logger, IMyProgrammableBlock me)
        {
            _logger = logger;
            _me = me;
        }

        public long GetId()
        {
            _logger.LogLine($"{DateTime.Now:hh:mm:ss:fff} Was called GetId. Id is: {_me.CubeGrid.EntityId}");

            return _me.CubeGrid.EntityId;
        }
    }

    internal interface IGridInfo
    {
        long GetId();
    }

    interface ILogger
    {       
        void LogLine(string message);
    }

    class EchoLogger : ILogger
    {
        private readonly Action<string> _echo;

        public EchoLogger(Action<string> echo)
        {
            _echo = echo;
        }      

        public void LogLine(string message)
        {
            _echo(message + Environment.NewLine);
        }      
    }
}
