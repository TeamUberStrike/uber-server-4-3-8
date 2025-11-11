function UberStrikeFacebookSocial()
{
    this.InviteFriend = function ()
    {
        FB.ui({ method: 'apprequests',
            message: 'Join me and get your adrenaline fix - it\'s time to get shootin\'! UberStrike is easily the best and most popular 3D game on Facebook. Play now!',
            redirect_uri: "https://facebook.com/uberstrikelocal",
            data : "type=invite_friend", 
        }, 
        inviteFriendCallBack);
    }
};

var UBFacebookSocial = new UberStrikeFacebookSocial();


function inviteFriendCallBack(data) {
    var message = "Enjoy our game :)";
    ConfirmationFading(message, null);
}

function FacebookPostFeed(event, params, trackingId) {
    FB.api('/me/feed', 'post', params,
    function (response) {
        if (!response || response.error) {
            //alert('Error occured');
        } else {
        }
      }
    );
}

// Still in use
function publishFBStreamPost(event, detail1) {

    var s_msg;
    var s_cap = "";
    var s_desc;
    var s_prompt;
    var s_img = "";
    var s_action = 'Challenge ' + fbFirstName;

    s_desc = fbFirstName + " is online right now, take the challenge!";
    s_img = "logo2.jpg";

    if (event == 'levelup') {
        s_cap = fbFirstName + ' (' + fbId3PlayerName + ') is now Level ' + detail1 + ' in UberStrike!';
        s_prompt = "Every level up comes with bragging rights, so share it with your friends...";

    }
    else if (event == 'windm') {
        s_cap = fbFirstName + ' (' + fbId3PlayerName + ') crushed ' + detail1 + ' people in an UberStrike Deathmatch game!';
        s_prompt = "Winners get bragging rights, so tell everyone how you feel...";

    }
    else if (event == 'winteamdm') {
        s_cap = fbFirstName + ' (' + fbId3PlayerName + ') dominated ' + detail1 + ' people in an UberStrike Team Deathmatch game!';
        s_prompt = "Winners get bragging rights, so tell everyone how you feel...";

    }
    else if (event == 'loseteamdm') {
        s_cap = fbFirstName + ' (' + fbId3PlayerName + ') was just dominated in an UberStrike Team Deathmatch game and urgently needs your help!';
        s_prompt = "Call for reinforcements!";

    }
    else if (event == 'newitem') {
        s_cap = fbFirstName + ' now has a ' + detail1 + ' to use in UberStrike!';
        s_prompt = "Time to show off your new gear!";
        s_img = md5(detail1 + "uberstrike") + ".jpg";
    }

    var params = {};
    params['message'] = s_msg;
    params['name'] = 'UberStrike!';
    params['description'] = s_desc;
    params['link'] = FacebookApplicationUrl;
    params['picture'] = imagesRoot + s_img;
    params['icon'] = imagesRoot + s_img;
    params['caption'] = s_cap;
    params['actions'] = { 'name': s_action, 'link': FacebookApplicationUrl + '&stream_event=' + event };
    FacebookPostFeed(event, params, 0);
}
