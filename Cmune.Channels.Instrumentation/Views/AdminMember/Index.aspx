<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width: 900px">
        <div class="niceDiv">
            <% 
                Html.RenderPartial("Partial/Register", (ViewBag.RegisterModel != null) ? (RegisterModel)ViewBag.RegisterModel : null); %>
        </div>
        <div class="niceDiv">
            <% Html.RenderPartial("Partial/ChangePassword"); %></div>
    </div>
</asp:Content>
