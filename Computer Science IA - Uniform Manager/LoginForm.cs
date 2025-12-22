using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace Computer_Science_IA___Uniform_Manager
{
    public partial class LoginForm : Form
    {
        private SqlConnection cn;
        private SqlCommand cmd;
        private SqlDataReader dr;
        private HashAlgorithm sha256 = SHA256.Create();
        public LoginForm()
        {
            InitializeComponent();
            cn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\faken\source\repos\Computer Science IA - Uniform Manager\Computer Science IA - Uniform Manager\DatabasestorageIA.mdf"";Integrated Security=True");
            cn.Open();
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (textBoxPassword.Text != string.Empty || textBoxUsername.Text != string.Empty)
            {
                cmd = new SqlCommand("select * from UserInfo where Username='" + textBoxUsername.Text + "' and Hashedpassword='" + Encoding.ASCII.GetString(sha256.ComputeHash(Encoding.ASCII.GetBytes(textBoxPassword.Text))) + "'", cn);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    dr.Close();
                    this.Hide();
                    UniformManager home = new UniformManager();
                    home.ShowDialog();
                }
                else
                {
                    dr.Close();
                    MessageBox.Show("No Account avilable with this username and password ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Please enter value in all field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            // checks if the user has entered all fields 
            if (textBoxConfirmPasswordCreate.Text != string.Empty || textBoxPasswordCreate.Text != string.Empty || textBoxUsernameCreate.Text != string.Empty)
            {
                // checks if both password are same
                if (textBoxConfirmPasswordCreate.Text == textBoxPasswordCreate.Text)
                {
                    if (textBoxPasswordCreate.Text.Length < 12)
                    {
                        MessageBox.Show("Password must be at least 12 characters long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    Random rand = new Random();
                    int id = rand.Next();
                    cmd = new SqlCommand("select * from UserInfo where UserId='" + id + "'", cn);
                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        dr.Close();
                        MessageBox.Show("Username Already exist please try another ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        dr.Close();
                        cmd = new SqlCommand(
                            "INSERT INTO UserInfo (FirstName, LastName, Email, HashedPassword, AccountLevel, Username) " +
                            "VALUES (@FirstName, @LastName, @Email, @HashedPassword, @AccountLevel, @Username)",
                            cn
                        );

                        cmd.Parameters.AddWithValue("FirstName", textBoxFirstNameCreate.Text);
                        cmd.Parameters.AddWithValue("LastName", textBoxLastNameCreate.Text);
                        cmd.Parameters.AddWithValue("Email", textBoxEmailCreate.Text);
                        cmd.Parameters.AddWithValue("HashedPassword", Encoding.ASCII.GetString(sha256.ComputeHash(Encoding.ASCII.GetBytes(textBoxPasswordCreate.Text))));
                        cmd.Parameters.AddWithValue("AccountLevel", 2);
                        cmd.Parameters.AddWithValue("Username", textBoxUsernameCreate.Text);
                        cmd.ExecuteNonQuery();


                        cmd = new SqlCommand("SET IDENTITY_INSERT UserInfo OFF", cn);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Your Account is created . Please login now.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter both password same ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter value in all field.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
