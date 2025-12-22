namespace Computer_Science_IA___Uniform_Manager
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            buttonLogin = new Button();
            textBoxPassword = new TextBox();
            textBoxUsername = new TextBox();
            labelPassword = new Label();
            labelUsername = new Label();
            groupBox2 = new GroupBox();
            textBoxLastNameCreate = new TextBox();
            textBoxFirstNameCreate = new TextBox();
            labelLastNameCreate = new Label();
            labelFirstNameCreate = new Label();
            textBoxConfirmPasswordCreate = new TextBox();
            labelConfirmPasswordCreate = new Label();
            buttonCreate = new Button();
            textBoxPasswordCreate = new TextBox();
            textBoxUsernameCreate = new TextBox();
            labelPasswordCreate = new Label();
            labelUsernameCreate = new Label();
            textBoxEmailCreate = new TextBox();
            labelEmailCreate = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonLogin);
            groupBox1.Controls.Add(textBoxPassword);
            groupBox1.Controls.Add(textBoxUsername);
            groupBox1.Controls.Add(labelPassword);
            groupBox1.Controls.Add(labelUsername);
            groupBox1.Location = new Point(48, 43);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(354, 326);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Login";
            // 
            // buttonLogin
            // 
            buttonLogin.Location = new Point(88, 203);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(146, 34);
            buttonLogin.TabIndex = 3;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = true;
            buttonLogin.Click += buttonLogin_Click;
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(164, 123);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.Size = new Size(167, 31);
            textBoxPassword.TabIndex = 2;
            // 
            // textBoxUsername
            // 
            textBoxUsername.Location = new Point(164, 69);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(167, 31);
            textBoxUsername.TabIndex = 1;
            // 
            // labelPassword
            // 
            labelPassword.AutoSize = true;
            labelPassword.Location = new Point(43, 126);
            labelPassword.Name = "labelPassword";
            labelPassword.Size = new Size(87, 25);
            labelPassword.TabIndex = 1;
            labelPassword.Text = "Password";
            // 
            // labelUsername
            // 
            labelUsername.AutoSize = true;
            labelUsername.Location = new Point(43, 72);
            labelUsername.Name = "labelUsername";
            labelUsername.Size = new Size(91, 25);
            labelUsername.TabIndex = 0;
            labelUsername.Text = "Username";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(textBoxEmailCreate);
            groupBox2.Controls.Add(labelEmailCreate);
            groupBox2.Controls.Add(textBoxLastNameCreate);
            groupBox2.Controls.Add(textBoxFirstNameCreate);
            groupBox2.Controls.Add(labelLastNameCreate);
            groupBox2.Controls.Add(labelFirstNameCreate);
            groupBox2.Controls.Add(textBoxConfirmPasswordCreate);
            groupBox2.Controls.Add(labelConfirmPasswordCreate);
            groupBox2.Controls.Add(buttonCreate);
            groupBox2.Controls.Add(textBoxPasswordCreate);
            groupBox2.Controls.Add(textBoxUsernameCreate);
            groupBox2.Controls.Add(labelPasswordCreate);
            groupBox2.Controls.Add(labelUsernameCreate);
            groupBox2.Location = new Point(421, 43);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(458, 414);
            groupBox2.TabIndex = 5;
            groupBox2.TabStop = false;
            groupBox2.Text = "Create Account";
            // 
            // textBoxLastNameCreate
            // 
            textBoxLastNameCreate.Location = new Point(213, 98);
            textBoxLastNameCreate.Name = "textBoxLastNameCreate";
            textBoxLastNameCreate.Size = new Size(167, 31);
            textBoxLastNameCreate.TabIndex = 5;
            // 
            // textBoxFirstNameCreate
            // 
            textBoxFirstNameCreate.Location = new Point(213, 52);
            textBoxFirstNameCreate.Name = "textBoxFirstNameCreate";
            textBoxFirstNameCreate.Size = new Size(167, 31);
            textBoxFirstNameCreate.TabIndex = 4;
            // 
            // labelLastNameCreate
            // 
            labelLastNameCreate.AutoSize = true;
            labelLastNameCreate.Location = new Point(36, 101);
            labelLastNameCreate.Name = "labelLastNameCreate";
            labelLastNameCreate.Size = new Size(95, 25);
            labelLastNameCreate.TabIndex = 8;
            labelLastNameCreate.Text = "Last Name";
            // 
            // labelFirstNameCreate
            // 
            labelFirstNameCreate.AutoSize = true;
            labelFirstNameCreate.Location = new Point(36, 55);
            labelFirstNameCreate.Name = "labelFirstNameCreate";
            labelFirstNameCreate.Size = new Size(97, 25);
            labelFirstNameCreate.TabIndex = 7;
            labelFirstNameCreate.Text = "First Name";
            // 
            // textBoxConfirmPasswordCreate
            // 
            textBoxConfirmPasswordCreate.Location = new Point(213, 276);
            textBoxConfirmPasswordCreate.Name = "textBoxConfirmPasswordCreate";
            textBoxConfirmPasswordCreate.PasswordChar = '*';
            textBoxConfirmPasswordCreate.Size = new Size(167, 31);
            textBoxConfirmPasswordCreate.TabIndex = 9;
            // 
            // labelConfirmPasswordCreate
            // 
            labelConfirmPasswordCreate.AutoSize = true;
            labelConfirmPasswordCreate.Location = new Point(36, 282);
            labelConfirmPasswordCreate.Name = "labelConfirmPasswordCreate";
            labelConfirmPasswordCreate.Size = new Size(156, 25);
            labelConfirmPasswordCreate.TabIndex = 5;
            labelConfirmPasswordCreate.Text = "Confirm Password";
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new Point(140, 325);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(146, 34);
            buttonCreate.TabIndex = 10;
            buttonCreate.Text = "Create Account";
            buttonCreate.UseVisualStyleBackColor = true;
            buttonCreate.Click += buttonCreate_Click;
            // 
            // textBoxPasswordCreate
            // 
            textBoxPasswordCreate.Location = new Point(213, 226);
            textBoxPasswordCreate.Name = "textBoxPasswordCreate";
            textBoxPasswordCreate.PasswordChar = '*';
            textBoxPasswordCreate.Size = new Size(167, 31);
            textBoxPasswordCreate.TabIndex = 8;
            // 
            // textBoxUsernameCreate
            // 
            textBoxUsernameCreate.Location = new Point(213, 139);
            textBoxUsernameCreate.Name = "textBoxUsernameCreate";
            textBoxUsernameCreate.Size = new Size(167, 31);
            textBoxUsernameCreate.TabIndex = 6;
            // 
            // labelPasswordCreate
            // 
            labelPasswordCreate.AutoSize = true;
            labelPasswordCreate.Location = new Point(36, 232);
            labelPasswordCreate.Name = "labelPasswordCreate";
            labelPasswordCreate.Size = new Size(87, 25);
            labelPasswordCreate.TabIndex = 1;
            labelPasswordCreate.Text = "Password";
            // 
            // labelUsernameCreate
            // 
            labelUsernameCreate.AutoSize = true;
            labelUsernameCreate.Location = new Point(36, 142);
            labelUsernameCreate.Name = "labelUsernameCreate";
            labelUsernameCreate.Size = new Size(91, 25);
            labelUsernameCreate.TabIndex = 0;
            labelUsernameCreate.Text = "Username";
            // 
            // textBoxEmailCreate
            // 
            textBoxEmailCreate.Location = new Point(213, 180);
            textBoxEmailCreate.Name = "textBoxEmailCreate";
            textBoxEmailCreate.Size = new Size(167, 31);
            textBoxEmailCreate.TabIndex = 7;
            // 
            // labelEmailCreate
            // 
            labelEmailCreate.AutoSize = true;
            labelEmailCreate.Location = new Point(36, 183);
            labelEmailCreate.Name = "labelEmailCreate";
            labelEmailCreate.Size = new Size(54, 25);
            labelEmailCreate.TabIndex = 11;
            labelEmailCreate.Text = "Email";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(971, 549);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "LoginForm";
            Text = "LoginForm";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label labelPassword;
        private Label labelUsername;
        private Button buttonLogin;
        private TextBox textBoxPassword;
        private TextBox textBoxUsername;
        private GroupBox groupBox2;
        private TextBox textBoxConfirmPasswordCreate;
        private Label labelConfirmPasswordCreate;
        private Button buttonCreate;
        private TextBox textBoxPasswordCreate;
        private TextBox textBoxUsernameCreate;
        private Label labelPasswordCreate;
        private Label labelUsernameCreate;
        private TextBox textBoxLastNameCreate;
        private TextBox textBoxFirstNameCreate;
        private Label labelLastNameCreate;
        private Label labelFirstNameCreate;
        private TextBox textBoxEmailCreate;
        private Label labelEmailCreate;
    }
}