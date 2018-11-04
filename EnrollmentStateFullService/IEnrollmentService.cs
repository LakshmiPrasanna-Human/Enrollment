using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace EnrollmentStateFullService
{
    public interface IStudentEnrollmentService
    {
        // Gets the list of EnrollmentData structures.
        Task<IReadOnlyList<EnrollmentData>> GetEnrollmentDataAsync(CancellationToken token);
                
        Task AddEnrollmentAsync(string Grade, string SchoolYear, string FirstName, string LastName, CancellationToken token);

    }
}
