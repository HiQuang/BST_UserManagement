using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BST_UserManagement
{
    public partial class Form1 : Form
    {
        private BinarySearchTree tree = new BinarySearchTree();

        public Form1()
        {
            InitializeComponent();
            cboGender.Items.AddRange(new string[] { "Nam", "Nữ", "Khác" });
            panelTree.Paint += PanelTree_Paint;
        }
        public static string HashSHA256(string rawData)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        private void btnAddUser_Click(object sender, EventArgs e)
        {
            // ... (các kiểm tra như trước)
            // Kiểm tra thông tin không được bỏ trống
            if (string.IsNullOrWhiteSpace(txtID.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text) ||
                string.IsNullOrWhiteSpace(txtFullName.Text) ||
                string.IsNullOrWhiteSpace(txtHometown.Text) ||
                string.IsNullOrWhiteSpace(txtPhone.Text) ||
                string.IsNullOrWhiteSpace(txtNationality.Text) ||
                string.IsNullOrWhiteSpace(txtAddress.Text) ||
                cboGender.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin!", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra ID phải đúng 12 số
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtID.Text, @"^\d{12}$"))
            {
                MessageBox.Show("ID phải gồm đúng 12 chữ số.", "ID không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //Kiểm tra sđt phải đủ 10 số
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtPhone.Text, @"^\d{10}$"))
            {
                MessageBox.Show("Số Điện Thoại phải gồm đúng 10 chữ số.", "Số Điện Thoại không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Tạo user mới, CreatedAt đã gán trong ctor
            User user = new User
            {
                ID = txtID.Text,
                Password = HashSHA256(txtPassword.Text),
                FullName = txtFullName.Text,
                BirthDate = dtpBirthDate.Value,
                Hometown = txtHometown.Text,
                Phone = txtPhone.Text,
                Gender = cboGender.Text,
                Nationality = txtNationality.Text,
                Address = txtAddress.Text
            };
            try
            {
                tree.Insert(user);
                MessageBox.Show("Thêm người dùng thành công.");
                ClearRegisterFields();
                panelTree.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearFields()
        {
            txtID.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            txtHometown.Clear();
            txtPhone.Clear();
            txtNationality.Clear();
            txtAddress.Clear();
            cboGender.SelectedIndex = -1;
            dtpBirthDate.Value = DateTime.Now;
        }
        private void btnLogin_Click(object sender, EventArgs e)
        {
            lstUserInfo.Items.Clear();
            string id = txtLoginID.Text.Trim();
            string pass = txtLoginPassword.Text.Trim();
            string hashedInputPassword = HashSHA256(pass); // Mã hóa trước khi so sánh

            User user = tree.Search(id);
            if (user == null)
            {
                MessageBox.Show("Không tìm thấy ID.");
                return;
            }

            if (user.IsLocked)
            {
                if (user.LockedUntil.HasValue && DateTime.Now < user.LockedUntil.Value)
                {
                    MessageBox.Show(
                        $"Tài khoản đang bị khoá đến {user.LockedUntil.Value:dd/MM/yyyy HH:mm}.",
                        "Bị khoá", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                user.IsLocked = false;
                user.FailedAttempts = 0;
                user.LockedUntil = null;
            }
            if (user.Password == hashedInputPassword)
            {
                user.FailedAttempts = 0;

                // Hiện thông tin lên listbox
                ShowUserInfo(user);

                // Thông báo và bắt DialogResult
                DialogResult result = MessageBox.Show("Đăng nhập thành công!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);

                if (result == DialogResult.OK)
                {
                    // Dọn thông tin sau khi nhấn OK
                    lstUserInfo.Items.Clear();
                    txtLoginID.Clear();
                    txtLoginPassword.Clear();
                }
            }
            else
            {
                user.FailedAttempts++;
                if (user.FailedAttempts > 5)
                {
                    user.IsLocked = true;
                    user.LockedUntil = DateTime.Now.AddHours(24);
                    MessageBox.Show(
                        "Bạn đã sai quá 5 lần. Tài khoản bị khoá trong 24 giờ.",
                        "Bị khoá", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                        $"Sai mật khẩu. Lần {user.FailedAttempts}/5 trước khi khoá.",
                        "Sai mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            panelTree.Invalidate();
        }
    

        private void PanelTree_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Font font = new Font("Arial", 7);
            Brush brush = Brushes.MistyRose;
            Pen pen = new Pen(Color.Black);

            tree.DrawTree(g, font, brush, pen, panelTree.Width);
        }

        private void ShowUserInfo(User user)
        {
            lstUserInfo.Items.Clear();
            foreach (var line in user.ToString().Split('\n'))
                lstUserInfo.Items.Add(line);
        }

        private void ClearRegisterFields()
        {
            txtID.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            dtpBirthDate.Value = DateTime.Today;
            txtHometown.Clear();
            txtPhone.Clear();
            cboGender.SelectedIndex = -1;
            txtNationality.Clear();
            txtAddress.Clear();
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }
    }
}
