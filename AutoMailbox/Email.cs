using System;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;

namespace AutoMailbox
{
    public class Email
    {
        public Email(string @from, string to, DateTime time, string subject, string text, string html)
        {
            From = @from;
            To = to;
            Time = time;
            Subject = subject;
            Text = text;
            Html = html;
        }

        protected bool Equals(Email other)
        {
            return From.Equals(other.From) && To.Equals(other.To) && Time.Equals(other.Time) &&
                   Subject.Equals(other.Subject) && Text.Equals(other.Text) && Html.Equals(other.Html);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Email) obj);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + From?.GetHashCode() ?? 0;
                hash = hash * 23 + To?.GetHashCode() ?? 0;
                hash = hash * 23 + Time.GetHashCode();
                hash = hash * 23 + Subject?.GetHashCode() ?? 0;
                hash = hash * 23 + Text?.GetHashCode() ?? 0;
                hash = hash * 23 + Html?.GetHashCode() ?? 0;
                return hash;
            }
        }

        public static bool operator ==(Email left, Email right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Email left, Email right)
        {
            return !Equals(left, right);
        }

        public string From { get; }
        public string To { get; }
        public DateTime Time { get; }
        public string Subject { get; }
        public string Text { get; }
        public string Html { get; }
        public string Username => (To ?? "").Split('<').Last().Split('@').First();

        public static Email Empty()
        {
            return new Email("", "", DateTime.MinValue, "", "", "");
        }

        public static Email Parse(EmailFormModel formModel)
        {
            var dt = ParseDate(formModel.Headers);
            return new Email(formModel.From ?? "", formModel.To ?? "", dt, formModel.Subject ?? "", formModel.Text ?? "", formModel.Html ?? "");
        }

        static DateTime ParseDate(string headers)
        {
            headers = headers ?? "";

            foreach (var line in headers.Split('\n'))
            {
                const string prefix = "Date: ";
                if (!line.StartsWith(prefix)) continue;
                var s = line.Substring(prefix.Length);

                // Drop anything in parens at the end
                s = s.Split('(').First();

                if (!DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var dt))
                    Log.Error("Unable to parse email time: {Time}.", s);
                else
                    return dt;
            }

            return DateTime.MinValue;
        }

    }
}