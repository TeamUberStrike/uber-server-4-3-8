

namespace Cmune.Realtime.Common.Defines
{
    public static class CountryCodes
    {
        public static string GetName(Codes code)
        {
            if ((int)code < _names.Length)
                return _names[(int)code];
            else
                return "Unknown";
        }

        public enum Codes
        {
            None = 0,
            AF,
            AL,
            DZ,
            AS,
            AD,
            AO,
            AI,
            AQ,
            AG,
            AR,
            AM,
            AW,
            AU,
            AT,
            AZ,
            BS,
            BH,
            BD,
            BB,
            BY,
            BE,
            BZ,
            BJ,
            BM,
            BT,
            BO,
            BA,
            BW,
            BV,
            BR,
            IO,
            BN,
            BG,
            BF,
            BI,
            KH,
            CM,
            CA,
            CV,
            KY,
            CF,
            TD,
            CL,
            CN,
            CX,
            CC,
            CO,
            KM,
            CG,
            CD,
            CK,
            CR,
            CI,
            HR,
            CY,
            CZ,
            DK,
            DJ,
            DM,
            DO,
            TL,
            EC,
            EG,
            SV,
            GQ,
            ER,
            EE,
            ET,
            FK,
            FO,
            FJ,
            FI,
            FR,
            FX,
            GF,
            PF,
            TF,
            GA,
            GM,
            GE,
            DE,
            GH,
            GI,
            GR,
            GL,
            GD,
            GP,
            GU,
            GT,
            GN,
            GW,
            GY,
            HT,
            HM,
            HN,
            HK,
            HU,
            IS,
            IN,
            ID,
            IQ,
            IE,
            IL,
            IT,
            JM,
            JP,
            JO,
            KZ,
            KE,
            KI,
            KW,
            KG,
            LA,
            LV,
            LB,
            LS,
            LR,
            LY,
            LI,
            LT,
            LU,
            MO,
            MK,
            MG,
            MW,
            MY,
            MV,
            ML,
            MT,
            MH,
            MQ,
            MR,
            MU,
            YT,
            MX,
            FM,
            MD,
            MC,
            MN,
            MS,
            MA,
            MZ,
            NA,
            NR,
            NP,
            NL,
            AN,
            NC,
            NZ,
            NI,
            NE,
            NG,
            NU,
            NF,
            MP,
            NO,
            OM,
            PK,
            PW,
            PS,
            PA,
            PG,
            PY,
            PE,
            PH,
            PN,
            PL,
            PT,
            PR,
            QA,
            RE,
            RO,
            RU,
            RW,
            KN,
            LC,
            VC,
            WS,
            SM,
            ST,
            SA,
            SN,
            CS,
            SC,
            SL,
            SG,
            SK,
            SI,
            SB,
            SO,
            ZA,
            GS,
            KR,
            ES,
            LK,
            SH,
            PM,
            SR,
            SJ,
            SZ,
            SE,
            CH,
            TW,
            TJ,
            TZ,
            TH,
            TG,
            TK,
            TO,
            TT,
            TN,
            TR,
            TM,
            TC,
            TV,
            UG,
            UA,
            AE,
            GB,
            US,
            UM,
            UY,
            UZ,
            VU,
            VA,
            VE,
            VN,
            VG,
            VI,
            WF,
            EH,
            YE,
            ZM,
            ZW,
        }

        private static readonly string[] _names = new string[] {
               "None",
               "Afghanistan", 
               "Albania", 
               "Algeria", 
               "American Samoa", 
               "Andorra", 
               "Angola", 
               "Anguilla", 
               "Antarctica", 
               "Antigua and Barbuda", 
               "Argentina", 
               "Armenia", 
               "Aruba", 
               "Australia", 
               "Austria", 
               "Azerbaijan", 
               "Bahamas", 
               "Bahrain", 
               "Bangladesh", 
               "Barbados", 
               "Belarus", 
               "Belgium", 
               "Belize", 
               "Benin", 
               "Bermuda", 
               "Bhutan", 
               "Bolivia", 
               "Bosnia and Herzegovina", 
               "Botswana", 
               "Bouvet Island", 
               "Brazil", 
               "British Indian Ocean Territory", 
               "Brunei Darussalam", 
               "Bulgaria", 
               "Burkina Faso", 
               "Burundi", 
               "Cambodia", 
               "Cameroon", 
               "Canada", 
               "Cape Verde", 
               "Cayman Islands", 
               "Central African Republic", 
               "Chad", 
               "Chile", 
               "China", 
               "Christmas Island", 
               "Cocos (Keeling) Islands", 
               "Colombia", 
               "Comoros", 
               "Congo", 
               "Congo, Democratic Republic", 
               "Cook Islands", 
               "Costa Rica", 
               "Cote d'Ivoire CI ",
               "Croatia", 
               "Cyprus", 
               "Czech Republic", 
               "Denmark", 
               "Djibouti", 
               "Dominica", 
               "Dominican Republic", 
               "East Timor", 
               "Ecuador", 
               "Egypt", 
               "El Salvador", 
               "Equatorial Guinea", 
               "Eritrea", 
               "Estonia", 
               "Ethiopia", 
               "Falkland Islands (Malvinas)", 
               "Faroe Islands", 
               "Fiji", 
               "Finland", 
               "France", 
               "France, Metropolitan", 
               "French Guiana", 
               "French Polynesia", 
               "French Southern Territories", 
               "Gabon", 
               "Gambia", 
               "Georgia", 
               "Germany", 
               "Ghana", 
               "Gibraltar", 
               "Greece", 
               "Greenland", 
               "Grenada", 
               "Guadeloupe", 
               "Guam", 
               "Guatemala", 
               "Guinea", 
               "Guinea-Bissau", 
               "Guyana", 
               "Haiti", 
               "Heard and McDonald Islands", 
               "Honduras", 
               "Hong Kong", 
               "Hungary", 
               "Iceland", 
               "India", 
               "Indonesia", 
               "Iraq", 
               "Ireland", 
               "Israel", 
               "Italy", 
               "Jamaica", 
               "Japan", 
               "Jordan", 
               "Kazakhstan", 
               "Kenya", 
               "Kiribati", 
               "Kuwait", 
               "Kyrgyzstan", 
               "Lao People's Democratic Republic", 
               "Latvia", 
               "Lebanon", 
               "Lesotho", 
               "Liberia", 
               "Libya", 
               "Liechtenstein", 
               "Lithuania", 
               "Luxembourg", 
               "Macau", 
               "Macedonia", 
               "Madagascar", 
               "Malawi", 
               "Malaysia", 
               "Maldives", 
               "Mali", 
               "Malta", 
               "Marshall Islands", 
               "Martinique", 
               "Mauritania", 
               "Mauritius", 
               "Mayotte", 
               "Mexico", 
               "Micronesia", 
               "Moldova", 
               "Monaco", 
               "Mongolia", 
               "Montserrat", 
               "Morocco", 
               "Mozambique", 
               "Namibia", 
               "Nauru", 
               "Nepal", 
               "Netherlands", 
               "Netherlands Antilles", 
               "New Caledonia", 
               "New Zealand", 
               "Nicaragua", 
               "Niger", 
               "Nigeria", 
               "Niue", 
               "Norfolk Island", 
               "Northern Mariana Islands", 
               "Norway", 
               "Oman", 
               "Pakistan", 
               "Palau", 
               "Palestinian Territory", 
               "Panama", 
               "Papua New Guinea", 
               "Paraguay", 
               "Peru", 
               "Philippines", 
               "Pitcairn", 
               "Poland", 
               "Portugal", 
               "Puerto Rico", 
               "Qatar", 
               "Reunion", 
               "Romania", 
               "Russian Federation", 
               "Rwanda", 
               "Saint Kitts and Nevis", 
               "Saint Lucia", 
               "Saint Vincent and the Grenadines", 
               "Samoa", 
               "San Marino", 
               "Sao Tome and Principe", 
               "Saudi Arabia", 
               "Senegal", 
               "Serbia and Montenegro", 
               "Seychelles", 
               "Sierra Leone", 
               "Singapore", 
               "Slovakia", 
               "Slovenia", 
               "Solomon Islands", 
               "Somalia", 
               "South Africa", 
               "South Georgia and The South Sandwich Islands", 
               "South Korea", 
               "Spain", 
               "Sri Lanka", 
               "St. Helena", 
               "St. Pierre and Miquelon", 
               "Suriname", 
               "Svalbard and Jan Mayen Islands", 
               "Swaziland", 
               "Sweden", 
               "Switzerland", 
               "Taiwan", 
               "Tajikistan", 
               "Tanzania", 
               "Thailand", 
               "Togo", 
               "Tokelau", 
               "Tonga", 
               "Trinidad and Tobago", 
               "Tunisia", 
               "Turkey", 
               "Turkmenistan", 
               "Turks and Caicos Islands", 
               "Tuvalu", 
               "Uganda", 
               "Ukraine", 
               "United Arab Emirates", 
               "United Kingdom", 
               "United States", 
               "United States Minor Outlying Islands", 
               "Uruguay", 
               "Uzbekistan", 
               "Vanuatu", 
               "Vatican", 
               "Venezuela", 
               "Viet Nam", 
               "Virgin Islands (British)", 
               "Virgin Islands (U.S.)", 
               "Wallis and Futuna Islands", 
               "Western Sahara", 
               "Yemen", 
               "Zambia", 
               "Zimbabwe"
    };

        /*
         * Afghanistan AF 
 Albania AL 
 Algeria DZ 
 American Samoa AS 
 Andorra AD 
 Angola AO 
 Anguilla AI 
 Antarctica AQ 
 Antigua and Barbuda AG 
 Argentina AR 
 Armenia AM 
 Aruba AW 
 Australia AU 
 Austria AT 
 Azerbaijan AZ 
 Bahamas BS 
 Bahrain BH 
 Bangladesh BD 
 Barbados BB 
 Belarus BY 
 Belgium BE 
 Belize BZ 
 Benin BJ 
 Bermuda BM 
 Bhutan BT 
 Bolivia BO 
 Bosnia and Herzegovina BA 
 Botswana BW 
 Bouvet Island BV 
 Brazil BR 
 British Indian Ocean Territory IO 
 Brunei Darussalam BN 
 Bulgaria BG 
 Burkina Faso BF 
 Burundi BI 
 Cambodia KH 
 Cameroon CM 
 Canada CA 
 Cape Verde CV 
 Cayman Islands KY 
 Central African Republic CF 
 Chad TD 
 Chile CL 
 China CN 
 Christmas Island CX 
 Cocos (Keeling) Islands CC 
 Colombia CO 
 Comoros KM 
 Congo CG 
 Congo, Democratic Republic CD 
 Cook Islands CK 
 Costa Rica CR 
 Cote d'Ivoire CI 
 Croatia HR 
 Cyprus CY 
 Czech Republic CZ 
 Denmark DK 
 Djibouti DJ 
 Dominica DM 
 Dominican Republic DO 
 East Timor TL 
 Ecuador EC 
 Egypt EG 
 El Salvador SV 
 Equatorial Guinea GQ 
 Eritrea ER 
 Estonia EE 
 Ethiopia ET 
 Falkland Islands (Malvinas) FK 
 Faroe Islands FO 
 Fiji FJ 
 Finland FI 
 France FR 
 France, Metropolitan FX 
 French Guiana GF 
 French Polynesia PF 
 French Southern Territories TF 
 Gabon GA 
 Gambia GM 
 Georgia GE 
 Germany DE 
 Ghana GH 
 Gibraltar GI 
 Greece GR 
 Greenland GL 
 Grenada GD 
 Guadeloupe GP 
 Guam GU 
 Guatemala GT 
 Guinea GN 
 Guinea-Bissau GW 
 Guyana GY 
 Haiti HT 
 Heard and McDonald Islands HM 
 Honduras HN 
 Hong Kong HK 
 Hungary HU 
 Iceland IS 
 India IN 
 Indonesia ID 
 Iraq IQ 
 Ireland IE 
 Israel IL 
 Italy IT 
 Jamaica JM 
 Japan JP 
 Jordan JO 
 Kazakhstan KZ 
 Kenya KE 
 Kiribati KI 
 Kuwait KW 
 Kyrgyzstan KG 
 Lao People's Democratic Republic LA 
 Latvia LV 
 Lebanon LB 
 Lesotho LS 
 Liberia LR 
 Libya LY 
 Liechtenstein LI 
 Lithuania LT 
 Luxembourg LU 
 Macau MO 
 Macedonia MK 
 Madagascar MG 
 Malawi MW 
 Malaysia MY 
 Maldives MV 
 Mali ML 
 Malta MT 
 Marshall Islands MH 
 Martinique MQ 
 Mauritania MR 
 Mauritius MU 
 Mayotte YT 
 Mexico MX 
 Micronesia FM 
 Moldova MD 
 Monaco MC 
 Mongolia MN 
 Montserrat MS 
 Morocco MA 
 Mozambique MZ 
 Namibia NA 
 Nauru NR 
 Nepal NP 
 Netherlands NL 
 Netherlands Antilles AN 
 New Caledonia NC 
 New Zealand NZ 
 Nicaragua NI 
 Niger NE 
 Nigeria NG 
 Niue NU 
 Norfolk Island NF 
 Northern Mariana Islands MP 
 Norway NO 
 Oman OM 
 Pakistan PK 
 Palau PW 
 Palestinian Territory PS 
 Panama PA 
 Papua New Guinea PG 
 Paraguay PY 
 Peru PE 
 Philippines PH 
 Pitcairn PN 
 Poland PL 
 Portugal PT 
 Puerto Rico PR 
 Qatar QA 
 Reunion RE 
 Romania RO 
 Russian Federation RU 
 Rwanda RW 
 Saint Kitts and Nevis KN 
 Saint Lucia LC 
 Saint Vincent and the Grenadines VC 
 Samoa WS 
 San Marino SM 
 Sao Tome and Principe ST 
 Saudi Arabia SA 
 Senegal SN 
 Serbia and Montenegro CS 
 Seychelles SC 
 Sierra Leone SL 
 Singapore SG 
 Slovakia SK 
 Slovenia SI 
 Solomon Islands SB 
 Somalia SO 
 South Africa ZA 
 South Georgia and The South Sandwich Islands GS 
 South Korea KR 
 Spain ES 
 Sri Lanka LK 
 St. Helena SH 
 St. Pierre and Miquelon PM 
 Suriname SR 
 Svalbard and Jan Mayen Islands SJ 
 Swaziland SZ 
 Sweden SE 
 Switzerland CH 
 Taiwan TW 
 Tajikistan TJ 
 Tanzania TZ 
 Thailand TH 
 Togo TG 
 Tokelau TK 
 Tonga TO 
 Trinidad and Tobago TT 
 Tunisia TN 
 Turkey TR 
 Turkmenistan TM 
 Turks and Caicos Islands TC 
 Tuvalu TV 
 Uganda UG 
 Ukraine UA 
 United Arab Emirates AE 
 United Kingdom GB 
 United States US 
 United States Minor Outlying Islands UM 
 Uruguay UY 
 Uzbekistan UZ 
 Vanuatu VU 
 Vatican VA 
 Venezuela VE 
 Viet Nam VN 
 Virgin Islands (British) VG 
 Virgin Islands (U.S.) VI 
 Wallis and Futuna Islands WF 
 Western Sahara EH 
 Yemen YE 
 Zambia ZM 
 Zimbabwe ZW 

         */
    }
}
