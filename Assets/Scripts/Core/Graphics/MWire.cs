using UnityEngine;
using Game.Data;

namespace Game.Graphics
{
    [RequireComponent(typeof(LineRenderer))]
    public class MWire : MonoBehaviour
    {
        public Wire Wire { get; set; }

        public void Init()
        {
            CircuitGrid.Instance?.DrawWire(Wire.T1.WorldPosition, Wire.T2.WorldPosition, GetComponent<LineRenderer>());
        }
    }
}
