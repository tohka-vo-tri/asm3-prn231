using DataAccess.Models;
using LayoutAPI.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LayoutAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : Controller
    {
        private readonly PersonRepository _personRepository;
        private readonly VirusRepository _virusRepository;
        private readonly PersionVirusRepository _persionVirusRepository;
        public PersonController(PersonRepository personRepository,VirusRepository virusRepository,PersionVirusRepository persionVirusRepository)
        {
            _personRepository = personRepository;
            _virusRepository = virusRepository;
            _persionVirusRepository = persionVirusRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] AddPersonRequest.PersonDto personDto)
        {
            if (personDto == null)
            {
                return BadRequest(new { message = "Invalid person data" });
            }

            // Check if PersonId is a valid positive integer
            if (personDto.PersonId <= 0)
            {
                return BadRequest(new { message = "Person ID must be a positive integer." });
            }

            // Check if the ID already exists
            var checkId = _personRepository.GetById(personDto.PersonId); // Ensure this method is async
            if (checkId != null)
            {
                return BadRequest(new { message = "Person ID already exists." });
            }

            try
            {
                var person = new Person
                {
                    Fullname = personDto.FullName,
                    BirthDay = DateOnly.FromDateTime(personDto.BirthDay), // Use DateOnly
                    Phone = personDto.Phone,
                    PersonId = personDto.PersonId // If UserId is actually intended to be PersonId, clarify the purpose
                };

                if (personDto.Viruses != null)
                {
                    foreach (var virusDto in personDto.Viruses)
                    {
                        // Check if the virus already exists
                        var virus = await _virusRepository.GetByNameAsync(virusDto.VirusName);
                        if (virus == null)
                        {
                            // Create new virus if it doesn't exist
                            virus = new Virus
                            {
                                VirusName = virusDto.VirusName
                            };

                            // Allow the database to assign the VirusId if it's set as auto-increment
                            await _virusRepository.CreateAsync(virus); // Ensure this method is async
                        }

                        // Add the virus to the person's virus collection
                        var virusPerson = new PersonVirus
                        {
                            VirusId = virus.VirusId, // Now use the existing or newly created VirusId
                            ResistanceRate = virusDto.ResistanceRate
                        };
                        person.PersonViruses.Add(virusPerson);
                    }
                }

                // Save person to repository
                await _personRepository.CreateAsync(person); // Ensure this method is async

                var response = new
                {
                    personId = person.PersonId,
                    message = "Person and viruses added successfully"
                };

                return CreatedAtAction(nameof(GetPerson), new { id = person.PersonId }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the person.", error = ex.Message });
            }
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person =  _personRepository.GetById(id); // Assuming this method is async

            if (person == null)
            {
                return NotFound(new { message = "Person not found" });
            }

            var personViruses = await _persionVirusRepository.GetAllByIdAsync(id); // Await the async call

            if (personViruses == null || !personViruses.Any()) // Check if the list is null or empty
            {
                return NotFound(new { message = "No viruses found for the specified person." });
            }

            var response = new
            {
                personId = person.PersonId,
                fullName = person.Fullname,
                birthDay = person.BirthDay,
                phone = person.Phone,
                viruses = personViruses.Select(v => new
                {
                    virusName = v.Virus.VirusName, // Access the Virus entity to get the name
                    resistanceRate = v.ResistanceRate
                }).ToList()
            };

            return Ok(response);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPersons()
        {
            // Fetch all persons with their associated viruses
            var persons = await _personRepository.GetAllAsync(); // Ensure this method is async

            if (persons == null || !persons.Any())
            {
                return NotFound(new { message = "No persons found." });
            }

            // Create the response object with the required data
            var response = persons.Select(person => new
            {
                personId = person.PersonId,
                fullName = person.Fullname,
                birthDay = person.BirthDay,
                phone = person.Phone,
                viruses = person.PersonViruses.Select(v => new
                {
                    virusName = v.Virus.VirusName, // Access the Virus entity to get the name
                    resistanceRate = v.ResistanceRate
                }).ToList()
            }).ToList();

            return Ok(response);
        }
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdatePerson(int id, [FromBody] UpdatePersonRequest.PersonDto personDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id <= 0)
        //    {
        //        return BadRequest(new { message = "Person ID must be a positive integer." });
        //    }

        //    // Check if the person exists
        //    var person =  _personRepository.GetById(id); // Ensure this method is async
        //    if (person == null)
        //    {
        //        return NotFound(new { message = "Person not found." });
        //    }

        //    // Update person properties
        //    person.Fullname = personDto.FullName;
        //    person.BirthDay = DateOnly.FromDateTime(personDto.BirthDay);
        //    person.Phone = personDto.Phone;

        //    var existingPersonViruses = await _persionVirusRepository.GetAllByIdAsync(id);

        //    // Clear existing viruses and add new ones or update them
        //    person.PersonViruses.Clear();
        //    if (personDto.Viruses != null)
        //    {
        //        foreach (var virusDto in personDto.Viruses)
        //        {
        //            var virus = await _virusRepository.GetByNameAsync(virusDto.VirusName);
        //            if (virus == null)
        //            {
        //                virus = new Virus
        //                {
        //                    VirusName = virusDto.VirusName
        //                };
        //                var maxId = await _virusRepository.GetMaxIdAsync();
        //                virus.VirusId = (maxId ?? 0) + 1;
        //                await _virusRepository.CreateAsync(virus);
        //            }

        //            var personVirus = new PersonVirus
        //            {
        //                PersonId = person.PersonId,
        //                VirusId = virus.VirusId,
        //                ResistanceRate = virusDto.ResistanceRate
        //            };
        //            person.PersonViruses.Add(personVirus);
        //        }
        //    }

        //    await _personRepository.UpdateAsync(person);

        //    return Ok(new { message = "Person and viruses updated successfully." });
        //}

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Person ID must be a positive integer." });
            }

            // Check if the person exists
            var person =  _personRepository.GetById(id); // Ensure this method is async
            if (person == null)
            {
                return NotFound(new { message = "Person not found" });
            }

            try
            {
                // Delete associated PersonVirus entries
                var personViruses = await _persionVirusRepository.GetAllByIdAsync(id);
                if (personViruses != null && personViruses.Any())
                {
                    foreach (var personVirus in personViruses)
                    {
                        // Assuming there's a method to delete a PersonVirus entry
                        await _persionVirusRepository.RemoveAsync(personVirus);
                    }
                }

                // Delete the person
                await _personRepository.RemoveAsync(person); // Ensure this method is async

                return Ok(new { message = "Person and related viruses deleted successfully" });
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, new { message = "An error occurred while deleting the person", error = ex.Message });
            }
        }



    }
}
