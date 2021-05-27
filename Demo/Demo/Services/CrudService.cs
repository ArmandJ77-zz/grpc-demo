using Database;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Database.EntityModels;

namespace Server.Services
{
    public class CrudService : Crud.CrudBase
    {
        private readonly ILogger<CrudService> _logger;
        private readonly DemoDbContext _demoDbContext;

        public CrudService(ILogger<CrudService> logger
        , DemoDbContext demoDbContext)
        {
            _logger = logger;
            _demoDbContext = demoDbContext;
        }

        public override async Task<CreateResponse> CreateItem(CreateRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Create Request Received with body: {JsonConvert.SerializeObject(request)}");

            var entity = new Item { Name = request.Name };
            await _demoDbContext.AddAsync(entity);
            await _demoDbContext.SaveChangesAsync();

            return new CreateResponse { Id = entity.Id };
        }

        public override async Task<GetItemResponse> GetById(GetItemByIdRequest request, ServerCallContext context)
        {
            _logger.LogInformation($"Get By Id Request Received with Id: {request.Id}");

            var result = await _demoDbContext.Items.FindAsync(request.Id);

            if (result is not null)
                return new GetItemResponse
                {
                    Id = result.Id,
                    Name = result.Name
                };

            _logger.LogInformation($"Item not found with Id: {request.Id}");
            return new GetItemResponse();
        }
    }
}
