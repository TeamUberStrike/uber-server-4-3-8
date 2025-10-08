<%@ Page Language="C#" MasterPageFile="~/Views/Shared/LogOn.Master" Inherits="System.Web.Mvc.ViewPage<Cmune.Channels.Instrumentation.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Cmune - Sign In
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div id="loginContent">
        <br />
        <h2>
            Sign in</h2>
        <br />
        <br />
        <% using (Html.BeginForm("LogIn", "Account"))
           { %>
        <%: Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
        <br />
        <div>
            <%: Html.Hidden("returnUrl", (string) ViewBag.ReturnUrl) %>
            <div class="editor-label left">
                <%: Html.LabelFor(m => m.UserName) %>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(m => m.UserName) %><br />
                <%: Html.ValidationMessageFor(m => m.UserName) %>
            </div>
            <br />
            <div class="editor-label left">
                <%: Html.LabelFor(m => m.Password) %>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.Password) %><br />
                <%: Html.ValidationMessageFor(m => m.Password) %>
            </div>
            <br />
            <div class="editor-label left">
                &nbsp;
            </div>
            <div class="editor-field">
                <%: Html.CheckBoxFor(m => m.RememberMe) %>
                <%: Html.LabelFor(m => m.RememberMe) %>
            </div>
            <br />
            <div class="editor-label left">
                &nbsp;
            </div>
            <div class="editor-field">
                <input type="submit" value="Sign In" />
            </div>
        </div>
        <% } %>
    </div>
</asp:Content>
