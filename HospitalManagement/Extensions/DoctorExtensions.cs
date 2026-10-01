using HospitalManagement.Models;

namespace HospitalManagement.Extensions
{
    // This static class contains extension methods for Doctor.
    public static class DoctorExtensions
    {
        // This method adds a fixed service charge of ₹100.
        public static decimal CalculateTotalCharge(this Doctor doctor)
        {
            // Add ₹100 service charge to the consultation fee.
            return doctor.ConsultationFee + 100;
        }
    }
}
