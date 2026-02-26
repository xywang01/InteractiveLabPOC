using System;
using System.IO;
using UnityEngine;

namespace Recording
{
    public class OutputManager : MonoBehaviour
    {
        public enum Sex
        {
            Male,
            Female,
            Other
        }

        private enum FileOutputType
        {
            Interaction,
            Movement,
            Vision
        }

        public string participantId;
        public Sex participantSex;
        public int participantAge;

        public Transform screenPosition;
        public Transform VRPosition;
        public Transform screenCamera;
        public Transform VRCamera;
        public float movementOffsetTimeCheck = 0.5f;
        public float movementOffsetDistanceCheck = 1f;
        private float movementOffsetTimer = 0f;
        public float visionOffsetTimeCheck = 0.25f;
        public float visionOffsetAngleCheck = 5f;
        private float visionOffsetTimer = 0f;

        private bool _testModeIsSet = false;
        private TestMode _testMode;
        private Transform _playerPosition;
        private Transform _playerCamera;
        private Vector3 _lastPlayerLocation = new Vector3(0, 0, 0);
        private float _lastPlayerRotation = 0f;

        private string _systemType;

        private string _outputFolder;
        public string OutputFolder => _outputFolder;
        private string _interactionOutputFileName;
        private string _movementOutputFileName;
        private string _visionOutputFileName;
        private string _participantsFileName;
        private string _mapOutputFileName;

        private RecordingTable _interactionOutputTable;
        private RecordingTable _movementOutputTable;
        private RecordingTable _visionOutputTable;

        private bool isSetUp = false;

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
        public void Setup(string age, string id, string sex)
        {
            AssignValues(age, id, sex);

            if (_testMode == TestMode.Screen)
            {
                _playerPosition = screenPosition;
                _playerCamera = screenCamera;
            }
            else
            {
                _playerPosition = VRPosition;
                _playerCamera = VRCamera;
            }

            _outputFolder = Path.Combine(Application.persistentDataPath, $"par_{participantId}");

            if (!Directory.Exists(_outputFolder))
            {
                Directory.CreateDirectory(_outputFolder);
            }

            _participantsFileName = Application.persistentDataPath + "/participant_data.csv";
            if (!System.IO.File.Exists(_participantsFileName))
            {
                File.AppendAllText(_participantsFileName, "Timestamp,ParticipantID,ParticipantSex,ParticipantAge" + Environment.NewLine);
            }

            File.AppendAllText(_participantsFileName, $"{DateTime.Now:yyyyMMdd},{participantId},{participantSex},{participantAge}" + Environment.NewLine);

            // EQUIPMENT RECORDING SETUP ========================================
            _interactionOutputTable = new RecordingTable();
            _interactionOutputTable.AddColumn("ComponentID", Type.GetType("System.String"));
            _interactionOutputTable.AddColumn("ComponentState", Type.GetType("System.String"));
            _interactionOutputTable.AddColumn("SystemType", Type.GetType("System.String"));

            // MOVEMENT RECORDING SETUP ========================================
            _movementOutputTable = new RecordingTable();
            _movementOutputTable.AddColumn("Location", Type.GetType("System.String"));

            // MOVEMENT RECORDING SETUP ========================================
            _visionOutputTable = new RecordingTable();
            _visionOutputTable.AddColumn("Angle", Type.GetType("System.String"));

            if (!_testModeIsSet)
            {
                _testMode = ModeManagerEvents.GetCurrentMode();
            }

            SetOutputFileName();

            isSetUp = true;
        }

        private void AssignValues(string age, string id, string sex)
        {
            participantId = id;
            participantAge = int.Parse(age);

            if (sex.ToLower() == "male")
            {
                participantSex = Sex.Male;
            }
            else if (sex.ToLower() == "female")
            {
                participantSex = Sex.Female;
            }
            else
            {
                participantSex = Sex.Other;
            }
        }

        void SetOutputFileName()
        {
            // Create output file name
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm");

            _interactionOutputFileName = _outputFolder + $"/par_{participantId}_{_testMode}_interaction_{timestamp}.csv";
            _movementOutputFileName = _outputFolder + $"/par_{participantId}_{_testMode}_movement_{timestamp}.csv";
            _visionOutputFileName = _outputFolder + $"/par_{participantId}_{_testMode}_vision_{timestamp}.csv";
            _mapOutputFileName = _outputFolder + $"/par_{participantId}_{_testMode}_map_{timestamp}.png";

            // check for duplicate files
            //int fileCount = 0;
            //while (System.IO.File.Exists(_interactionOutputFileName))
            //{
            //    string fileCountOld = fileCount == 0 ? "" : fileCount.ToString();
            //    string oldChar = fileCount == 0 ? $"{fileCountOld}.csv" : $"_{fileCountOld}.csv";

            //    fileCount++;
            //    string newChar = $"_{fileCount}.csv";
            //    _interactionOutputFileName = _interactionOutputFileName.Replace(oldChar, newChar);
            //}
            CheckForDuplicates(_interactionOutputFileName);
            CheckForDuplicates(_movementOutputFileName);
            CheckForDuplicates(_visionOutputFileName);
        }

        private void CheckForDuplicates(string fileName)
        {
            int fileCount = 0;
            while (System.IO.File.Exists(fileName))
            {
                string fileCountOld = fileCount == 0 ? "" : fileCount.ToString();
                string oldChar = fileCount == 0 ? $"{fileCountOld}.csv" : $"_{fileCountOld}.csv";

                fileCount++;
                string newChar = $"_{fileCount}.csv";
                fileName = fileName.Replace(oldChar, newChar);
            }
        }
    
        void RecordOutput(string componentID, string componentState)
        {
            Debug.Log("Recording Output");
            Debug.Log(componentID);
            Debug.Log(componentState);
            _interactionOutputTable.AddRow(new TableCell<object>[]
            {
                new TableCell<object>("SystemType", _systemType), 
                new TableCell<object>("ComponentID", componentID), 
                new TableCell<object>("ComponentState", componentState)
            });
            
            // save the data everytime the table is updated - overwrite!
            _interactionOutputTable.ToCsv(_interactionOutputFileName, allowOverwrite:true);
        }

        private void RecordMovementOutput(string location)
        {
            _movementOutputTable.AddRow(new TableCell<object>[]
            {
                new TableCell<object>("Location", location)
            });

            // save the data everytime the table is updated - overwrite!
            _movementOutputTable.ToCsv(_movementOutputFileName, allowOverwrite: true);
        }

        private void RecordVisionOutput(string angle)
        {
            _visionOutputTable.AddRow(new TableCell<object>[]
            {
                new TableCell<object>("Angle", angle)
            });

            // save the data everytime the table is updated - overwrite!
            _visionOutputTable.ToCsv(_visionOutputFileName, allowOverwrite: true);
        }

        private void MovementCheck()
        {
            if (movementOffsetTimer <= movementOffsetTimeCheck)
            {
                movementOffsetTimer += Time.deltaTime;
            }
            else
            {
                _lastPlayerLocation.y = 0;
                Vector3 currentLocation = _playerPosition.position;
                currentLocation.y = 0;
                Vector3 distance = currentLocation - _lastPlayerLocation;

                if (distance.sqrMagnitude >= movementOffsetDistanceCheck)
                {
                    _lastPlayerLocation = _playerPosition.position;
                    string recordLocation = _lastPlayerLocation.ToString();
                    StartCoroutine(MapRecorder.Instance.GenerateMap(_playerPosition));
                    RecordMovementOutput(recordLocation);
                    movementOffsetTimer = 0f;
                }
            }
        }

        private void VisionCheck()
        {
            if (visionOffsetTimer <= visionOffsetTimeCheck)
            {
                visionOffsetTimer += Time.deltaTime;
            }
            else
            {
                float currentRotation = _playerCamera.transform.eulerAngles.x;
                float distance = currentRotation - _lastPlayerRotation;

                if (Math.Abs(distance) >= visionOffsetAngleCheck)
                {
                    _lastPlayerRotation = _playerCamera.transform.eulerAngles.x;
                    string recordAngle = _lastPlayerRotation.ToString();
                    RecordVisionOutput(recordAngle);
                    visionOffsetTimer = 0f;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!isSetUp)
            {
                return;
            }

            //if (!_testModeIsSet)
            //{
            //    _testMode = ModeManagerEvents.GetCurrentMode();
            //    SetOutputFileName();
            //}

            MovementCheck();
            VisionCheck();
        }

        public string GetMapOutputName()
        {
            return _mapOutputFileName;
        }
    }
}

