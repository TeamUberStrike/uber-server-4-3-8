// UltimatepayCall.js

var ultimatePayParams = {
    "method": "StartOrderFrontEnd",
    "display": "Lightbox",
    "sn": "CMUN",
    "userid": "",
    "currency": "",
    "sepamount": "",
    "sepamountdesc": "",
    "mirror": "MirrorValue",
    "email": "",
    "fname": "",
    "lname": "",
    "addr1": "",
    "city": "",
    "st": "",
    "zip": "",
    "country": "",
    "language": "",
    "phone1": "",
    "xdurl": "",
    "hash": "",
    "merchtrans": ""
};

var ultimatePayIsOnPlayPage = true;

function BuyUltimatePayHelper(userid, currency, amount, amountdesc, hash, xdurl, developerId, merchtrans, isOnPlayPage, paymentId) {
   
    try {
        isOnPlayPage = typeof (isOnPlayPage) != 'undefined' ? isOnPlayPage : true;
        paymentId = typeof (paymentId) != 'undefined' ? paymentId : '';

        ultimatePayParams["userid"] = userid;
        ultimatePayParams["currency"] = currency;
        ultimatePayParams["sepamount"] = amount;
        ultimatePayParams["hash"] = hash;
        ultimatePayParams["sepamountdesc"] = amount + '%24+pack';
        ultimatePayParams["xdurl"] = xdurl;
        ultimatePayParams["developerid"] = developerId;
        ultimatePayIsOnPlayPage = isOnPlayPage;
        ultimatePayParams["paymentid"] = paymentId;
        ultimatePayParams["merchtrans"] = merchtrans;
        ultimatePayParams["amountdesc"] = amountdesc;

        ulp.ultimatePay = true;

        ulp.displayUltimatePay();
    }
    catch (error) {
        callWebService('Stats/ReportIssue?type=0&data=DisplayPayment@@@' + navigator.userAgent);
    }
}

if (typeof ulp !== 'undefined') {
    //    try {
    //ulp.on('paymentSuccess', PlaySpanVY.ultimatePaySuccess);
    ulp.on('closeLB', function (data) {
        /* work only on portal */
        if (channelType == 0) {
            LoadSelectPaymentStep();
        }
        else if (channelType == 4) {
            loadThankYou();
        }

    });
    //    }
    //    catch (error) {
    //        callWebService('Stats/ReportIssue?type=0&data=PageLoad@@@' + navigator.userAgent);
    //    }
}
