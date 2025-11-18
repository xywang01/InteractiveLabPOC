using System;
using System.Data;
using System.IO;
using UnityEngine;

namespace Recording
{
    public class TableCell<T>
{
    public string Key {get; set;}
    public T Value {get; set;}
        
    public TableCell(string key, T value)
    {
        Key = key;
        Value = value;
    }
}
    
public class RecordingTable //: MonoBehaviour
{
    private float _timestamp;
    private DataTable _table;
    private int _rowCount;
        
    // constructor
    public RecordingTable(string tableName="DefaultTable")
    {
        InitializeTable(tableName);
    }

    public DataTable Table => _table;

    public void PrintColumnNames()
    {
        foreach (DataColumn column in _table.Columns)
        {
            Debug.Log(column.ColumnName);
        }
    }
        
    void InitializeTable(string tableName)
    {
        _table = new DataTable(tableName);
        _rowCount = 0;

        // Create an ID column and make it the primary key column
        AddColumn("id", Type.GetType("System.Int32"), readOnly: true, unique: true);
        DataColumn[] primaryKeyColumns = new DataColumn[1];
        primaryKeyColumns[0] = _table.Columns["id"];
        _table.PrimaryKey = primaryKeyColumns;
            
        // Also use this class to control timestamps
        AddColumn("Timestamp", Type.GetType("System.Decimal"));
    }

    public void AddColumn(string colName, Type colType, bool readOnly = false, bool unique = false)
    {
        _table.Columns.Add(CreateColumn(colName, colType, readOnly, unique));
    }
        
    public void AddRow<T>(TableCell<T>[] values)
    {
        DataRow row = _table.NewRow();
        row["id"] = _rowCount;
        _rowCount++;
        row["Timestamp"] = Time.time;  // NOTE: Time.time get the global time since the application has started
            
        foreach (TableCell<T> cell in values)
        {
            row[cell.Key] = cell.Value;
        }
        _table.Rows.Add(row);
    }
    
    DataColumn CreateColumn(string columnName, Type dataType, bool readOnly = false, bool unique = false)
    {
        var column = new DataColumn
        {
            DataType = dataType, 
            ColumnName = columnName, 
            ReadOnly = readOnly, 
            Unique = unique
        };
        return column;
    }
    
    public void ToCsv(string strFilePath, bool allowOverwrite=false)
    {
        if (!allowOverwrite)
        {
            int fileCount = 0;
            while (System.IO.File.Exists(strFilePath))
            {
                string fileCountOld = fileCount == 0 ? "" : fileCount.ToString();
                fileCount++;

                string oldChar = $"{fileCountOld}.csv";
                string newChar = $"{fileCount}.csv";
                strFilePath = strFilePath.Replace(oldChar, newChar);
            }
        }
       
        StreamWriter sw = new StreamWriter(strFilePath, false);
        //headers    
        for (int i = 0; i < _table.Columns.Count; i++) {  
            sw.Write(_table.Columns[i]);  
            if (i < _table.Columns.Count - 1) {  
                sw.Write(",");  
            }  
        }  
        sw.Write(sw.NewLine);  
        foreach(DataRow dr in _table.Rows) {  
            for (int i = 0; i < _table.Columns.Count; i++) {  
                if (!Convert.IsDBNull(dr[i])) {  
                    string value = dr[i].ToString();  
                    if (value.Contains(",")) {  
                        value = String.Format("\"{0}\"", value);  
                        sw.Write(value);  
                    } else {  
                        sw.Write(dr[i].ToString());  
                    }  
                }  
                if (i < _table.Columns.Count - 1) {  
                    sw.Write(",");  
                }  
            }  
            sw.Write(sw.NewLine);  
        }  
        sw.Close();  
    }
}
}

