using Microsoft.EntityFrameworkCore;

namespace SobMedidaApi.Models
{
    [Owned]
    public class Address
    {
        public string Street { get; set; } = string.Empty;       // Logradouro
        public string Number { get; set; } = string.Empty;       // Número
        public string Neighborhood { get; set; } = string.Empty; // Bairro
        public string City { get; set; } = string.Empty;         // Cidade
        public string State { get; set; } = string.Empty;        // UF
        public string ZipCode { get; set; } = string.Empty;      // CEP
    }
}