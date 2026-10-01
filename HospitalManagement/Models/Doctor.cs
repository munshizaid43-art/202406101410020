namespace HospitalManagement.Models
{
    // This class represents a doctor in the hospital.
    public class Doctor
    {
        // Stores the unique ID of the doctor.
        public int DoctorId { get; set; }

        // Stores the doctor's name.
        public string DoctorName { get; set; } = "";

        // Stores the doctor's medical specialization.
        public string Specialization { get; set; } = "";

        // Stores the doctor's experience in years.
        public int Experience { get; set; }

        // Stores the doctor's consultation fee.
        public decimal ConsultationFee { get; set; }
    }
}