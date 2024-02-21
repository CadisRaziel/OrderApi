using Microsoft.AspNetCore.Identity;
using OrderApi.Endpoints.Employees;
using OrderApi.Entpoints.Categories;
using OrderApi.Infra.Data;
using OrderApi.Infra.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Conex�o com banco de dados
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["ConnectionStrings:OrderApiDb"]);


/*
//Quando vamos criar um usuario a senha tem varias verifica�oes como tem que ser maior que 6, tem que te numero, tem que te caracter especial
//Deixando dessa forma ele vai ser o padr�o igual eu disse acima
//Abaixo desse comentario veremos a mesma fun��o por�m com o options no construtor, e � ele que a gente remove se quiser o "padr�o" e deixa da forma que queremos
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
 */

//Configura��o do Identity (estamos usando o servi�o do identity junto com meu DbContext para ele entender como acessar o banco de dados)
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
    //options.Tokens.TwoFactorRecoveryCodeTokenProvider = "CustomTwoFactorRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recuperar a autentica��o
    //options.Tokens.DefaultEmailProvider = "CustomEmail"; //-> aqui eu removo a obrigatoriedade de ter um token para email padr�o
    //options.Tokens.DefaultPhoneProvider = "CustomPhone"; //-> aqui eu removo a obrigatoriedade de ter um token para telefone padr�o
    //options.Tokens.DefaultAuthenticatorProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticador padr�o
    //options.Tokens.DefaultRecoveryCodeProvider = "CustomRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recupera��o de codigo padr�o
    //options.Tokens.DefaultTwoFactorRecoveryCodeProvider = "CustomTwoFactorRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recupera��o de codigo de autentica��o padr�o
    //options.Tokens.DefaultChangePhoneNumberProvider = "CustomChangePhoneNumber"; //-> aqui eu removo a obrigatoriedade de ter um token para mudar o telefone padr�o
    //options.Tokens.DefaultChangeEmailProvider = "CustomChangeEmail"; //-> aqui eu removo a obrigatoriedade de ter um token para mudar o email padr�o
    //options.Tokens.DefaultPasswordResetProvider = "CustomPasswordReset"; //-> aqui eu removo a obrigatoriedade de ter um token para resetar a senha padr�o
    //options.Tokens.DefaultEmailConfirmationProvider = "CustomEmailConfirmation"; //-> aqui eu removo a obrigatoriedade de ter um token para confirmar o email padr�o
    //options.Tokens.DefaultPhoneConfirmationProvider = "CustomPhoneConfirmation"; //-> aqui eu removo a obrigatoriedade de ter um token para confirmar o telefone padr�o
    //options.Tokens.DefaultAuthenticatorProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticador padr�o
    //options.Tokens.DefaultTwoFactorRecoveryCodeProvider = "CustomTwoFactorRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recupera��o de codigo de autentica��o padr�o
    //options.Tokens.DefaultRecoveryCodeProvider = "CustomRecoveryCode"; //-> aqui eu removo a obrigatoriedade de ter um token para recupera��o de codigo padr�o
    //options.Tokens.DefaultAuthenticatorProvider = "CustomAuthenticator"; //-> aqui eu removo a obrigatoriedade de ter um token para autenticador padr�o

}).AddEntityFrameworkStores<ApplicationDbContext>();

//Injetando dependencia
//AddScoped -> quando a classe precisar ser instanciada em um metodo, e em quando dura a nossa requisi��o essa instancia estara na memoria, quando a requisi��o acabar ela � destruida
builder.Services.AddScoped<QueryAllUsersWithClaimName>();


var app = builder.Build();



//Escondendo a logica do endpoint (cada arquivo sera responsavel por apenas 1 rota)
app.MapMethods(CategoryPost.Template, CategoryPost.Methods, CategoryPost.Handle);
app.MapMethods(CategoryGetAll.Template, CategoryGetAll.Methods, CategoryGetAll.Handle);
app.MapMethods(CategoryPut.Template, CategoryPut.Methods, CategoryPut.Handle);
app.MapMethods(EmployeePost.Template, EmployeePost.Methods, EmployeePost.Handle);
app.MapMethods(EmployeeGetAll.Template, EmployeeGetAll.Methods, EmployeeGetAll.Handle);

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
