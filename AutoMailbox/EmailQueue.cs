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
                if (inQueue.Username != username) continue;
                email = inQueue;
                return true;
            }

            email = Email.Empty();
            return false;
        }
    }
}