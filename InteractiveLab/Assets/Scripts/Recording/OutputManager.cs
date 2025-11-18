using System;
using UnityEngine;

namespace Recording
{
    public class OutputManager : MonoBehaviour
    {
        public enum Sex
        {
            Male,
            Female
        }

        public string participantId;
        public Sex participantSex;
        public int participantAge;

        private bool _testModeIsSet = false;
        private TestMode _testMode;

        private string _systemType;

        private string _outputFolder = "Assets/BehavioralData";
        private string _outputFileName;
        
        private RecordingTable _outputTable;

        private void OnEnable()
        {
            OutputManagerEvents.OnRecord += RecordOutput;
            OutputManagerEvents.OnSetSystem += SetSystemType;
        }

        private void OnDisable()
        {
            OutputManagerEvents.OnRecord -= RecordOutput;
            OutputManagerEvents.OnSetSystem -= SetSystemType;
        }
        
        private void SetSystemType(string systemType)
        {
            _systemType = systemType;
            _testModeIsSet = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            
            // EQUIPMENT RECORDING SETUP ========================================
            _outputTable = new RecordingTable();
            _outputTable.AddColumn("ComponentID", Type.GetType("System.String"));
            _outputTable.AddColumn("ComponentState", Type.GetType("System.String"));
            _outputTable.AddColumn("SystemType", Type.GetType("System.String"));
            _outputTable.AddColumn("TestMode", Type.GetType("System.String"));
            _outputTable.AddColumn("ParticipantID", Type.GetType("System.String"));
            _outputTable.AddColumn("ParticipantSex", Type.GetType("System.String"));
            _outputTable.AddColumn("ParticipantAge", Type.GetType("System.Int32"));
        }

        void SetOutputFileName()
        {
            // Create output file name
            _outputFileName = $"{_outputFolder}/par_{participantId}_{_testMode}_{DateTime.Now:yyyyMMdd}.csv";
            
            // check for duplicate files
            int fileCount = 0;
            while (System.IO.File.Exists(_outputFileName))
            {
                string fileCountOld = fileCount == 0 ? "" : fileCount.ToString();
                string oldChar = fileCount == 0 ? $"{fileCountOld}.csv" : $"_{fileCountOld}.csv";
                
                fileCount++;
                string newChar = $"_{fileCount}.csv";
                _outputFileName = _outputFileName.Replace(oldChar, newChar);
            }
        }
    
        void RecordOutput(string componentID, string componentState)
        {
            _outputTable.AddRow(new TableCell<object>[]
            {
                new TableCell<object>("SystemType", _systemType), 
                new TableCell<object>("ComponentID", componentID), 
                new TableCell<object>("ComponentState", componentState),
                new TableCell<object>("TestMode", _testMode.ToString()),
                new TableCell<object>("ParticipantID", participantId),
                new TableCell<object>("ParticipantSex", participantSex.ToString()),
                new TableCell<object>("ParticipantAge", participantAge)
            });
            
            // save the data everytime the table is updated - overwrite!
            _outputTable.ToCsv(_outputFileName, allowOverwrite:true);
        }

        // Update is called once per frame
        void Update()
        {
            if (!_testModeIsSet)
            {
                _testMode = ModeManagerEvents.GetCurrentMode();
                SetOutputFileName();
            }
        }
    }
}

