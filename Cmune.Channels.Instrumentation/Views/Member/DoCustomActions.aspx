<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/member.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/item.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:1200px; margin:auto; text-align:left; padding-top:15px;">

        <div id="attributesItemsToMembersDiv" class="niceDiv" style="margin:30px 0 30px 0; text-align:center;">
            <h1>Attribute items to members</h1>
            <form id="itemAttributionForm" method="post" action="">
                <table style="margin:0 0 30px 0;">
                    <tr>
                        <td>Attributes items</td>
                        <td style="width:80px;">
                            <%= Html.TextBox("ItemsIDTextBox")%>
                            </td>
                        <td>during</td>
                        <td style="width:25px;">
                            <%= Html.DropDownList("DurationDropDownList") %>
                        </td>
                        <td style="width:35px">days</td>
                        <td>to members</td>
                        <td>
                            <%= Html.TextBox("CmidsTextBox")%>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td style="vertical-align:top;">
                        <span class="helper">(items ID should be separated by a comma ie: 1,2,3)</span>
                        </td>
                        <td></td>
                        <td colspan="2" style="vertical-align:top;"></td>
                        <td></td>
                        <td style="vertical-align:top;">
                        <span class="helper">(CMIDs should be separated by a comma ie: 1,2,3)</span>
                        </td>
                    </tr>
                </table>
                <div id="buttonDiv">
                    <span>Please check that you didn't invert the items Id and the Cmids</span>
                    <input type="submit" id="AttributesItemsButton" name="AttributesItemsButton" onclick="attributeItemsToMembersGo(); return false;" value="I didn't, proceed now!" />
                </div>
            </form>
            <div style="margin-top:50px; text-align:left;">
                Member name: <%= Html.TextBox("MemberNameTextBox")%> <input type="submit" id="getMemberCmidButton" name="getMemberCmidButton" onclick="getMemberCmid(); return false;" value="Get Cmid" /> <span id="memberCmidSpan"></span><br />
                Item name: <%= Html.TextBox("ItemNameTextBox")%> <input type="submit" id="getItemIdButton" name="getItemIdButton" onclick="getItemId(); return false;" value="Get item Id" /> <span id="itemIdSpan"></span>
            </div>
        </div>

        <div class="niceDiv">
            <h1>Attribute points or credits to members</h1>
            <form id="attributeCurrencyForm" method="post" action="">
                Attribute <%= Html.TextBox("currency") %> <%= Html.DropDownList("attributeCurrencyCurrencyType")%> to (<input id="allMembers" name="allMembers" type="checkbox" onclick="toggleMemberIds();" /> all) members <span id="allMembersSpan"><%= Html.TextBox("membersHandle") %> (comma separated)</span> on <%= Html.DropDownList("attributeCurrencyEsnsType")%> (Use None to search by Cmid) <input type="submit" id="attributeCurrencyButton" onclick="attributeCurrency(); return false;" value="Do it" /><img id="attributeCurrencyLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" /><span id="attributeCurrencyResult"></span>
            </form>
        </div>

        <div class="niceDiv">
            <h1>Ban multiple members</h1>
            <form id="banCmidsForm" method="post" action="">
                Ban permanently Cmids: <%= Html.TextBox("cmidsData")%> (comma separated) for cheating (Warn me if the user spent more than $<%= Html.TextBox("UsdDepositsThresholdData", ViewData["UsdDepositsThresholdData"], new { style = "width:20px;" })%>) <input type="submit" onclick="banPermanently(); return false;" value="Permanently ban" />
            </form>
            <div id="banCmidsContainer">
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function toggleMemberIds() {
            if ($('#allMembers').attr('checked')) {
                $('#allMembersSpan').hide();
            }
            else {
                $('#allMembersSpan').show();
            }
        }
    </script>
</asp:Content>