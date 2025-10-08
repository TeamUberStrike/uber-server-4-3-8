<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/member.js")%>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
<div>
    <h1>Manage members access</h1>
    <form action="<%= Url.Action("ManageMemberAccess", "Member") %>" method="post" >
        <table>
            <tr>
                <%= Html.TableTd("Name") %>
                <%= Html.TableTd("Access level") %>
            </tr>
            <% 
                int i = 0;
                foreach (var item in (List<MemberAccessDisplay>)ViewData["memberAccessLevelsDisplay"])
                {
            %>
            <tr>
                <td>
                    <a href="<%= Url.Action("See", "Member")%>?cmid=<%= item.Cmid.ToString() %>" target="_blank">
                        <%= item.Name %></a>
                </td>
                <td>
                    <% 
                    var accessLevelList = ((List<SelectListItem>)ViewData["accessLevelList"]);
                    accessLevelList.ForEach(d => d.Selected = false);
                    accessLevelList.Find(d => d.Value == ((int)item.AccessLevel).ToString()).Selected = true;   
                    %>
                    <%= Html.DropDownList("accessLevelList"+i, accessLevelList) %>
                    <%= Html.Hidden("cmidHiddenField"+i.ToString(), item.Cmid.ToString()) %>
                    <%= Html.Hidden("accessLevelHiddenField" + i.ToString(), item.Name.ToString()) %>
                </td>
            </tr>
            <% 
                    i++;
                } %>
        </table>
        <%= Html.Hidden("numberOfMembers", i)%>
        <input type="submit" value="Modify levels" id="modifyAccessLevelsButton" />
    </form>
</div>
</asp:Content>
