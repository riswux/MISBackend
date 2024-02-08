using Microsoft.AspNetCore.Identity;
using MISBackend.Data;

namespace MISBackend.Repository
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

        public Tuple<int, string, Data.Entity.Doctor?> Register(Model.Payload.DoctorRegisterModel doctor)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Email == doctor.Email || (doctor.Phone.Length >= 1 && o.Phone == doctor.Phone));
                if (exists != null)
                {
                    return new Tuple<int, string, Data.Entity.Doctor?>(400, "Doctor is available", exists);
                }
                else
                {
                    Data.Entity.Doctor data = new Data.Entity.Doctor
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

                    return new Tuple<int, string, Data.Entity.Doctor?>(200, "Doctor was registered", data);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Data.Entity.Doctor?>(500, "Error : " + ex.Message, null);
            }
        }

        public Tuple<int, string, Data.Entity.Doctor?> Login(Model.Payload.LoginCredentialModel doctor)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Email == doctor.Email && doctor.Password == doctor.Password);
                if (exists == null)
                {
                    return new Tuple<int, string, Data.Entity.Doctor?>(400, "Doctor is unavailable", null);
                }
                else
                {
                    return new Tuple<int, string, Data.Entity.Doctor?>(200, "Doctor was registered", exists);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Data.Entity.Doctor?>(500, "Error : " + ex.Message, null);
            }
        }

        public Tuple<int, string, Data.Entity.Doctor?> GetDoctor(Guid id)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Id == id);
                if (exists == null)
                {
                    return new Tuple<int, string, Data.Entity.Doctor?>(400, "Doctor is unavailable", null);
                }
                else
                {
                    return new Tuple<int, string, Data.Entity.Doctor?>(200, "Doctor was registered", exists);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Data.Entity.Doctor?>(500, "Error : " + ex.Message, null);
            }
        }

        public Tuple<int, string, Data.Entity.Doctor?> Edit(Guid id, Model.Payload.DoctorEditModel doctor)
        {
            try
            {
                var exists = context.Doctors.FirstOrDefault(o => o.Id == id);
                if (exists == null)
                {
                    return new Tuple<int, string, Data.Entity.Doctor?>(400, "Doctor is unavailable", exists);
                }
                else
                {
                    var exists2 = context.Doctors.FirstOrDefault(o => o.Id != id && (o.Email == doctor.Email || (doctor.Phone.Length >= 1 && o.Phone == doctor.Phone)));

                    if (exists2 != null)
                    {
                        return new Tuple<int, string, Data.Entity.Doctor?>(400, "Doctor is unavailable", exists2);
                    }
                    else
                    {
                        Data.Entity.Doctor data = new Data.Entity.Doctor
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
                        return new Tuple<int, string, Data.Entity.Doctor?>(200, "Doctor was registered", data);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, ex);
                return new Tuple<int, string, Data.Entity.Doctor?>(500, "Error : " + ex.Message, null);
            }
        }
    }
}
