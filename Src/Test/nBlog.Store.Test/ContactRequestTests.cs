using FluentAssertions;
using nBlog.sdk.Model;
using nBlog.Store.Test.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Extensions;
using Xunit;

namespace nBlog.Store.Test
{
    public class ContactRequestTests
    {
        [Fact]
        public async Task GivenContactRequest_WhenFullLifeCycle_ShouldPass()
        {
            TestWebsiteHost host = TestApplication.GetHost();

            const int max = 10;

            IReadOnlyList<ContactRequest> list = Enumerable.Range(0, max)
                .Select(x => new ContactRequest
                {
                    Name = $"Name_{x}",
                    Email = $"Email_{x}",
                    Subject = $"Subject_{x}",
                    Message = $"Message_{x}",
                    RequestResume = x % 2 == 0,
                }).ToList();

            await list
                .ForEachAsync(async x => await host.ContactRequestClient.Set(x));

            List<ContactRequest> readList = new List<ContactRequest>();

            foreach (var item in list)
            {
                ContactRequest? contactRequest = await host.ContactRequestClient.Get(item.RequestId);
                contactRequest.Should().NotBeNull();
                (item == contactRequest).Should().BeTrue();

                (await host.ContactRequestClient.Delete(item.RequestId)).Should().BeTrue();
            }
        }
    }
}
