using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BST_UserManagement
{
    public class User
    {
        public string ID { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Hometown { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }
        public string Address { get; set; }

        public DateTime CreatedAt { get; private set; }      // thời điểm tạo
        public int FailedAttempts { get; set; } = 0;         // số lần đăng nhập sai
        public bool IsLocked { get; set; } = false;          // trạng thái khoá
        public DateTime? LockedUntil { get; set; } = null;   // thời điểm tự mở khoá

        public User()
        {
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            string status = IsLocked
                ? $"(Đã khoá đến {LockedUntil:dd/MM/yyyy HH:mm})"
                : "";
            return $"\t\t\tTHÔNG TIN NGƯỜI DÙNG\nHọ tên: {FullName}\nNgày sinh: {BirthDate:dd/MM/yyyy}\n" +
                   $"Quê quán: {Hometown}\tSĐT: {Phone}\nGiới tính: {Gender}\t" +
                   $"Quốc tịch: {Nationality}\nĐịa chỉ: {Address}\n" +
                   $"Tạo lúc: {CreatedAt:dd/MM/yyyy HH:mm}\n" +
                   status;
        }
    }
}
