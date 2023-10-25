using Microsoft.EntityFrameworkCore;
using ZetesAPI_2.Data;
using ZetesAPI_2.Models;

namespace ZetesAPI_2.Services
{
    public class CsvResponsesDBRepository : ICsvResponsesDbRepository
    {
        private CsvvalidateContext _context;
        public CsvResponsesDBRepository(CsvvalidateContext csvvalidate) 
        {
            _context = csvvalidate;
        }
        public bool AddRecord(CsvResponses responses)
        {
            bool success = false;
            try
            {
                responses.Id = Guid.NewGuid();
                var entity = _context.CsvResponses.Add(responses).Entity;
                _context.Entry(responses).State = EntityState.Added;
                var added = _context.SaveChanges();

                if (added > 0)
                {
                    success= true;
                }
                 success =false;
            }
            catch (Exception ex)
            {

                throw;
            }
            return success;
        }

        public bool DeleteRecord(CsvResponses responses)
        {
            var record =  _context.CsvResponses.Find(responses.Id);
            _context.CsvResponses.Remove(record);
             _context.SaveChanges();
            return true;
        }
    }
}
