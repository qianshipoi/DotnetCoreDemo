using Microsoft.AspNetCore.DataProtection;

namespace DataProtectionRedis
{
    public class MyClass
    {
        IDataProtector _protector;

        public MyClass(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("DataProtectionRedis.MyClass.v1");
        }

        public void RunSample()
        {
            Console.Write("Enter input: ");
            string input = Console.ReadLine();

            // protect the payload
            string protectedPayload = _protector.Protect(input);
            Console.WriteLine($"Protect returned: {protectedPayload}");

            // unprotect the payload
            string unprotectedPayload = _protector.Unprotect(protectedPayload);
            Console.WriteLine($"Unprotect returned: {unprotectedPayload}");
        }
    }
}
