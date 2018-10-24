using System;
using System.Linq;

namespace AutoMailbox
{
    public class EmailQueue : CircularQueue<Email>
    {
        public EmailQueue(int size) : base(size)
        {
        }

        public bool TryGetLatest(string username, out Email email)
        {
            foreach (var inQueue in ToArray().Reverse())
            {
                if (!string.Equals(inQueue.Username, username, StringComparison.InvariantCultureIgnoreCase)) continue;
                email = inQueue;
                return true;
            }

            email = Email.Empty();
            return false;
        }
    }
}