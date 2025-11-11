<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="Cmune.DataCenter.Common.Entities" %>
<%@ Import Namespace="Cmune.Channels.Instrumentation.Extensions" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeadContent" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/js/member.js")%>?v=<%= ViewBag.PublishVersion %>"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <div style="width:900px; margin:auto;">
        <%= Html.Partial("Partial/QuickSearch")%>
        <% if ((bool)ViewBag.IsMemberExisting) { %>
        <div id="MemberSeeContent">
            <h2 id="memberNameHeaderH2" style="text-align:left;"><%: ViewData["memberName"]%>'s Profile</h2>
            <hr />
            <br class="clear" />
            <div class="left" style="text-align: left; margin:0;">
                <div class="titleMiddle">
                    Account:</div>
                <br class="clear" />
                <form action="/Member/Edit" method="post" id="editMemberForm">
                <%= Html.Hidden("action")%>
                <%= Html.Hidden("memberCmid")%>
                <%= Html.Hidden("memberAccessIsAccountDisabled")%>
                <%= Html.Hidden("memberAccessIsChatDisabled")%>
                <%= Html.Hidden("memberName")%>
                <%= Html.Hidden("memberEmail")%>
                <%= Html.Hidden("memberEmailStatus", (int)ViewBag.EmailAddressState)%>
                <table cellpadding="4" cellspacing="4" width="405px">
                    <tr>
                        <td>
                            Email:
                        </td>
                        <td>
                            <div id="emailDiv">
                                <% if (((EmailAddressStatus)ViewBag.EmailAddressState) == EmailAddressStatus.Verified)
                                   { %>
                                   <span class="sync-good memberEmailLabel" title="Verified">
                                <% }
                                   else if (((EmailAddressStatus)ViewBag.EmailAddressState) == EmailAddressStatus.Unverified)
                                   { %>
                                   <span class="it memberEmailLabel" title="Unverified">
                                <% }
                                   else if (((EmailAddressStatus)ViewBag.EmailAddressState) == EmailAddressStatus.Invalid)
                                   { %>
                                   <span class="sync-error memberEmailLabel" title="Invalid">
                                <% } %>
                                    <%= ViewData["memberEmail"]%>
                                </span>
                                <% if (((bool)ViewData["isEmailPasswordVisible"]))
                                   { %>
                                    <a href="javascript:void(0);" onclick="selectCurrentAdmin('editMemberEmailAdminNames'); TogglePanel('#EditEmailPanel');" id="editEmailLink">
                                        <img src="<%= Url.Content("~/Content/img/b_edit.png")%>" alt="Edit email" />
                                    </a>
                                <% } %>
                                <% else
                                   { %>
                                    <a href="#" id="CompleteUserLinkButton" onclick="LoadCompleteMemberForm()"><b>Complete user</b></a>
                                <% } %>
                            </div>
                            <% if (((bool)ViewData["isEmailPasswordVisible"]))
                               { %>
                                <div id="EditEmailPanel" class="editBox" style="display:none;">
                                    New email:<br />
                                    <%= Html.TextBox("newMemberEmail")%><br />
                                    <% if ((bool)ViewData["isUsernameExplainationVisible"] == true)
                                       { %>
                                        Explain your action to the user:<br />
                                        <%= Html.TextArea("explanationMemberEmail", new { MaxLength = "500", Width = "99%", Rows = "5" })%><br />
                                        <input id="notifyMemberEmailCheckBox" type="checkbox" checked="checked" name="notifyMemberEmailCheckBox" />
                                        <label for="notifyMemberEmailCheckBox">Notify user</label>
                                        <table style="margin:15px 0 15px 0;">
                                            <tr>
                                                <td>Type:</td>
                                                <td><%= Html.DropDownList("editMemberEmailType", ViewData["editMemberEmailType"] as SelectList)%></td>
                                            </tr>
                                            <tr>
                                                <td>Action taker:</td>
                                                <td><%= Html.DropDownList("editMemberEmailAdminNames", ViewData["editMemberEmailAdminNames"] as SelectList, new { onchange = "setAdmin($('#editMemberEmailAdminNames').val(), $('#editMemberEmailAdminNames option:selected').text());" })%></td>
                                            </tr>
                                        </table>
                                    <% } %>
                                    <input id="editEmailChangeButton" type="submit" value="Change email" onclick="return ChangeMemberInfo('changeEmail');" />
                                    <input id="editEmailCancelButton" type="button" onclick="TogglePanel('#EditEmailPanel');" value="Cancel" />
                                </div>
                            <% } %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Name:
                        </td>
                        <td>
                            <div id="usernameDiv">
                                <span class="memberNameLabel">
                                    <%: ViewData["memberName"]%></span> <a href="javascript:void(0);" onclick="selectCurrentAdmin('editMemberNameAdminNames'); TogglePanel('#EditUsernamePanel');" id="editUsernameLink">
                                        <img src="<%= Url.Content("~/Content/img/b_edit.png") %>" alt="Edit username" /></a>
                            </div>
                            <div id="EditUsernamePanel" class="editBox" style="display:none;">
                                New name:<br />
                                <%= Html.TextBox("newMemberName")%><br />
                                Explain your action to the user:<br />
                                <%= Html.TextArea("explanationMemberName", new { MaxLength = "500", Width = "99%", Rows = "5" })%><br />
                                <input id="notifyMemberNameCheckBox" type="checkbox" checked="checked" name="notifyMemberNameCheckBox" /> <label for="notifyMemberNameCheckBox">Notify user</label>
                                <table style="margin:15px 0 15px 0;">
                                    <tr>
                                        <td>Type:</td>
                                        <td><%= Html.DropDownList("editMemberNameType", ViewData["editMemberNameType"] as SelectList, new { onchange = "displayNameChangeTemplate($('#editMemberNameType').val());" })%></td>
                                    </tr>
                                    <tr>
                                        <td>Action taker:</td>
                                        <td><%= Html.DropDownList("editMemberNameAdminNames", ViewData["editMemberNameAdminNames"] as SelectList, new { onchange = "setAdmin($('#editMemberNameAdminNames').val(), $('#editMemberNameAdminNames option:selected').text());" })%></td>
                                    </tr>
                                </table>
                                <input id="editMemberNameChangeButton" type="submit" value="Change member name" onclick="return ChangeMemberInfo('changeName');" />
                                <input id="editMemberNameCancelButton" type="button" onclick="TogglePanel('#EditUsernamePanel');" value="Cancel" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            Password:
                        </td>
                        <td>
                            <% if ((bool)ViewData["isEmailPasswordVisible"])
                               { %>
                            <a href="#DD" id="editPasswordLink" onclick="TogglePanel('#newPwd')">Change password</a> - 
                            <a href="#" onclick="resetPassword(); return false;">Reset password</a> <img id="resetPasswordLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
                            <% } %>
                            <% else
                               {  %>
                            <a href="#" id="CompleteUserLinkButton1" onclick="LoadCompleteMemberForm(); return false;">
                                <b>Complete user</b></a>
                            <%} %>
                            <% 
                               if (((bool)ViewData["isEmailPasswordVisible"]))
                               { %>
                            <div id="newPwd" style="display: none;">
                                <div id="pwdChange">
                                    <div>
                                        <table cellpadding="0" cellspacing="0">
                                            <tr>
                                                <td>
                                                    The new password:
                                                </td>
                                                <td>
                                                    <input type="password" id="pwd" name="pwd" class="inputRegister" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td>
                                                    Type it again:
                                                </td>
                                                <td>
                                                    <input type="password" id="pwd2" name="pwd2" class="inputRegister" />
                                                </td>
                                            </tr>
                                        </table>
                                        <span class="Clear" style="font-size: 12px;"></span>
                                        <input type="submit" id="updatePwd" validationgroup="sign" value="Change Now!" onclick="return ChangeMemberInfo('changePassword')" />
                                        <input type="button" onclick="TogglePanel('#newPwd');" value="Cancel" />
                                    </div>
                                </div>
                            </div>
                            <span style="display: none">You password has been successfully changed!</span>
                            <%} %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Account created:
                        </td>
                        <td>
                            <span>
                                <%= ((DateTime)ViewBag.AccountCreationDate).ToNiceDisplay(true)%></span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Last login:
                        </td>
                        <td>
                            <span>
                                <%= ((DateTime)ViewBag.LastLoginDate).ToNiceDisplay(true)%></span>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Ban status:
                        </td>
                        <td>
                            <div id="BanStatusUpdatePanel">
                                <span id="banStatusLabel" class="<%= ((Cmune.DataCenter.Common.Entities.BanMode)ViewData["memberAccessIsAccountDisabled"]) == Cmune.DataCenter.Common.Entities.BanMode.No?"":"error" %>">
                                    <%= ViewData["memberAccessIsAccoundDisabledBanStatus"].ToString()%>
                                </span>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Chat ban status:
                        </td>
                        <td>
                            <div id="ChatBanStatusUpdatePanel">
                                <span id="chatBanStatusLabel" class="<%= ((Cmune.DataCenter.Common.Entities.BanMode)ViewData["memberAccessIsChatDisabled"]) == Cmune.DataCenter.Common.Entities.BanMode.No?"":"error" %>">
                                    <%= ViewData["memberAccessIsChatDisabledBanStatus"].ToString()%>
                                </span>
                            </div>
                        </td>
                    </tr>
                </table>

                <div id="memberOriginContainer" style="margin:10px 0 0 0;">
                    <% if (ViewData["MemberOrigin"] != null)
                       {
                           var memberOrigin = (MemberOriginDisplay)ViewData["MemberOrigin"];  %>
                    Started to play on <span class="bold"><%= memberOrigin.Channel.ToString()%></span> from <span class="bold"><%= memberOrigin.RegionName.ToString()%></span>.

                    <% if (memberOrigin.ReferrerId != ReferrerPartnerType.None)
                       { %>
                    <br />This user was referred by <span class="bold"><%= memberOrigin.ReferrerId.ToString()%></span>.
                    <% } %>
                    <% }
                       else
                       { %>
                    We've no stats regarding where this player started to play from
                    <% } %>
                </div>

                <div style="margin:10px 0 0 0;">
                    <% foreach (var esnsHandleDisplay in (System.Collections.Generic.List<EsnsHandleDisplay>)ViewData["esnsIdentitiesDisplay"])
                       { %>
                        <%= esnsHandleDisplay.EsnsName%>  account: <a href='<%= esnsHandleDisplay.EsnsProfileLink %>' target="_blank" class="bold">
                        <%= esnsHandleDisplay.EsnsMemberId%></a><br />
                    <% } %>
                </div>

                <div style="margin:10px 0 0 0;">
                    <a id="DisableUserLinkButton" onclick="LoadBan(); return false;" href="">
                        <b>Ban member</b></a> - Spent <span id="banSpentSpan"></span> so far
                    <br /><br />
                    <a id="ModifyPointsCreditsLinkButton" onclick="LoadCreditsAndPoints(); return false;" href="">
                        <b>Add/Remove points & credits</b>
                    </a>
                    <br /><br />
                    <a id="ChatDisableUserLinkButton" onclick="LoadChatBan(); return false;" href="">
                        <b>Ban member for the chat</b>
                    </a>
                    <%--<br /><br />
                    <a id="DeleteUserLinkButton" onclick="LoadDelete(); return false;" href="">
                        <b>Delete user</b>
                    </a>--%>
                    <br /><br />
                    <a onclick="loadPrivateMessageSender(); return false;" href="#">
                        <b>Send PM to member</b>
                    </a>
                    <br /><br />
                    <% if (((string)ViewData["EnvironnementLiteral"]).ToString() != "prod")
                       { %>
                    Set level to <%= Html.TextBox("newLevel", 1, new { style = "width:15px" })%> <input id="setLevelButton" type="button" onclick="setLevel($('#memberCmid').val(), $('#newLevel').val());" value="Set" />
                    <% } %>
                    <br />
                    Member access:
                    <%= Html.DropDownList("memberAccessAccessLevel", (List<SelectListItem>)ViewData["MemberAccessDropDownList"], new { onChange = "setAccessLevel();" })%>
                </div>
                </form>
                <div>
                    <br />
                    <br />
                    <img src="<%= Url.Content("~/Content/img/creditTiny.jpg")%>" alt="Credits icon" />
                    Credits: <b><span id="memberCreditsLabel">
                        <%= ViewData["memberCredits"]%></span></b>
                    <img src="<%= Url.Content("~/Content/img/pointTiny.jpg")%>" alt="Points icon" />
                    Points: <b><span id="memberPointsLabel">
                        <%= ViewData["memberPoints"]%></span></b><br />
                    <br />
                </div>
            </div>
            <div class="right" style="width:350px">
                <% Html.RenderPartial("Partial/Statistics", ViewData); %><br />

                <div style="margin:0;">

                    <h3 style="margin:0 0 25px 0;">5 days stats history</h3>

                    <div id="dailyStatisticsContainer">
                    </div>

                </div>

            </div>

            <div class="clear"></div>

            <div id="memberNotesDiv" style="margin:25px 0 0 0;">
                <h3>Member notes</h3>

                <script type="text/javascript">
                    var memberNoteLoadingImage = $('<img />').attr('src', '<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>');
                    var memberNoteAdminNamesDdl = $('<%= (Html.DropDownList("editMemberNameAdminNames", ViewData["editMemberNameAdminNames"] as SelectList, new { style = "width:70px;" })).ToString().Replace("\r\n", "")%>');
                    var memberNoteActionTypesDdl = $('<%= (Html.DropDownList("moderationActionTypes", ViewData["moderationActionTypes"] as SelectList, new { style = "" })).ToString().Replace("\r\n", "")%>');
                </script>

                <img id="memberNotesLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />

                <div id="memberNotesSubDiv">
                    <% Html.RenderPartial("Partial/MemberNotes", (List<Cmune.DataCenter.Common.Entities.ModerationActionView>)ViewBag.MemberNotes); %>
                </div>

                <a href="#" onclick="selectCurrentAdmin('addNoteAdminNames'); $('#addNoteDiv').toggle(); return false;">Add a note</a>
                <div id="addNoteDiv" style="display:none; margin:25px 0 0 0; width:350px; background-color:#ECECEC; -moz-border-radius:35px; border-radius:35px; border:3px solid #D8D8D8; padding:10px;">
                    <form id="addNoteForm" action="" method="post">
                        Type:<br /><%= Html.DropDownList("addNoteType", ViewData["addNoteType"] as SelectList, new { onchange = "displayNoteTemplates();" })%><br /><br />
                        <p style="margin-bottom:10px;">Templates:</p>
                        <ul id="warningTemplates" style="text-align:left; margin-bottom:10px;">
                            <li><a href="#" onclick="displayNoteTemplate(1); return false;">Speed hacking</a></li>
                            <li><a href="#" onclick="displayNoteTemplate(3); return false;">xp farming</a></li>
                        </ul>
                        Description:<br /><%= Html.TextArea("addNoteDescription", new { MaxLength = "1000", Width = "99%", Rows = "5" })%><br /><br />
                        <span>Action taker:</span> <%= Html.DropDownList("addNoteAdminNames", ViewData["addNoteAdminNames"] as SelectList, new { onchange = "setAdmin($('#addNoteAdminNames').val(), $('#addNoteAdminNames option:selected').text());" })%>
                        <input type="submit" value="Add note" id="addNoteButton" onclick="addMemberNote(); return false;" />
                    </form>
                </div>
                <img id="addNoteLoadingImg" style="display:none;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            </div>

            <% Html.RenderPartial("Partial/Clan", ((ClanMemberDisplay)ViewBag.ClanMember)); %>

            <input id="viewPreviousClansButton" type="button" value="View previous clans" onclick="getPreviousClans();" style="margin:25px 0 0 0;" />

            <div id="previousClansContainer" style="margin:25px 0 0 0;">
            </div>

            <div style="margin:25px 0 0 0;">

                <h3 style="margin:0 0 30px 0;">Items currently owned</h3>

                <div id="itemsContainer">
                </div>

            </div>

            <div style="margin:25px 0 0 0;">

                <input id="getLoadoutButton" type="button" value="View loadout" onclick="getLoadout();" />

                <div id="loadoutContainer">
                </div>

            </div>

            <div style="margin:25px 0 0 0;">

                <h3 style="margin:0 0 30px 0;">Points deposits</h3>

                <div id="pointsDepositsContainer">
                </div>

            </div>

            <div style="margin:25px 0 0 0;">

                <h3 style="margin:0 0 30px 0;">Item transactions</h3>

                <div id="itemTransactionsContainer">
                </div>

            </div>

            <div style="margin:25px 0 0 0;">

                <h3>Currency deposits</h3>

                <p style="margin:30px 0 30px 0; text-align:left;">
                    This member deposited a total of <span id="totalCurrencyDepositsSpan" style="font-weight:bold;"></span>.
                </p>

                <div id="currencyDepositsContainer">
                </div>

            </div>

            <div style="margin:45px 0 0 0;">
                <h3>View linked accounts by Ips</h3>
                <input id="viewIpAndOtherAccountsButton" type="button" value="View" style="margin:15px 0 0 0;" onclick="loadIpsAndOtherAccounts()" />
                <img id="viewIpAndOtherAccountsLoadingImg" style="display:none; margin:15px 0 0 0;" src="<%= Url.Content("~/Content/img/ajax-loader-pac-man.gif")%>" alt="Loader" />
            </div>

            <div id="IpsAndOtherAccountsContainer">
            </div>

            <% Html.RenderPartial("Partial/PreviousEmails", (List<MemberPreviousEmailDisplay>)ViewBag.PreviousEmails); %>

            <% Html.RenderPartial("Partial/PreviousNames", (List<MemberPreviousNameDisplay>)ViewBag.PreviousNames); %>

            <div style="margin:45px 0 0 0;">
                <h3 style="margin:0 0 15px 0;">Contacts</h3>
                <input id="viewContactsButton" type="button" value="View" onclick="getContacts();" />
            </div>

            <div id="contactsContainer" style="margin:50px 0 0 0;">
            </div>

        </div>
        <% } else { %>
        <div style="padding-top:50px; font-weight:bold;">
        This member does not exist
        </div>
        <% } %>
    </div>

    <script type="text/javascript">
        <% if ((bool)ViewBag.IsMemberExisting) { %>
        getItems();
        getPointsDeposits();
        getItemTransactions();
        getCurrencyDeposits();
        getTotalCurrencyDeposits();
        getDailyStats();
        displayNameChangeTemplate(7);
        <% } %>
    </script>

    <!--
        <%= ViewBag.DebugInfo %>
    -->

</asp:Content>