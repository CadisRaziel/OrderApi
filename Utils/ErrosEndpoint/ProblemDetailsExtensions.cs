using Flunt.Notifications;

namespace OrderApi.Utils.ErrosEndpoint   
{
    public static class ProblemDetailsExtensions
    {
        public static Dictionary<string, string[]> ConvertToProblemDetails(this IReadOnlyCollection<Notification> nottificantions)
        {
            return nottificantions.GroupBy(g => g.Key) //-> Agrupamos as keys (eu passo elas na entidade ".IsNotNullOrEmpty(createdBy, "createdBy", "createdBy error")" createdBy em "" é uma key
                    .ToDictionary(g => g.Key, g => g.Select(s => s.Message) //-> pego a chave e o valor e passo pro dictonary
                    .ToArray()); //-> transformo em um array
        }
    }
}
