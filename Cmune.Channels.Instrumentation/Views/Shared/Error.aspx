<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Error.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
Error
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
    <img src="<%= Url.Content("~/Content/img/panda.png")%>" alt="panda" />
        Sorry, an error occurred while processing your request.
    </h2>
    <% if (ConfigurationUtilities.ReadConfigurationManager("CmuneAPIKey", false) == "dev"){
       %>
        <% if (Model != null){ %>
        <p><b>Message</b>: <%= 
                           Model.Exception.Message %></p>
        <p><b>Source</b>: <%= 
                          Model.Exception.Source %></p>
        <p><b>TargetSite</b>:<%= 
                             Model.Exception.TargetSite %></p>
        <p><b>Data</b>:<%= 
                       Model.Exception.Data %></p>
        <p><b>Stack Trace</b>: <%= 
                               Model.Exception.StackTrace.Replace("\r\n","<br/>") %></p>
        <p><b>InnerException</b>:<%= ((Model.Exception.InnerException == null) ? "" : Model.Exception.InnerException.ToString().Replace("\r\n", "<br/>")) %></p>
        <p><b>HelpLink</b>:<%= 
                           Model.Exception.HelpLink %></p>
        <%}  else { %>
        <p>We can't have a track of the error cause HandleErrorInfo is null</p>
        <%} }
          %>
</asp:Content>
