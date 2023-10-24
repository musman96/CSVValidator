using ZetesAPI_2.Models;

namespace ZetesAPI_2.Services
{
    public interface ICSVValidator
    {
        ResponseModel ValidateCSV(IFormFile file);
    }
}
