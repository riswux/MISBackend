using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using MISBackend.DAL.Entity;
using MISBackend.DAL;
using MISBackend.DAL.Model.Payload;

namespace MISBackend.BLL.Services
{
    public class RepMisBack
    {
        private readonly MISDbContext context;
        private readonly ILogger<RepMisBack> logger;

        public RepMisBack(MISDbContext context, ILogger<RepMisBack> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        public Tuple<int, string, Doctor?> Register(DoctorRegisterModel doctor)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Email == doctor.Email || (doctor.Phone.Length >= 1 && o.Phone == doctor.Phone));
                if (exists != null)
                {
                    return new Tuple<int, string, Doctor?>(400, "Doctor is available", exists);
                }
                else
                {
                    Doctor data = new Doctor
                    {
                        Id = Guid.NewGuid(),
                        Email = doctor.Email,
                        Phone = doctor.Phone,
                        Birthday = doctor.Birthday,
                        Gender = doctor.Gender,
                        Name = doctor.Name,
                        Password = doctor.Password,
                        Speciality = doctor.Speciality,
                        CreateTime = DateTime.Now
                    };
                    context.Doctors.Add(data);
                    context.SaveChanges();

                    return new Tuple<int, string, Doctor?>(200, "Doctor was registered", data);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Doctor?>(500, "Error : " + ex.Message, null);
            }
        }

        public Tuple<int, string, Doctor?> Login(LoginCredentialModel doctor)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Email == doctor.Email && doctor.Password == doctor.Password);
                if (exists == null)
                {
                    return new Tuple<int, string, Doctor?>(400, "Doctor is unavailable", null);
                }
                else
                {
                    return new Tuple<int, string, Doctor?>(200, "Doctor was registered", exists);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Doctor?>(500, "Error : " + ex.Message, null);
            }
        }

        public Tuple<int, string, Doctor?> GetDoctor(Guid id)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Id == id);
                if (exists == null)
                {
                    return new Tuple<int, string, Doctor?>(400, "Doctor is unavailable", null);
                }
                else
                {
                    return new Tuple<int, string, Doctor?>(200, "Doctor was registered", exists);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Doctor?>(500, "Error : " + ex.Message, null);
            }
        }

        public Tuple<int, string, Doctor?> Edit(Guid id, DoctorEditModel doctor)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Id == id);
                if (exists == null)
                {
                    return new Tuple<int, string, Doctor?>(400, "Doctor is unavailable", exists);
                }
                else
                {
                    var exists2 = context.Doctors.FirstOrDefault(o => o.Id != id && (o.Email == doctor.Email || (doctor.Phone.Length >= 1 && o.Phone == doctor.Phone)));

                    if (exists2 != null)
                    {
                        return new Tuple<int, string, Doctor?>(400, "Doctor is unavailable", exists2);
                    }
                    else
                    {
                        Doctor data = new Doctor
                        {
                            Id = id,
                            Email = doctor.Email,
                            Phone = doctor.Phone,
                            Birthday = doctor.Birthday,
                            Gender = doctor.Gender,
                            Name = doctor.Name,
                            Password = exists.Password,
                            Speciality = exists.Speciality,
                            CreateTime = DateTime.Now
                        };

                        context.Doctors.Entry(exists).CurrentValues.SetValues(data);
                        context.SaveChanges();
                        return new Tuple<int, string, Doctor?>(200, "Doctor was registered", data);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Doctor?>(500, "Error : " + ex.Message, null);
            }
        }
    }
}
