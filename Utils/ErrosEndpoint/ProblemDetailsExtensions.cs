using Flunt.Notifications;
using Microsoft.AspNetCore.Identity;

namespace OrderApi.Utils.ErrosEndpoint   
{
    public static class ProblemDetailsExtensions
    {
        //Olhe que interessante assim como o java podemos ter o mesmo nome de classe(o que difere é seus parametros)
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> nottificantions)
        {
            return nottificantions.GroupBy(g => g.Key) //-> Agrupamos as keys (eu passo elas na entidade ".IsNotNullOrEmpty(createdBy, "createdBy", "createdBy error")" createdBy em "" é uma key
                    .ToDictionary(g => g.Key, g => g.Select(s => s.Message) //-> pego a chave e o valor e passo pro dictonary
                    .ToArray()); //-> transformo em um array
        }
        
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IEnumerable<IdentityError> error)
        {

            /*
             //Caso eu queira retornar só o 'Error'
             var dictionary = new Dictionary<string, string[]>();
             dictionary.Add("Error", error.Select(s => s.Description).ToArray());
             return dictionary;
             */

            return error.GroupBy(g => g.Code) 
                    .ToDictionary(g => g.Key, g => g.Select(s => s.Description) 
                    .ToArray()); 
        }
    }
}
