using System.Threading.Tasks;
using NUnit.Framework;
using Server.Services;

namespace Tests.Functional
{
    [TestFixture]
    public class CrudServiceTests : TestBase
    {
        [Test]
        public async Task CreateTest()
        {
            // Arrange
            var client = new Crud.CrudClient(Channel);

            // Act
            var response = await client.CreateItemAsync(new CreateRequest{Name = "New Item"});

            // Assert
            Assert.GreaterOrEqual(0, response.Id);
        }
    }
}
