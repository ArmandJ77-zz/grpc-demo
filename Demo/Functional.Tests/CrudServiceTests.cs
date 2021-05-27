using Database;
using Database.EntityModels;
using NUnit.Framework;
using Server.Services;
using System.Threading.Tasks;
using Tests.Functional.Helpers;

namespace Tests.Functional
{
    [TestFixture]
    public class CrudServiceTests : TestBase
    {
        [Test]
        public async Task Given_A_New_Item_When_CreatItemAsync_Assert_ResponseId_GreaterOrEqual_0()
        {
            // Arrange
            var client = new Crud.CrudClient(Channel);
            var item = new CreateRequest { Name = "New Item" };
            // Act
            var response = await client.CreateItemAsync(item);

            // Assert
            Assert.GreaterOrEqual(0, response.Id);
        }

        [Test]
        public async Task Given_An_ItemId_When_GetById_Then_Assert_GetItemByIdRequest_Is_Not_Null()
        {
            // Arrange
            var client = new Crud.CrudClient(Channel);
            var item = new GetItemByIdRequest { Id = 1 };
            var seedItem = new Item
            {
                Id = 1,
                Name = "Alienware M15 R3"
            };

            Fixture.Host.Seed<DemoDbContext>(db =>
            {
                db.Clear();
                db.Items.Add(seedItem);
            });

            // Act
            var response = await client.GetByIdAsync(item);

            // Assert
            Assert.That(response.Id, Is.EqualTo(1));
            StringAssert.AreEqualIgnoringCase("Alienware M15 R3", response.Name);
        }
    }
}
