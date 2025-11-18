using UnityEngine;

namespace Recording
{
    public class OutputManagerEvents : MonoBehaviour
    {
        public delegate void SetSystem(string systemType);
        public static event SetSystem OnSetSystem;
        public static void SetSystemType(string systemType)
        {
            OnSetSystem?.Invoke(systemType);
        }
        
        public delegate void Record(string componentID, string componentState);
        public static event Record OnRecord;

        public static void RecordToOutput(string componentID, string componentState)
        {
            OnRecord?.Invoke(componentID, componentState);
        }
    }    
}

