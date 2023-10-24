using Microsoft.VisualBasic.FileIO;
using ZetesAPI_2.Models;

namespace ZetesAPI_2.Services
{
    public class CSVValidator : ICSVValidator
    {
        public CSVValidator()
        {
            
        }
        public ResponseModel ValidateCSV(IFormFile file)
        {
            try
            {
                using (TextFieldParser parser = new TextFieldParser(file.OpenReadStream()))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");

                    // Read the headers
                    string[] headers = parser.ReadFields();

                    if (headers == null || headers.Length < 5 || headers.Length > 100)
                    {
                        return new ResponseModel()
                        {
                            status = "failed",
                            code = "400",
                            message = "Invalid header. The CSV file must contain at least 5 and at most 100 fields.",
                            isSuccessful = false

                        };
                    }

                    // Dictionary to keep track of field names and their types
                    Dictionary<string, string> fieldTypes = new Dictionary<string, string>();
                    int rowIndex = 2; // Start counting rows from 2 (1-based index)

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        rowIndex++;

                        // Validate number of fields
                        if (fields.Length != headers.Length)
                        {
                            return new ResponseModel()
                            {
                                status = "failed",
                                code = "400",
                                message = $"Invalid number of fields in row {rowIndex}.",
                                isSuccessful = false

                            };
                        }

                        // Validate empty values and field name length
                        for (int i = 0; i < fields.Length; i++)
                        {
                            string fieldName = headers[i];
                            string fieldValue = fields[i];

                            if (string.IsNullOrWhiteSpace(fieldValue))
                            {
                                return new ResponseModel()
                                {
                                    status = "failed",
                                    code = "400",
                                    message = $"The value in the '{fieldName}' column on row {rowIndex} is empty.",
                                    isSuccessful = false

                                };
                            }

                            if (fieldName.Length > 100)
                            {
                                return new ResponseModel()
                                {
                                    status = "failed",
                                    code = "400",
                                    message = $"The field name '{fieldName}' cannot exceed 100 characters.",
                                    isSuccessful = false

                                };
                            }

                            // Validate field types
                            if (fieldName.Equals("Type", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!IsValidFieldType(fieldValue))
                                {
                                    return new ResponseModel()
                                    {
                                        status = "failed",
                                        code = "400",
                                        message = $"The value in the 'Type' column on row {rowIndex} is invalid.",
                                        isSuccessful = false

                                    };
                                }
                            }

                            // Validate Search and Visible properties
                            if (fieldName.Equals("Search", StringComparison.OrdinalIgnoreCase) || fieldName.Equals("Visible", StringComparison.OrdinalIgnoreCase))
                            {
                                if (!IsValidYesNoValue(fieldValue))
                                {
                                    return new ResponseModel()
                                    {
                                        status = "failed",
                                        code = "400",
                                        message = $"The value in the '{fieldName}' column on row {rowIndex} is invalid.",
                                        isSuccessful = false

                                    };
                                }
                            }

                            // Validate field type property rules
                            if (fieldTypes.ContainsKey("Type"))
                            {
                                if (!IsValidFieldProperties(fieldTypes["Type"], fieldName, fieldValue))
                                {
                                    return new ResponseModel()
                                    {
                                        status = "failed",
                                        code = "400",
                                        message = $"The value in the '{fieldName}' column on row {rowIndex} is invalid.",
                                        isSuccessful = false

                                    };
                                }
                            }
                        }

                        // Track field types
                        fieldTypes.Clear();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            fieldTypes[headers[i]] = fields[i];
                        }
                    }

                    // Check if there are at least 2 lines in the file
                    if (rowIndex < 3)
                    {
                        return new ResponseModel()
                        {
                            status = "failed",
                            code = "400",
                            message = "The CSV is empty. Please populate the CSV and import it again.",
                            isSuccessful = false

                        };
                    }

                    // Check if there is at least one Text field marked as Searchable
                    if (!fieldTypes.Any(kv => kv.Key.Equals("Type", StringComparison.OrdinalIgnoreCase) &&
                        kv.Value.Equals("Text", StringComparison.OrdinalIgnoreCase)))
                    {
                        return new ResponseModel()
                        {
                            status = "failed",
                            code = "400",
                            message = "At least one field must be marked as searchable.",
                            isSuccessful = false

                        };
                    }

                    // Check if there is at least one Date field
                    if (!fieldTypes.Any(kv => kv.Key.Equals("Type", StringComparison.OrdinalIgnoreCase) &&
                        kv.Value.Equals("Date", StringComparison.OrdinalIgnoreCase)))
                    {
                        return new ResponseModel()
                        {
                            status = "failed",
                            code = "400",
                            message = "At least one field must be of type Date.",
                            isSuccessful = false

                        };
                    }

                    // Check if there are more than 100 fields
                    if (headers.Length > 100)
                    {
                        return new ResponseModel()
                        {
                            status = "failed",
                            code = "400",
                            message = "More than 100 fields are not allowed.",
                            isSuccessful = false

                        };
                    }

                    // CSV is valid
                    return new ResponseModel()
                    {
                        status = "Success",
                        code = "200",
                        message = "CSV file is valid.",
                        isSuccessful = true

                    };
                }
            }
            catch (MalformedLineException ex)
            {
                return new ResponseModel()
                {
                    status = "failed",
                    code = "400",
                    message = "File content is not in a recognizable CSV format.",
                    data = ex.Source ,
                    innerException = ex.InnerException?.Message,
                    isSuccessful = false

                };
            }
            catch (Exception ex)
            {
                return new ResponseModel()
                {
                    status = "failed",
                    code = "400",
                    message = "Failed to import the file. Please refer to the 'Import best practices' article when creating the CSV and import it again.",
                    isSuccessful = false

                };
            }

        }
        private static bool IsValidFieldType(string fieldType)
        {
            string[] validFieldTypes = { "Text", "Number", "Yes/No", "Date", "Image" };
            return validFieldTypes.Contains(fieldType, StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsValidYesNoValue(string value)
        {
            return value.Equals("Yes", StringComparison.OrdinalIgnoreCase) || value.Equals("No", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsValidFieldProperties(string fieldType, string fieldName, string fieldValue)
        {
            switch (fieldType)
            {
                case "Text":
                    return fieldName.Equals("Search", StringComparison.OrdinalIgnoreCase) ||
                           fieldName.Equals("Library Filter", StringComparison.OrdinalIgnoreCase) ||
                           fieldName.Equals("Visible", StringComparison.OrdinalIgnoreCase);
                case "Number":
                    return fieldName.Equals("Visible", StringComparison.OrdinalIgnoreCase);
                case "Date":
                    return fieldName.Equals("Visible", StringComparison.OrdinalIgnoreCase);
                case "Yes/No":
                    return fieldName.Equals("Search", StringComparison.OrdinalIgnoreCase) ||
                           fieldName.Equals("Visible", StringComparison.OrdinalIgnoreCase);
                case "Image":
                    return fieldName.Equals("Visible", StringComparison.OrdinalIgnoreCase);
                default:
                    return false;
            }
        }
    }
}
