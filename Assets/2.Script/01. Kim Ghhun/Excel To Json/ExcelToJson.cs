using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ExcelToJson
{
    [HideInInspector]
    public string excelFilePath;
    [HideInInspector]
    public string jsonOutputPath;

    public void ConvertExcelToJson(int sheetNum)
    {
        if (string.IsNullOrEmpty(excelFilePath) || string.IsNullOrEmpty(jsonOutputPath))
        {
            Debug.LogError("Excel Path, Json Path is NULL");
            return;
        }

        // Open Excel File
        using (FileStream stream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(stream); //For .xlsx

            if (sheetNum < 0 || sheetNum >= workbook.NumberOfSheets)
            {
                Debug.LogError($"유효하지 않은 시트 번호: {sheetNum}. 총 시트 수: {workbook.NumberOfSheets}");
                return; // 유효하지 않은 경우 종료
            }

            ISheet sheet = workbook.GetSheetAt(sheetNum); // Select Sheet;

            IFormulaEvaluator evaluator = workbook.GetCreationHelper().CreateFormulaEvaluator();

            // List for data from converted json
            var rowsData = new List<Dictionary<string, object>>();
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;


            // Extract Data looping every row in sheet
            for (int i = 1; i <= sheet.LastRowNum; i++) // 0 is Header
            {
                IRow row = sheet.GetRow(i);
                var rowData = new Dictionary<string, object>();

                for (int j = 0; j < cellCount; j++)
                {
                    string columnName = headerRow.GetCell(j).ToString(); //Get ColumnName from Header

                    if(row.GetCell(j) == null)
                    {
                        Debug.LogError("Cell range ERROR! Please delete the remaining rows except for the last one");
                    }

                    ICell cell = row.GetCell(j);

                    rowData[columnName] = GetValueFromCell(cell, evaluator); //Get Cell Value
                }

                rowsData.Add(rowData); // Add Row data to List
            }

            //save Data after convert Json
            string json = JsonConvert.SerializeObject(rowsData, Formatting.Indented);
            File.WriteAllText(jsonOutputPath, json);

            Debug.Log($"Convert Excel To Json : {jsonOutputPath}");
        }
    }

    private object GetValueFromCell(CellValue cellValue)
    {
        switch (cellValue.CellType)
        {
            case CellType.Numeric:
                return cellValue.NumberValue;
            case CellType.String:
                return cellValue.StringValue;
            case CellType.Boolean:
                return cellValue.BooleanValue;
            default:
                return null;
        }
    }

    private object GetValueFromCell(ICell cell, IFormulaEvaluator eval)
    {
        if (cell == null) return null;

        switch (cell.CellType)
        {
            case CellType.Numeric:
                return cell.NumericCellValue;
            case CellType.String:
                return cell.StringCellValue;
            case CellType.Formula:
                var evaluatedCell = eval.Evaluate(cell);
                return GetValueFromCell(evaluatedCell);
            case CellType.Boolean:
                return cell.BooleanCellValue;
            default:
                return null;
        }
    }
}
