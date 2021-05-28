using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game.Graphics
{
    public class Map : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private MTerminal _terminal;
        [SerializeField] private MBattery _battery;
        [SerializeField] private MFuse _fuse;
        [SerializeField] private MWire _wire;

        public Data.Map MapData { get; private set; }

        private Dictionary<int, MTerminal> _terminals;

        public Vector3 GetTerminalPosition(int id) => _terminals[id].transform.position;
        public void CreateFromMap(Data.Map map)
        {
            MapData = map;
            foreach(Terminal terminal in map.Terminals)
            {
                var t = Instantiate(_terminal);
                t.Terminal = terminal;
                t.transform.position = terminal.WorldPosition;
            }
            foreach(Battery battery in map.Batteries)
            {
                var b = Instantiate(_battery);
                b.Battery = battery;
                SetPositionRotation(b.transform, battery.T1.WorldPosition, battery.T2.WorldPosition, _battery.Height / 2);
            }
            foreach(Fuse fuse in map.Fuses)
            {
                var b = Instantiate(_fuse);
                b.Fuse = fuse;
                SetPositionRotation(b.transform, fuse.T1.WorldPosition, fuse.T2.WorldPosition, _fuse.Height / 2);
            }
            foreach(Wire wire in map.Wires)
            {
                var b = Instantiate(_wire);
                b.Wire = wire;
            }
            map.worldPosProvider = GetTerminalPosition;
        }

        public static void SetPositionRotation(Transform transform, Vector3 p1, Vector3 p2, float height)
        {
            transform.position = Vector3.MoveTowards(p1, p2, height / 2);
            transform.up = (p1 - p2).normalized;
        }
    }
}
