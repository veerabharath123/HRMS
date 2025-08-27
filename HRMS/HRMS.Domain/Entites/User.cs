using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRMS.Domain.Common;
using static HRMS.Domain.Records.UserRecords;

namespace HRMS.Domain.Entites
{
    public class User : AuditableWithBaseEntity<int>
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public byte[] Password { get; private set; }
        public byte[] HashSalt { get; private set; }
        public int? Otp { get; private set; }
        public DateTime? OtpDateTime { get; private set; }
        public bool FirstTime { get; private set; }
        public bool IsActive { get; private set; }

        public void Add(UserAddOrUpdateRec rec)
        {
            Update(rec);
            FirstTime = false;
        }
        public void Update(UserAddOrUpdateRec rec)
        {
            FirstName = rec.FirstName;
            LastName = rec.LastName;
            UserName = rec.UserName;
            Email = rec.Email;
            IsActive = true;
        }
        public void SetPassword(byte[] password, byte[] hash)
        {
            Password = password;
            HashSalt = hash;
        }
    }
}
