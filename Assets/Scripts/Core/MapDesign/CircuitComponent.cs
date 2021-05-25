using System;

namespace Game.MapDesign
{
    public enum ComponentTypes
    {
        Battery,
        Fuse
    }

    [System.Serializable]
    public class CircuitComponent
    {
        // should not be 0, 0 is always reserved by ground
        private int _id = 1;
        public int ID 
        {
            get => _id;
            set 
            {
                if(value == 0) throw new Exception("Can't set Component ID to 0.");
                _id = value;
            }
        }

        public int R { get; set; }
        public int C { get; set; }
        public float Rotation { get; set; }
        public ComponentTypes Type { get; set; }

        public float? Voltage { get; set; }
        public float? Resistance { get; set; }
        public float? MaxCurrent { get; set; }
    }
}
