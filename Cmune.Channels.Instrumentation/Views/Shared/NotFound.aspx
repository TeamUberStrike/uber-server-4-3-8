<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Error.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
    <img src="<%= Url.Content("~/Content/img/panda.png")%>" alt="panda" />
        Page not found !
    </h2>
</asp:Content>
