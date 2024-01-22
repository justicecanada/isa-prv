using System.Text.Json.Serialization;

namespace ContainerAppsAuth.Auth
{
    
    public class MsClientPrincipal
    {

        [JsonPropertyName("auth_typ")]
        public string? AuthenticationType { get; set; }
        [JsonPropertyName("claims")]
        public IEnumerable<UserClaim> Claims { get; set; } = Array.Empty<UserClaim>();
        [JsonPropertyName("name_typ")]
        public string? NameType { get; set; }
        [JsonPropertyName("role_typ")]
        public string? RoleType { get; set; }

    }

}
