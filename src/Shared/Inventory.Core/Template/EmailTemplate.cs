﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Core.Template
{
    public class EmailTemplate
    {
        public static string Get()
        {
            return "<!doctype html>\r\n<html xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\">\r\n<head>\r\n    <title></title>\r\n    <!--[if !mso]><!-->\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <!--<![endif]-->\r\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <style type=\"text/css\">\r\n        #outlook a {\r\n            padding: 0;\r\n        }\r\n\r\n        body {\r\n            margin: 0;\r\n            padding: 0;\r\n            -webkit-text-size-adjust: 100%;\r\n            -ms-text-size-adjust: 100%;\r\n        }\r\n\r\n        table, td {\r\n            border-collapse: collapse;\r\n            mso-table-lspace: 0pt;\r\n            mso-table-rspace: 0pt;\r\n        }\r\n\r\n        img {\r\n            border: 0;\r\n            height: auto;\r\n            line-height: 100%;\r\n            outline: none;\r\n            text-decoration: none;\r\n            -ms-interpolation-mode: bicubic;\r\n        }\r\n\r\n        p {\r\n            display: block;\r\n            margin: 13px 0;\r\n        }\r\n    </style>\r\n    <!--[if mso]>\r\n    <noscript>\r\n    <xml>\r\n    <o:OfficeDocumentSettings>\r\n      <o:AllowPNG/>\r\n      <o:PixelsPerInch>96</o:PixelsPerInch>\r\n    </o:OfficeDocumentSettings>\r\n    </xml>\r\n    </noscript>\r\n    <![endif]-->\r\n    <!--[if lte mso 11]>\r\n    <style type=\"text/css\">\r\n      .mj-outlook-group-fix { width:100% !important; }\r\n    </style>\r\n    <![endif]-->\r\n    <!--[if !mso]><!-->\r\n    <link href=\"https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700\" rel=\"stylesheet\" type=\"text/css\">\r\n    <style type=\"text/css\">\r\n        @import url(https://fonts.googleapis.com/css?family=Ubuntu:300,400,500,700);\r\n    </style>\r\n    <!--<![endif]-->\r\n\r\n\r\n\r\n    <style type=\"text/css\">\r\n        @media only screen and (min-width:480px) {\r\n            .mj-column-per-100 {\r\n                width: 100% !important;\r\n                max-width: 100%;\r\n            }\r\n        }\r\n    </style>\r\n    <style media=\"screen and (min-width:480px)\">\r\n        .moz-text-html .mj-column-per-100 {\r\n            width: 100% !important;\r\n            max-width: 100%;\r\n        }\r\n    </style>\r\n\r\n\r\n    <style type=\"text/css\">\r\n    </style>\r\n    <style type=\"text/css\">\r\n        .hide_on_mobile {\r\n            display: none !important;\r\n        }\r\n\r\n        @media only screen and (min-width: 480px) {\r\n            .hide_on_mobile {\r\n                display: block !important;\r\n            }\r\n        }\r\n\r\n        .hide_section_on_mobile {\r\n            display: none !important;\r\n        }\r\n\r\n        @media only screen and (min-width: 480px) {\r\n            .hide_section_on_mobile {\r\n                display: table !important;\r\n            }\r\n\r\n            div.hide_section_on_mobile {\r\n                display: block !important;\r\n            }\r\n        }\r\n\r\n        .hide_on_desktop {\r\n            display: block !important;\r\n        }\r\n\r\n        @media only screen and (min-width: 480px) {\r\n            .hide_on_desktop {\r\n                display: none !important;\r\n            }\r\n        }\r\n\r\n        .hide_section_on_desktop {\r\n            display: table !important;\r\n            width: 100%;\r\n        }\r\n\r\n        @media only screen and (min-width: 480px) {\r\n            .hide_section_on_desktop {\r\n                display: none !important;\r\n            }\r\n        }\r\n\r\n        p, h1, h2, h3 {\r\n            margin: 0px;\r\n        }\r\n\r\n        ul, li, ol {\r\n            font-size: 11px;\r\n            font-family: Ubuntu, Helvetica, Arial;\r\n        }\r\n\r\n        a {\r\n            text-decoration: none;\r\n            color: inherit;\r\n        }\r\n\r\n        @media only screen and (max-width:480px) {\r\n            .mj-column-per-100 {\r\n                width: 100% !important;\r\n                max-width: 100% !important;\r\n            }\r\n\r\n                .mj-column-per-100 > .mj-column-per-100 {\r\n                    width: 100% !important;\r\n                    max-width: 100% !important;\r\n                }\r\n        }\r\n    </style>\r\n\r\n</head>\r\n<body style=\"word-spacing:normal;background-color:#f4f4f4;\">\r\n\r\n\r\n    <div style=\"background-color:#f4f4f4;\">\r\n\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#3f51b5;background-color:#3f51b5;width:100%;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td>\r\n\r\n\r\n                        <!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" role=\"presentation\" style=\"width:600px;\" width=\"600\" bgcolor=\"#3f51b5\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]-->\r\n\r\n\r\n                        <div style=\"margin:0px auto;max-width:600px;\">\r\n\r\n                            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\">\r\n                                <tbody>\r\n                                    <tr>\r\n                                        <td style=\"direction:ltr;font-size:0px;padding:14px 0px 14px 0px;text-align:center;\">\r\n                                            <!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:middle;width:600px;\" ><![endif]-->\r\n\r\n                                            <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:middle;width:100%;\">\r\n\r\n                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:middle;\" width=\"100%\">\r\n                                                    <tbody>\r\n\r\n                                                        <tr>\r\n                                                            <td align=\"left\" style=\"font-size:0px;padding:5px 5px 5px 5px;word-break:break-word;\">\r\n\r\n                                                                <div style=\"font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:1.5;text-align:left;color:#000000;\"><p style=\"font-size: 11px; font-family: Ubuntu, Helvetica, Arial; text-align: center;\"><span style=\"font-size: 22px;\"><strong>INVENTORY MANAGEMENT</strong></span></p></div>\r\n\r\n                                                            </td>\r\n                                                        </tr>\r\n\r\n                                                    </tbody>\r\n                                                </table>\r\n\r\n                                            </div>\r\n\r\n                                            <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                                        </td>\r\n                                    </tr>\r\n                                </tbody>\r\n                            </table>\r\n\r\n                        </div>\r\n\r\n\r\n                        <!--[if mso | IE]></td></tr></table><![endif]-->\r\n\r\n\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#f4f4f4;background-color:#f4f4f4;width:100%;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td>\r\n\r\n\r\n                        <!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" role=\"presentation\" style=\"width:600px;\" width=\"600\" bgcolor=\"#f4f4f4\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]-->\r\n\r\n\r\n                        <div style=\"margin:0px auto;max-width:600px;\">\r\n\r\n                            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\">\r\n                                <tbody>\r\n                                    <tr>\r\n                                        <td style=\"direction:ltr;font-size:0px;padding:9px 0px 9px 0px;text-align:center;\">\r\n                                            <!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]-->\r\n\r\n                                            <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\">\r\n\r\n                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\">\r\n                                                    <tbody>\r\n\r\n                                                        <tr>\r\n                                                            <td align=\"center\" style=\"font-size:0px;padding:0px 20px 0px 20px;word-break:break-word;\">\r\n\r\n                                                                <div style=\"font-family:Verdana, sans-serif;font-size:13px;line-height:1.5;text-align:center;color:#2b729e;\">\r\n                                                                    <h1 style=\"font-size: 22px; font-family: Ubuntu, Helvetica, Arial;\"><span style=\"font-size: 18px;\"><span style=\"color: #000000;\">Hello there!</span></span></h1>\r\n                                                                    <h1 style=\"font-size: 22px; font-family: Ubuntu, Helvetica, Arial;\"><span style=\"font-size: 18px;\"><span style=\"color: #000000;\">You have a {name} to approval!</span></span></h1>\r\n                                                                </div>\r\n\r\n                                                            </td>\r\n                                                        </tr>\r\n\r\n                                                    </tbody>\r\n                                                </table>\r\n\r\n                                            </div>\r\n\r\n                                            <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                                        </td>\r\n                                    </tr>\r\n                                </tbody>\r\n                            </table>\r\n\r\n                        </div>\r\n\r\n\r\n                        <!--[if mso | IE]></td></tr></table><![endif]-->\r\n\r\n\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n\r\n        <!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" role=\"presentation\" style=\"width:600px;\" width=\"600\" bgcolor=\"#3f51b5\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]-->\r\n\r\n\r\n        <div style=\"background:#3f51b5;background-color:#3f51b5;margin:0px auto;max-width:600px;\">\r\n\r\n            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#3f51b5;background-color:#3f51b5;width:100%;\">\r\n                <tbody>\r\n                    <tr>\r\n                        <td style=\"direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;\">\r\n                            <!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"width:600px;\" ><![endif]-->\r\n\r\n                            <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0;line-height:0;text-align:left;display:inline-block;width:100%;direction:ltr;\">\r\n                                <!--[if mso | IE]><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" ><tr><td style=\"vertical-align:top;width:600px;\" ><![endif]-->\r\n\r\n                                <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\">\r\n\r\n                                    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\">\r\n                                        <tbody>\r\n\r\n                                            <tr>\r\n                                                <td align=\"left\" style=\"font-size:0px;padding:10px 10px 10px 99px;word-break:break-word;\">\r\n\r\n                                                    <div style=\"font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:13px;line-height:1.8;text-align:left;color:#000000;\">\r\n                                                        {info}\r\n                                                    </div>\r\n\r\n                                                </td>\r\n                                            </tr>\r\n\r\n                                        </tbody>\r\n                                    </table>\r\n\r\n                                </div>\r\n\r\n                                <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                            </div>\r\n\r\n                            <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                        </td>\r\n                    </tr>\r\n                </tbody>\r\n            </table>\r\n\r\n        </div>\r\n\r\n\r\n        <!--[if mso | IE]></td></tr></table><![endif]-->\r\n\r\n\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#f4f4f4;background-color:#f4f4f4;width:100%;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td>\r\n\r\n\r\n                        <!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" role=\"presentation\" style=\"width:600px;\" width=\"600\" bgcolor=\"#f4f4f4\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]-->\r\n\r\n\r\n                        <div style=\"margin:0px auto;max-width:600px;\">\r\n\r\n                            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\">\r\n                                <tbody>\r\n                                    <tr>\r\n                                        <td style=\"direction:ltr;font-size:0px;padding:13px 0px 13px 0px;text-align:center;\">\r\n                                            <!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]-->\r\n\r\n                                            <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\">\r\n\r\n                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\">\r\n                                                    <tbody>\r\n\r\n                                                        <tr>\r\n                                                            <td align=\"center\" vertical-align=\"middle\" style=\"font-size:0px;padding:6px 6px 6px 6px;word-break:break-word;\">\r\n\r\n                                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"border-collapse:separate;width:auto;line-height:100%;\">\r\n                                                                    <tbody>\r\n                                                                        <tr>\r\n                                                                            <td align=\"center\" bgcolor=\"#3F51B5\" role=\"presentation\" style=\"border:0px solid #000;border-radius:none;cursor:auto;mso-padding-alt:10px 30px;background:#3F51B5;\" valign=\"middle\">\r\n                                                                                {link}\r\n                                                                            </td>\r\n                                                                        </tr>\r\n                                                                    </tbody>\r\n                                                                </table>\r\n\r\n                                                            </td>\r\n                                                        </tr>\r\n\r\n                                                    </tbody>\r\n                                                </table>\r\n\r\n                                            </div>\r\n\r\n                                            <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                                        </td>\r\n                                    </tr>\r\n                                </tbody>\r\n                            </table>\r\n\r\n                        </div>\r\n\r\n\r\n                        <!--[if mso | IE]></td></tr></table><![endif]-->\r\n\r\n\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#f4f4f4;background-color:#f4f4f4;width:100%;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td>\r\n\r\n\r\n                        <!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" role=\"presentation\" style=\"width:600px;\" width=\"600\" bgcolor=\"#f4f4f4\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]-->\r\n\r\n\r\n                        <div style=\"margin:0px auto;max-width:600px;\">\r\n\r\n                            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\">\r\n                                <tbody>\r\n                                    <tr>\r\n                                        <td style=\"direction:ltr;font-size:0px;padding:0px 0px 0px 0px;text-align:center;\">\r\n                                            <!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]-->\r\n\r\n                                            <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\">\r\n\r\n                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\">\r\n                                                    <tbody>\r\n\r\n                                                        <tr>\r\n                                                            <td style=\"font-size:0px;word-break:break-word;\">\r\n\r\n                                                                <div style=\"height:10px;line-height:10px;\">&#8202;</div>\r\n\r\n                                                            </td>\r\n                                                        </tr>\r\n\r\n                                                    </tbody>\r\n                                                </table>\r\n\r\n                                            </div>\r\n\r\n                                            <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                                        </td>\r\n                                    </tr>\r\n                                </tbody>\r\n                            </table>\r\n\r\n                        </div>\r\n\r\n\r\n                        <!--[if mso | IE]></td></tr></table><![endif]-->\r\n\r\n\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n        <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"background:#3f51b5;background-color:#3f51b5;width:100%;\">\r\n            <tbody>\r\n                <tr>\r\n                    <td>\r\n\r\n\r\n                        <!--[if mso | IE]><table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"\" role=\"presentation\" style=\"width:600px;\" width=\"600\" bgcolor=\"#3f51b5\" ><tr><td style=\"line-height:0px;font-size:0px;mso-line-height-rule:exactly;\"><![endif]-->\r\n\r\n\r\n                        <div style=\"margin:0px auto;max-width:600px;\">\r\n\r\n                            <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"width:100%;\">\r\n                                <tbody>\r\n                                    <tr>\r\n                                        <td style=\"direction:ltr;font-size:0px;padding:14px 0px 14px 0px;text-align:center;\">\r\n                                            <!--[if mso | IE]><table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"\" style=\"vertical-align:top;width:600px;\" ><![endif]-->\r\n\r\n                                            <div class=\"mj-column-per-100 mj-outlook-group-fix\" style=\"font-size:0px;text-align:left;direction:ltr;display:inline-block;vertical-align:top;width:100%;\">\r\n\r\n                                                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" role=\"presentation\" style=\"vertical-align:top;\" width=\"100%\">\r\n                                                    <tbody>\r\n\r\n                                                        <tr>\r\n                                                            <td align=\"center\" style=\"font-size:0px;padding:0px 0px 0px 0px;word-break:break-word;\">\r\n\r\n                                                                <div style=\"font-family:Ubuntu, Helvetica, Arial, sans-serif;font-size:11px;line-height:1.5;text-align:center;color:#000000;\">\r\n                                                                    <p style=\"font-family: Ubuntu, Helvetica, Arial; font-size: 11px;\">This is an automatic email sent by the system</p>\r\n                                                                    <p style=\"font-size: 11px; font-family: Ubuntu, Helvetica, Arial;\">Ignore this email if you have approved the ticket</p>\r\n                                                                </div>\r\n\r\n                                                            </td>\r\n                                                        </tr>\r\n\r\n                                                    </tbody>\r\n                                                </table>\r\n\r\n                                            </div>\r\n\r\n                                            <!--[if mso | IE]></td></tr></table><![endif]-->\r\n                                        </td>\r\n                                    </tr>\r\n                                </tbody>\r\n                            </table>\r\n\r\n                        </div>\r\n\r\n\r\n                        <!--[if mso | IE]></td></tr></table><![endif]-->\r\n\r\n\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n\r\n    </div>\r\n\r\n</body>\r\n</html>";
        }
    }
}
