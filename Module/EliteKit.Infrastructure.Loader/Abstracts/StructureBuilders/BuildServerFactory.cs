using EliteKit.Infrastructure.Core.Abstracts.BottomStructures;
using EliteKit.Infrastructure.Core.Documents.AppSettings;
using EliteKit.Infrastructure.Core.Expansions.CommonExamples;
using EliteKit.Infrastructure.Core.Functions.ModularLaunchers;
using EliteKit.Infrastructure.Core.Functions.UniversalLayouts;
using EliteKit.Infrastructure.Loader.Functions.MiddlePipelines;
using EliteKit.Infrastructure.Loader.Functions.StructuralFrames;

namespace EliteKit.Infrastructure.Loader.Abstracts.StructureBuilders;
public abstract class BuildServerFactory<T> : ModuleBase<T> where T : IModularization
{
    bool Display = true;
    public abstract WebApplication Run(WebApplication app);
    public async Task<WebApplication?> AsyncCallback(Assembly assembly, WebApplicationBuilder builder)
    {
        var signalAsync = SignalAsync();
        builder.Configuration.AddEnvironmentVariables();
        builder.Logging.AddFilter<ConsoleLoggerProvider>(nameof(Microsoft), LogLevel.Error);
        builder.Logging.AddFilter<ConsoleLoggerProvider>(nameof(FastEndpoints), LogLevel.Error);
        builder.Services.Configure<AccessRecipe>(builder.Configuration.GetSection(nameof(AccessRecipe)));
        builder.Services.Configure<HostInformation>(builder.Configuration.GetSection(nameof(HostInformation)));
        builder.Services.Configure<ServiceTerritory>(builder.Configuration.GetSection(nameof(ServiceTerritory)));
        builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());
        builder.Services.Configure<RequestLocalizationOptions>(x =>
        {
            List<CultureInfo> cultureInfos = [];
            var settings = builder.Configuration.GetSection(nameof(AccessRecipe)).Get<AccessRecipe>();
            if (settings is not null)
            {
                cultureInfos.AddRange(settings.SupportedCultures.Select(x => new CultureInfo(x)));
                x.DefaultRequestCulture = new(settings.DefaultCulture);
                x.SupportedCultures = cultureInfos;
                x.SupportedUICultures = cultureInfos;
                x.RequestCultureProviders.Insert(default, new AcceptLanguageHeaderRequestCultureProvider());
            }
        });
        builder.Services.AddScheduler();
        builder.Services.AddHttpClient();
        builder.Services.AddProblemDetails();
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddExceptionHandler<ExceptionMiddle>();
        builder.Host.ConfigureHostOptions(x =>
        {
            x.ServicesStopConcurrently = true;
            x.ServicesStartConcurrently = true;
            x.ShutdownTimeout = TimeSpan.FromSeconds(30);
        }).UseSystemd();
        builder.Services.ConfigureHttpJsonOptions(x =>
        {
            x.SerializerOptions.Converters.Add(new DateConvert());
            x.SerializerOptions.Converters.Add(new EnumConvert());
            x.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        builder.Services.AddControllers(x =>
        {
            x.ReturnHttpNotAcceptable = true;
        }).ConfigureApiBehaviorOptions(x =>
        {
            x.SuppressModelStateInvalidFilter = default;
        }).AddMvcOptions(x => x.Conventions.Add(new ModelConvention())).AddControllersAsServices();
        builder.Services.AddLocalization();
        builder.Services.AddResponseCaching().AddMemoryCache();
        builder.Services.AddSingleton<IStringLocalizerFactory>(x => new DialectFactory());
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>((context, builder) =>
        {
            List<Type> loadedModules = [];
            foreach (var moduleType in assembly.GetTypes().Where(x =>
            {
                return typeof(Autofac.Module).IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface;
            })) MountProject(moduleType);
            void MountProject(in Type type)
            {
                if (loadedModules.Contains(type)) return;
                var dependsOn = type.GetCustomAttribute<DependsOnAttribute>();
                if (dependsOn is not null)
                {
                    foreach (var dependency in dependsOn.Dependencies) MountProject(dependency);
                }
                var module = Activator.CreateInstance(type) as Autofac.Module;
                if (module is not null)
                {
                    builder.RegisterModule(module);
                    loadedModules.Add(type);
                }
            }
        });

        IList<(string tag, int port, bool enabled, string description)> listenBanners = [];
        builder.WebHost.UseKestrel(x =>
        {
            List<ListenInfo> endpoints = [];
            var hostInfo = builder.Configuration.GetSection(nameof(HostInformation)).Get<HostInformation>();
            if (hostInfo is not null)
            {
                if (hostInfo.HTTPS.Enabled)
                {
                    x.ConfigureHttpsDefaults(x => x.SslProtocols = SslProtocols.Tls12);
                    endpoints.Add(new(nameof(HostInformation.HTTPS), hostInfo.HTTPS.Port, x =>
                    {
                        x.UseHttps(hostInfo.HTTPS.Certificate.Location, hostInfo.HTTPS.Certificate.Password);
                    }));
                }
                endpoints.Add(new(nameof(HostInformation.HTTP), hostInfo.HTTP.Port, default));
            }
            if (endpoints.Count is not 0)
            {
                foreach (var endpoint in endpoints)
                {
                    if (!IPAddress.Loopback.VerifyPort(port: endpoint.Port))
                    {
                        switch (endpoint.ListenOptions)
                        {
                            case not null:
                                x.ListenAnyIP(endpoint.Port, endpoint.ListenOptions);
                                break;

                            default:
                                x.ListenAnyIP(endpoint.Port);
                                break;
                        }
                        listenBanners.Add((endpoint.Name, endpoint.Port, true, "main entrance"));
                    }
                }
            }
            Display = default;
        }).ConfigureLogging((context, builder) =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Critical);
            {
                var seq = context.Configuration.GetSection(nameof(ServiceTerritory)).Get<ServiceTerritory>()!.Seq;
                Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().MinimumLevel.Debug()
                .MinimumLevel.Override(nameof(Microsoft), LogEventLevel.Error)
                .MinimumLevel.Override(nameof(FastEndpoints), LogEventLevel.Error)
                .WriteTo.Console().WriteTo.Seq(seq.Endpoint, apiKey: seq.ApiKey).CreateLogger();
            }
        });

        var result = builder.Build();
        await signalAsync.ConfigureAwait(false);
        if (Missions.TryGetValue(nameof(ListenInfo), out var mission)) await mission.ConfigureAwait(false);
        {
            Table table = new();
            table.Border(TableBorder.Rounded);
            table.AddColumn(new TableColumn("[bold chartreuse3]Tag name[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold chartreuse3]Port number[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold chartreuse3]Service enabled[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold chartreuse3]Description[/]").LeftAligned());
            foreach (var (tag, port, enabled, description) in listenBanners)
            {
                table.AddRow($"[bold]{tag}[/]", $"[bold]{port.ToString(CultureInfo.InvariantCulture)}[/]",
                    $"[bold]{enabled}[/]", $"[bold]{description}[/]");
            }
            AnsiConsole.Write(table);
            await string.Empty.PrintAsync(ConsoleColor.White).ConfigureAwait(false);
        }
        async Task SignalAsync(int frequency = 100)
        {
            Console.Title = nameof(System);
            Console.CursorVisible = default;
            await Print(GetNameplate()).PrintAsync(ConsoleColor.Yellow).ConfigureAwait(false);
            await new[]
            {
                new string('*', 60), Environment.NewLine,
            }.Merge().PrintAsync().ConfigureAwait(false);
            if (!Missions.Keys.Any(x => x.IsMatch(nameof(ListenInfo)))) Missions[nameof(ListenInfo)] = Task.Run(async () =>
            {
                PeriodicTimer timer = new(TimeSpan.FromMilliseconds(frequency));
                string name = string.Empty, stroke = "/", dash = "-", backslash = "\\", or = "|";
                while (await timer.WaitForNextTickAsync(default).ConfigureAwait(false))
                {
                    Console.SetCursorPosition(default, Console.CursorTop);
                    Console.Write(name switch
                    {
                        var x when x.IsMatch(or) => name = stroke,
                        var x when x.IsMatch(stroke) => name = dash,
                        var x when x.IsMatch(backslash) => name = or,
                        _ => name = backslash,
                    });
                    if (!Display)
                    {
                        Console.SetCursorPosition(default, Console.CursorTop);
                        timer.Dispose();
                    }
                }
                Console.SetCursorPosition(default, Console.CursorTop);
            }, default);
            static string Print(in (string tag, string content)[] menus)
            {
                List<string> lines = [];
                var space = $"\u00A0\u00A0\u00A0";
                Console.InputEncoding = Encoding.UTF8;
                Console.OutputEncoding = Encoding.UTF8;
                foreach (var (key, content) in menus) lines.Add($"{key,16}{space}=>{space}{content,-10}{Environment.NewLine}");
                return ((string[])[.. lines]).Merge();
            }
            (string tag, string content)[] GetNameplate() => [
                ("Host name", Dns.GetHostName()),
                ("User name", Environment.UserName),
                (".NET SDK", Environment.Version.ToString()),
                ("Internet", NetworkInterface.GetIsNetworkAvailable().ToString()),
                ("Language tag", Thread.CurrentThread.CurrentCulture.IetfLanguageTag),
                ("Language name", Thread.CurrentThread.CurrentCulture.DisplayName),
                ("IPv4 physical", NetworkInterfaceType.Ethernet.GetLocalIPv4().FirstOrDefault() ?? string.Empty),
                ("IPv4 wireless", NetworkInterfaceType.Wireless80211.GetLocalIPv4().FirstOrDefault() ?? string.Empty),
                ("Project name", FileLayout.GetProjectName<T>()),
                ("OS version", Environment.OSVersion.ToString()),
            ];
        }
        return Run(result);
    }
    record ListenInfo(string Name, int Port, Action<ListenOptions>? ListenOptions);
    Dictionary<string, Task> Missions { get; } = new Dictionary<string, Task>(StringComparer.Ordinal);
}