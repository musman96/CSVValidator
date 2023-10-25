using ZetesAPI_2.Data;

namespace ZetesAPI_2.Services
{
    public interface ICsvResponsesDbRepository
    {
        bool AddRecord(CsvResponses responses);
        bool DeleteRecord(CsvResponses responses);
    }
}
