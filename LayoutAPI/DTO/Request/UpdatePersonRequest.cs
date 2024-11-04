using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LayoutAPI.DTO.Request
{
    public class UpdatePersonRequest
    {
        public class PersonDto
        {
            [Required(ErrorMessage = "Fullname is required.")]
            [RegularExpression(@"^([A-Z][a-z]*)(\s[A-Z][a-z]*)*$", ErrorMessage = "Each word of the Fullname must begin with the capital letter.")]
            [DefaultValue("Jonathan Doe")] // Set default value for FullName
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Birthday is required.")]
            [DataType(DataType.Date)]
            [DefaultValue("1990-05-15")] // Set default value for BirthDay in the format of DateTime
            public DateTime BirthDay { get; set; }

            [Required(ErrorMessage = "Phone number is required.")]
            [RegularExpression(@"^\+84989\d{7}$", ErrorMessage = "Phone number must be in the format +84989xxxxxx.")]
            [DefaultValue("+849123456789")] // Set default value for Phone
            public string Phone { get; set; } = string.Empty;

            public List<VirusDto> Viruses { get; set; } = new List<VirusDto>();
        }

        public class VirusDto
        {
            [Required(ErrorMessage = "Virus name is required.")]
            [DefaultValue("COVID-19")] // Set default value for VirusName
            public string VirusName { get; set; } = string.Empty;

            [Range(0.0, 1.0, ErrorMessage = "Resistance Rate must be between 0 and 1.")]
            [DefaultValue(0.2)] // Set default value for ResistanceRate
            public double? ResistanceRate { get; set; }
        }
    }
}
