using System;
using System.Globalization;
using AutoMailbox;
using Microsoft.Extensions.Logging;
using Xunit;

namespace AutoMailboxTests
{
    public class EmailTests
    {
        [Fact]
        public void Username_NullTo_EmptyUsername()
        {
            var email = new Email(null, null, DateTime.UtcNow, null, null, null);
            Assert.Equal("", email.Username);
        }

        [Fact]
        public void Username_NoAtSymbol_UsernameSameAsTo()
        {
            var email = new Email(null, "foo", DateTime.UtcNow, null, null, null);
            Assert.Equal("foo", email.Username);
        }

        [Fact]
        public void Username_EmailAddress_UsernamePart()
        {
            var email = new Email(null, "foo@bar.com", DateTime.UtcNow, null, null, null);
            Assert.Equal("foo", email.Username);
        }

        [Fact]
        public void Username_EmailAddressAndDisplayName_UsernamePart()
        {
            var email = new Email(null, "Foo Bar<Foo.Bar@domain.com>", DateTime.UtcNow, null, null, null);
            Assert.Equal("Foo.Bar", email.Username);
        }

        [Fact]
        public void Username_DisplayNameIsQuoted_UsernamePart()
        {
            var email = new Email(null, "\"foo@domain.com\" <foo@domain.com>", DateTime.UtcNow, null, null, null);
            Assert.Equal("foo", email.Username);
        }

        [Fact]
        public void Parse_Null_Empty()
        {
            var formModel = new EmailFormModel();
            var email = Email.Parse(formModel);
            Assert.Equal(Email.Empty(), email);
        }

        [Fact]
        public void Parse()
        {
            var formModel = new EmailFormModel()
            {
                Headers = @"Received: by mx0023p1las1.sendgrid.net with SMTP id 98X5f2O0t3 Tue, 13 Mar 2018 12:03:27 +0000 (UTC)
Date: Tue, 13 Mar 2018 12:03:18 +0500
Content-Language: en-US",
                From = "Foo Bar<Foo.Bar@domain.com>",
                To = "Bar Foo<Bar.Foo@domain.com>",
                Subject = "Subject",
                Text = "Hello",
                Html = "<p>Hello</p>"
            };

            var email = Email.Parse(formModel);
            Assert.Equal(new DateTime(2018, 3, 13, 7, 3, 18, DateTimeKind.Utc), email.Time);
            Assert.Equal("Foo Bar<Foo.Bar@domain.com>", email.From);
            Assert.Equal("Bar Foo<Bar.Foo@domain.com>", email.To);
            Assert.Equal("Bar.Foo", email.Username);
            Assert.Equal("Subject", email.Subject);
            Assert.Equal("Hello", email.Text);
            Assert.Equal("<p>Hello</p>", email.Html);
        }

        [Fact]
        public void Parse_InvalidTime()
        {
            var formModel = new EmailFormModel()
            {
                Headers = "Date: foo"
            };

            var email = Email.Parse(formModel);
            Assert.Equal(Email.Empty(), email);
        }

        [Fact]
        public void Parse_TimeWithParens()
        {
            var formModel = new EmailFormModel()
            {
                Headers = "Date: Wed, 14 Mar 2018 13:04:44 +0000 (UTC)"
            };

            var email = Email.Parse(formModel);
            Assert.Equal(new DateTime(2018, 3, 14, 13, 4, 44, DateTimeKind.Utc), email.Time);
        }

        [Fact]
        public void DateTimeParse_MsDocumented()
        {
            var dt = DateTime.Parse("Sat, 01 Nov 2008 19:35:00 GMT", CultureInfo.InvariantCulture).ToUniversalTime();
            var expected = new DateTime(2008, 11, 1, 19, 35, 0, DateTimeKind.Utc);
            Assert.Equal(expected, dt);
        }

        [Fact]
        public void DateTimeParse_FromEmailHeader()
        {
            var dt = DateTime.Parse("Tue, 13 Mar 2018 12:03:18 +0500", CultureInfo.InvariantCulture).ToUniversalTime();
            var expected = new DateTime(2018, 3, 13, 7, 3, 18, DateTimeKind.Utc);
            Assert.Equal(expected, dt);
        }
    }
}
