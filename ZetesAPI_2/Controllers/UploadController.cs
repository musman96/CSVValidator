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
        public UploadController( CSVValidator validator)
        {
            _csvValidator = validator;
        }
        // GET: api/<UploadController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UploadController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
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

            responseModel = _csvValidator.ValidateCSV(file);
            return responseModel;
        }

        // PUT api/<UploadController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UploadController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
