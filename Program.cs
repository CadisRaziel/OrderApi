using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using OrderApi.Endpoints.Clients;
using OrderApi.Endpoints.Employees;
using OrderApi.Endpoints.Products;
using OrderApi.Endpoints.Security;
using OrderApi.Entpoints.Categories;
using OrderApi.Infra.Data;
using OrderApi.Infra.Service;
using OrderApi.Services.Users;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;
using System.Text.Json;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Configurando os loggers no banco de dados
//context -> para saber o endereço do banco de dados
//configuration -> para configurar o serilog com os WriteTo
//Observação importante -> O serilog remove o log que o .net ja coloca por default e assume esse papel !! (os logs fica tipo os logs de nodejs)
builder.WebHost.UseSerilog((context, configuration) => {
    configuration
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            context.Configuration["ConnectionStrings:OrderApiDb"],
              //sinkOptions -> To dizendo pra ele criar a tabela automaticamente
              sinkOptions: new MSSqlServerSinkOptions()
              {
                  AutoCreateSqlTable = true,
                  TableName = "LogAPI"                 
              });
});


//Conexão com banco de dados
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:OrderApiDb"]);


/*
//Quando vamos criar um usuario a senha tem varias verificaçoes como tem que ser maior que 6, tem que te numero, tem que te caracter especial
//Deixando dessa forma ele vai ser o padrão igual eu disse acima
//Abaixo desse comentario veremos a mesma função porém com o options no construtor, e é ele que a gente remove se quiser o "padrão" e deixa da forma que queremos
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
 */

//Configuração do Identity (estamos usando o serviço do identity junto com meu DbContext para ele entender como acessar o banco de dados)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false; //-> aqui eu removo a obrigatoriedade de ter um caracter especial
    options.Password.RequireUppercase = false; //-> aqui eu removo a obrigatoriedade de ter uma letra maiuscula
    options.Password.RequireDigit = false; //-> aqui eu removo a obrigatoriedade de ter um numero
    options.Password.RequiredLength = 3; //-> aqui eu removo a obrigatoriedade de ter no minimo 6 caracteres
    options.Password.RequireLowercase = false; //-> aqui eu removo a obrigatoriedade de ter uma letra minuscula
    options.User.RequireUniqueEmail = true; //-> aqui eu obrigo o email a ser unico
    //options.SignIn.RequireConfirmedEmail = false; //-> aqui eu removo a obrigatoriedade de confirmar o email
    //options.SignIn.RequireConfirmedAccount = false; //-> aqui eu removo a obrigatoriedade de confirmar a conta
    //options.Lockout.MaxFailedAccessAttempts = 5; //-> aqui eu removo a obrigatoriedade de ter no minimo 5 tentativas de login
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //-> aqui eu removo a obrigatoriedade de ter no minimo 5 minutos de bloqueio
    //options.Lockout.AllowedForNewUsers = true; //-> aqui eu removo a obrigatoriedade de ter o bloqueio para novos usuarios
    //options.Tokens.EmailConfirmationTokenProvider = "CustomEmailConfirmation"; //-> aqui eu removo a obrigatoriedade de ter um token para confirmar o email
    //options.Tokens.ChangeEmailTokenProvider = "CustomChangeEmail"; //-> aqui eu removo a obrigatoriedade de ter um token para mudar o email
    //options.Tokens.PasswordResetTokenProvider = "CustomPasswordReset"; //-> aqui eu removo a obrigatoriedade de ter um token para resetar a senha
    //options.Tokens.PhoneConfirmationTokenProvider = "CustomPhoneConfirmation"; //-> aqui eu removo a obrigatoriedade de ter um token para confirmar o telefone
    //options.Tokens.AuthenticatorTokenProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticar
    //options.Tokens.ChangePhoneNumberTokenProvider = "CustomChangePhoneNumber"; //-> aqui eu removo a obrigatoriedade de ter um token para mudar o telefone
    //options.Tokens.TwoFactorRecoveryCodeTokenProvider = "CustomTwoFactorRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recuperar a autenticação
    //options.Tokens.DefaultEmailProvider = "CustomEmail"; //-> aqui eu removo a obrigatoriedade de ter um token para email padrão
    //options.Tokens.DefaultPhoneProvider = "CustomPhone"; //-> aqui eu removo a obrigatoriedade de ter um token para telefone padrão
    //options.Tokens.DefaultAuthenticatorProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticador padrão
    //options.Tokens.DefaultRecoveryCodeProvider = "CustomRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recuperação de codigo padrão
    //options.Tokens.DefaultTwoFactorRecoveryCodeProvider = "CustomTwoFactorRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recuperação de codigo de autenticação padrão
    //options.Tokens.DefaultChangePhoneNumberProvider = "CustomChangePhoneNumber"; //-> aqui eu removo a obrigatoriedade de ter um token para mudar o telefone padrão
    //options.Tokens.DefaultChangeEmailProvider = "CustomChangeEmail"; //-> aqui eu removo a obrigatoriedade de ter um token para mudar o email padrão
    //options.Tokens.DefaultPasswordResetProvider = "CustomPasswordReset"; //-> aqui eu removo a obrigatoriedade de ter um token para resetar a senha padrão
    //options.Tokens.DefaultEmailConfirmationProvider = "CustomEmailConfirmation"; //-> aqui eu removo a obrigatoriedade de ter um token para confirmar o email padrão
    //options.Tokens.DefaultPhoneConfirmationProvider = "CustomPhoneConfirmation"; //-> aqui eu removo a obrigatoriedade de ter um token para confirmar o telefone padrão
    //options.Tokens.DefaultAuthenticatorProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticador padrão
    //options.Tokens.DefaultTwoFactorRecoveryCodeProvider = "CustomTwoFactorRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recuperação de codigo de autenticação padrão
    //options.Tokens.DefaultRecoveryCodeProvider = "CustomRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recuperação de codigo padrão
    //options.Tokens.DefaultAuthenticatorProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticador padrão

}).AddEntityFrameworkStores<ApplicationDbContext>();

/*
 //Authorization de forma automatica (todos os endpoints vao precisar do token ao utilizar isso), com isso tambem não precisamos ficar passando esse código "[Authorize]" em todos os endpoints
 //Seria meio que uma politica de segurança default
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
      .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
      .RequireAuthenticatedUser()
      .Build();
    options.AddPolicy("EmployeePolicy", p =>
        p.RequireAuthenticatedUser().RequireClaim("EmployeeCode"));
});
    
 */

//Habilitando serviço de autorizaçao (1º passo para habilitar a autorização) (serviço disponivel pro app usar)
builder.Services.AddAuthorization(options =>
{
    //Politica de segurança (diferenciando admin de user normal, como se fosse uma Role)
    //Para que determinada rota seja acessada ela ira precisar dessa politicade segurança
    options.AddPolicy("EmployeePolicy", p =>
        p.RequireAuthenticatedUser()
        .RequireClaim("EmployeeCode"));// -> aqui eu digo que o usuario precisa ter a claim "EmployeeCode" para acessar essa rota

    //Aqui eu to criando uma politica de segurança para o usuario em especifico pode utilizar (as vezes queremos tem alguem com poder total, ai criamos essas policies utilitarias)
    //Employee0050Policy -> Aqui apenas o usuario que tiver o EmployeeCode 0050 pode acessar essa rota
    options.AddPolicy("Employee0050Policy", p =>
        p.RequireAuthenticatedUser()
        .RequireClaim("EmployeeCode", "0050"));// -> Alem do claim ele tem que ter o código(valor) 0050
});

//Informando ao .Net a forma que ele vai se autenticar (2º passo para habilitar a autorização) (serviço disponivel pro app usar)
builder.Services.AddAuthentication(x =>
{
    //Digo que vai se authenticar atraves do jwt
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
}).AddJwtBearer(options => //-> Digo que to trabalhando com jwt
{
    //Opções para validação (para saber se aquele token que eu to gerando e o mesmo que to recebendo)
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true, 
        ValidateAudience = true, //-> valida a audiencia
        ValidateIssuer = true, 
        ValidateLifetime = true, //-> valida ciclo de vida
        ValidateIssuerSigningKey = true, //-> valida chave de assinatura
        ClockSkew = TimeSpan.Zero, //-> Quanto tempo vamos aceitar o token expirado, aqui estamos dizendo 0 minutos de tolerancia(se eu nao passar esse código por default ele da 5 minutos de tolerancia a partir do momento que o token expira)
        ValidIssuer = builder.Configuration["JwtBearerTokenSettings:Issuer"], //-> configuração no appsettings global (Se a assinatura é igual a que eu to esperando, tipo chave publica e privada)
        ValidAudience = builder.Configuration["JwtBearerTokenSettings:Audience"], //-> configuração no appsettings global (Se a audiencia é igual a que eu to esperando, tipo o dominio)
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtBearerTokenSettings:SecretKey"])) //-> configuração no appsettings global (Se é a mesma secret key que eu to esperando)
    };
});

//Informando ao .net sobre nossas classes de serviço (injetor de dependencia, services ...)
//AddScoped -> quando a classe precisar ser instanciada em um metodo, e em quando dura a nossa requisição essa instancia estara na memoria, quando a requisição acabar ela é destruida
builder.Services.AddScoped<QueryAllUsersWithClaimName>();
builder.Services.AddScoped<UserCreator>(); //-> Classe de serviço tambem precisa ser declarada aqui !!


var app = builder.Build();

//Adicionando o middleware de autenticação (3º passo para habilitar a autorização) (habilitando pro app usar)
//Sempre utilizar nessa ordem
app.UseAuthentication();
app.UseAuthorization();


//Escondendo a logica do endpoint (cada arquivo sera responsavel por apenas 1 rota)
app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);
app.MapMethods(TokenPost.Template, TokenPost.Methods, TokenPost.Handle);
app.MapMethods(ProductPost.Template, ProductPost.Methods, ProductPost.Handle);
app.MapMethods(ProductGetAll.Template, ProductGetAll.Methods, ProductGetAll.Handle);
app.MapMethods(ProductGetShowCase.Template, ProductGetShowCase.Methods, ProductGetShowCase.Handle);
app.MapMethods(ClientsPost.Template, ClientsPost.Methods, ClientsPost.Handle);
app.MapMethods(ClientGet.Template, ClientGet.Methods, ClientGet.Handle);

//Configurando a execção (nosso manipulador de exceção é chamado colocando esse código)
///error -> rota que iremos criar para aprensentar o erro do tipo ExecptionHandler
app.UseExceptionHandler("/error");

//Podemos criar a rota direto aqui (podemos tratar varias coisas aqui)
app.Map("/error", (HttpContext http) => {

    var error = http.Features?.Get<IExceptionHandlerFeature>()?.Error;

    if (error != null)
    {
        if (error is SqlException)
            return Results.Problem(title: "Database out", statusCode: 500);
        else if (error is BadHttpRequestException)
            return Results.Problem(title: "Error to convert data to other type. See all the information sent", statusCode: 500);
        else if (error is NullReferenceException)
            return Results.Problem(title: "Error no json convert", statusCode: 500);
    }

    return Results.Problem(title: "An error ocurred", statusCode: 500);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
