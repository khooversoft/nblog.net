using nBlog.sdk.Model;
using System.Text;
using Toolbox.Tools;

namespace nBlog.sdk.Extensions
{
    public static class ContractRequestExtensions
    {
        public static void Verify(this ContactRequest subject)
        {
            subject.VerifyNotNull(nameof(subject));

            subject.Name.VerifyNotEmpty($"{nameof(subject.Name)} is required");
            subject.Email.VerifyNotEmpty($"{nameof(subject.Email)} is required");
            subject.Message.VerifyNotEmpty($"{nameof(subject.Message)} is required");
        }

        public static ContactRequest ToContractRequest(this byte[] subject)
        {
            subject.VerifyAssert(x => x?.Length > 0, $"{nameof(subject)} is empty");

            var json = Encoding.UTF8.GetString(subject);
            return Json.Default.Deserialize<ContactRequest>(json).VerifyNotNull($"Invalid {nameof(ContactRequest)}");
        }

        public static byte[] ToBytes(this ContactRequest subject)
        {
            subject.Verify();

            var json = Json.Default.Serialize(subject);
            return Encoding.UTF8.GetBytes(json);
        }

        public static bool IsValid(this ContactRequest subject)
        {
            try
            {
                subject.Verify();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}