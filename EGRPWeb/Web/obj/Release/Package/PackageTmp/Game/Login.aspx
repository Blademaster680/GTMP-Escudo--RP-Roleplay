<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Web.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../css/login.css" rel="stylesheet" />
    <link href="../css/materialize.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
         <center>
            <div class="section"></div>
            <div class="container">
                <div class="z-depth-5 grey lighten-4 row" style="display: inline-block; padding: 32px 48px 0px 48px; border: 1px solid #EEE; width: 400px;">
                        <img src="../images/EscudoLogo.png" style="width:250px" class="circle responsive-img" />
                        <br />
                        <div class='row'>
                            <div class='col s12'>
                            </div>
                        </div>
                        <div asp-validation-summary="ModelOnly"></div>
                        <div class='row'>
                            <div class='input-field col s12'>
                                <asp:Label ID="ErrorMessage" runat="server" Text=""></asp:Label>
                                <div>
                                    <asp:TextBox ID="Username" runat="server" placeholder="Please enter username"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class='row'>
                            <div class='input-field col s12'>
                                <asp:TextBox ID="Password" runat="server" TextMode="Password" placeholder="Please enter password"></asp:TextBox>
                            </div>
                            <label style='float: right;'>
                                <a asp-controller="Game" asp-action="ForgotPassword" class='red-text'><b>Forgot Password?</b></a>
                            </label>
                        </div>
                        <br />
                        <center>
                            <div class='row'>
                                <asp:Button ID="GameLogin" runat="server" Text="Login" class='col s12 btn btn-large waves-effect blue' OnClick="GameLogin_Click"/>
                            </div>
                            <br />

                            <a>Create account</a>
                            <br />
                            <br />
                        </center>
                    </form>
                </div>
            </div>
    
        </center>
    </form>

    <script>
        document.getElementById("GameLogin").onclick = function() {
          var username = document.getElementById("Username").value;
          var password = document.getElementById("Password").value;
          resourceCall("Login", username, password);
        };
    </script>

</body>
</html>
