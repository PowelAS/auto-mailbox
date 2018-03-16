# auto-mailbox
A service that can receive emails from Sendgrid Inbound Parse Webhook, and serve latest email on a HTTP GET request. Useful for automated tests involving emails.

## How to use

1. Deploy this web app on a public endpoint
2. Set up [Sendgrid Inbound Parse Webhook](https://sendgrid.com/docs/API_Reference/Webhooks/inbound_email.html) and point it to https://your-public-endpoint/api/email
3. Send an email to <whatever@your-inbound-subdomain.domain.com>
4. Check <https://your-public-endpoint/api/email/whatever>