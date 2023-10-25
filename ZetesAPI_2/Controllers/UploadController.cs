using Microsoft.AspNetCore.Mvc;
using ZetesAPI_2.Models;
using ZetesAPI_2.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ZetesAPI_2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private CSVValidator _csvValidator;
        private ICsvResponsesDbRepository _csvResponsesDbRepository;
        public UploadController( CSVValidator validator, CsvResponsesDBRepository repository)
        {
            _csvValidator = validator;
            _csvResponsesDbRepository = repository;
        }
       
        // POST api/<UploadController>
        [HttpPost]
        public ResponseModel Upload(IFormFile file)
        {
            var responseModel = new ResponseModel();
            if (file == null)
            {
                return new ResponseModel()
                {
                    status = "failed",
                    code = "400",
                    message = "No file provided. Please provide a file",
                    isSuccessful = false

                };
            }

            if (file.ContentType != "text/csv")
            {
                return new ResponseModel()
                {
                    status = "failed",
                    code = "400",
                    message = "Invalid file type. File must be CSV.",
                    isSuccessful = false

                };
            }

            //responseModel = _csvValidator.ValidateCSV(file);

            // check if the validation was successful
            //if (responseModel.isSuccessful)
            if(true)
            {
                // get the records and save to list
                var records = _csvValidator.GetSuccessCsvResponses(file);
                int savedRecordCount = 0;
                // save to the database
                foreach ( var record in records ) 
                {
                    bool saved = _csvResponsesDbRepository.AddRecord(record);
                    savedRecordCount = saved ? savedRecordCount+1 : savedRecordCount+0;
                }

                if (records.Count == savedRecordCount)
                {
                    responseModel = new ResponseModel()
                    {
                        status = "Success",
                        code = "200",
                        message = "All records have been saved to the database",
                        isSuccessful = true

                    };
                }
                else
                {
                    responseModel = new ResponseModel()
                    {
                        status = "Failed partially",
                        code = "200",
                        message = "Not all the records were saved to the database",
                        isSuccessful = true

                    };
                }
           
            }
            return responseModel;
        }

    }
}
