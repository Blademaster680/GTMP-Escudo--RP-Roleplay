using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void GameLogin_Click(object sender, EventArgs e)
        {
            var hashPass = Main.GetSha256FromString(Password.Text);
            var hasUser =
                MySQL.QueryResult(
                    $"SELECT ID FROM Accounts WHERE Username='{Username.Text}' AND Password='{hashPass}'");
            if (hasUser != null)
            {
                foreach (DataRow Row in hasUser.Rows)
                {
                    ErrorMessage.Text = "Account does exist";
                    ErrorMessage.Text = Row["id"].ToString();
                    //Session("ID") = Row["id"];
                }
            }
            else
            {
                ErrorMessage.Text = "Account does not exist";
            }
        }

        private void Username_Leave(object sender, EventArgs e)
        {
            if (Username.Text == "")
            {
                Username.Text = "Please enter your username";
                Username.ForeColor = Color.Black;
            }
        }

        private void Username_Enter(object sender, EventArgs e)
        {
            if (Username.Text == "Please enter your username")
            {
                Username.Text = "";
                Username.ForeColor = Color.Black;
            }
        }
    }
}