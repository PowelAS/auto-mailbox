using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMailbox;
using Xunit;

namespace AutoMailboxTests
{
    public class EmailQueueTests
    {
        [Fact]
        public void TryGetLatest_Empty()
        {
            var q = new EmailQueue(3);
            Assert.False(q.TryGetLatest("", out var email));
        }

        [Fact]
        public void TryGetLatest()
        {
            var q = new EmailQueue(3);
            q.Enqueue(new Email("from@foo.com", "to@foo.com", DateTime.UtcNow, "subject", "body", "html"));

            bool success;
            Email email;

            success = q.TryGetLatest("to", out email);
            Assert.True(success);
            Assert.Equal(email, email);

            // Idempotent
            success = q.TryGetLatest("to", out email);
            Assert.True(success);
            Assert.Equal(email, email);
        }

        [Fact]
        public void TryGetLatest_MultipleEmailsToSameRecipient()
        {
            var q = new EmailQueue(2);

            q.Enqueue(new Email("from@foo.com", "to@foo.com", DateTime.UtcNow, "email 1", "body", "html"));
            q.Enqueue(new Email("from@foo.com", "to@foo.com", DateTime.UtcNow, "email 2", "body", "html"));

            bool success;
            Email email;

            success = q.TryGetLatest("to", out email);
            Assert.True(success);
            Assert.Equal("email 2", email.Subject);

            // Idempotent
            success = q.TryGetLatest("to", out email);
            Assert.True(success);
            Assert.Equal("email 2", email.Subject);
        }

        [Fact]
        public void TryGetLatest_OldestGetDiscarded()
        {
            var q = new EmailQueue(2);
            var dt = DateTime.UtcNow;

            for (int i = 0; i < 3; i++)
                q.Enqueue(new Email("from@foo.com", $"{i}@foo.com", dt, "subject", "body", "html"));

            bool success;
            Email email;

            success = q.TryGetLatest("0", out email);
            Assert.False(success);

            success = q.TryGetLatest("1", out email);
            Assert.True(success);
            Assert.Equal(new Email("from@foo.com", "1@foo.com", dt, "subject", "body", "html"), email);

            success = q.TryGetLatest("2", out email);
            Assert.True(success);
            Assert.Equal(new Email("from@foo.com", "2@foo.com", dt, "subject", "body", "html"), email);
        }

        [Fact]
        public void LoadTest()
        {
            var q = new EmailQueue(1000);
            var dt = DateTime.UtcNow;
            var html = @"<html xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:w=""urn:schemas-microsoft-com:office:word"" xmlns:m=""http://schemas.microsoft.com/office/2004/12/omml"" xmlns=""http://www.w3.org/TR/REC-html40"">
<head>
<meta http-equiv=""Content-Type"" content=""text/html; charset=us-ascii"">
<meta name=""Generator"" content=""Microsoft Word 15 (filtered medium)"">
<style><!--
/* Font Definitions */
@font-face
	{font-family:""Cambria Math"";
	panose-1:2 4 5 3 5 4 6 3 2 4;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}
/* Style Definitions */
p.MsoNormal, li.MsoNormal, div.MsoNormal
	{margin:0cm;
	margin-bottom:.0001pt;
	font-size:11.0pt;
	font-family:""Calibri"",sans-serif;
	mso-fareast-language:EN-US;}
a:link, span.MsoHyperlink
	{mso-style-priority:99;
	color:#0563C1;
	text-decoration:underline;}
a:visited, span.MsoHyperlinkFollowed
	{mso-style-priority:99;
	color:#954F72;
	text-decoration:underline;}
span.EmailStyle17
	{mso-style-type:personal-compose;
	font-family:""Calibri"",sans-serif;
	color:windowtext;}
.MsoChpDefault
	{mso-style-type:export-only;
	font-family:""Calibri"",sans-serif;
	mso-fareast-language:EN-US;}
@page WordSection1
	{size:612.0pt 792.0pt;
	margin:72.0pt 72.0pt 72.0pt 72.0pt;}
div.WordSection1
	{page:WordSection1;}
--></style><!--[if gte mso 9]><xml>
<o:shapedefaults v:ext=""edit"" spidmax=""1026"" />
</xml><![endif]--><!--[if gte mso 9]><xml>
<o:shapelayout v:ext=""edit"">
<o:idmap v:ext=""edit"" data=""1"" />
</o:shapelayout></xml><![endif]-->
</head>
<body lang=""NO-NYN"" link=""#0563C1"" vlink=""#954F72"">
<div class=""WordSection1"">
<p class=""MsoNormal"">Hello<o:p></o:p></p>
</div>
</body>
</html>";

            for (int i=0; i<1100; i++)
                q.Enqueue(new Email("from@foo.com", $"{i}@foo.com", dt, "subject", "text", html));

            Assert.Equal(1000, q.Count);

            bool success;
            Email email;

            success = q.TryGetLatest("800", out email);
            Assert.True(success);
            Assert.Equal(new Email("from@foo.com", "800@foo.com", dt, "subject", "text", html), email);
        }
    }
}
