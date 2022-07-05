# Infoware.UnitOfWork
 UnitOfWork implementation

### Get it!
[![NuGet Badge](https://buildstats.info/nuget/Infoware.UnitOfWork)](https://www.nuget.org/packages/Infoware.UnitOfWork/)

### How to use it
- Inject via Dependency Injection 

```csharp
        services.AddAnotherService();

        services.AddDbContext<AwesomeContext>(
            options =>
            {
                options.UseSqlServer(configuration["ConnectionStrings:MyConnectionString"],
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    });
            }
        ).AddUnitOfWork<AwesomeContext>();



    public class CoolService : ICoolService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoolService (IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public DoAwesomeness(string? filter, int page, int rowsPerPage, CancellationToken cancellationToken = default)
        {
            DoMoreStuff();

            var myData = _unitOfWork.GetRepository<Data>().GetFirstOrDefaultAsync(
                selector:
                    e => new DataView
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description
                    },
                predicate:
                    e => e.Id == 999
            );

            var myList = _unitOfWork.GetRepository<Data>().GetAll(
                include:
                    p => p.Include(y => y.SubData),
                selector:
                    e => new DataView
                    {
                        Id = e.Id,
                        Name = e.Name,
                        Description = e.Description
                    },
                predicate:
                    e => e.Name.Contains(filter),
                orderBy:
                    p => p.OrderBy(x => x.Name)
            ).ToPagedListAsync(pageIndex: page, pageSize: rowsPerPage, cancellationToken: cancellationToken);
        };
    }
```

## Buy me a coofee
If you want, buy me a coofee :coffee: https://www.paypal.com/paypalme/vicosanzdev?locale.x=es_XC

