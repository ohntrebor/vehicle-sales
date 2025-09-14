using VehicleSales.Domain.Entities;
using VehicleSales.Infrastructure.Data;

namespace VehicleSales.Infrastructure.Seeders;

/// <summary>
/// Classe responsável por popular o banco de dados com dados iniciais de veículos
/// </summary>
public static class VehicleSeedData
{
    /// <summary>
    /// Popula o banco de dados com veículos mockados
    /// </summary>
    /// <param name="context">Contexto do banco de dados</param>
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        Console.WriteLine("🔍 Verificando se existem veículos no banco...");
        
        // Verifica se já existem veículos no banco
        var existingVehicles = context.Vehicles.Count();
        Console.WriteLine($"📊 Veículos encontrados no banco: {existingVehicles}");
        
        if (existingVehicles > 0)
        {
            Console.WriteLine("⏭️  Dados já existem, pulando seed.");
            return; // Dados já existem
        }
        
        Console.WriteLine("🌱 Iniciando seed de dados...");

        var vehicles = new List<Vehicle>
        {
            // Veículos disponíveis para venda - Populares
            new Vehicle("Toyota", "Corolla", 2022, "Branco", 85000.00m),
            new Vehicle("Honda", "Civic", 2021, "Prata", 95000.00m),
            new Vehicle("Hyundai", "HB20", 2022, "Vermelho", 65000.00m),
            new Vehicle("Ford", "Ka", 2021, "Azul", 55000.00m),
            new Vehicle("Chevrolet", "Onix", 2023, "Branco", 70000.00m),
            new Vehicle("Nissan", "Kicks", 2022, "Prata", 88000.00m),
            new Vehicle("Renault", "Sandero", 2021, "Amarelo", 62000.00m),
            new Vehicle("Fiat", "Argo", 2022, "Verde", 68000.00m),
            new Vehicle("Peugeot", "208", 2023, "Cinza", 75000.00m),
            new Vehicle("Volkswagen", "Polo", 2022, "Azul", 72000.00m),
            
            // Veículos SUVs/Utilitários
            new Vehicle("Jeep", "Renegade", 2022, "Laranja", 125000.00m),
            new Vehicle("Mitsubishi", "ASX", 2021, "Prata", 115000.00m),
            new Vehicle("Honda", "HR-V", 2023, "Preto", 135000.00m),
            new Vehicle("Hyundai", "Creta", 2022, "Branco", 118000.00m),
            new Vehicle("Volkswagen", "T-Cross", 2021, "Cinza", 108000.00m),
            
            // Veículos Premium
            new Vehicle("BMW", "320i", 2022, "Preto", 180000.00m),
            new Vehicle("Mercedes-Benz", "C200", 2021, "Branco", 220000.00m),
            new Vehicle("Audi", "A3", 2023, "Azul", 165000.00m),
            new Vehicle("Volvo", "XC40", 2022, "Cinza", 195000.00m),
            new Vehicle("Lexus", "UX250h", 2021, "Prata", 210000.00m)
        };

        // Adiciona alguns veículos vendidos para demonstração
        var soldVehicles = new List<Vehicle>
        {
            new Vehicle("Toyota", "Camry", 2020, "Preto", 120000.00m),
            new Vehicle("Honda", "Accord", 2019, "Branco", 110000.00m),
            new Vehicle("Volkswagen", "Passat", 2021, "Azul", 135000.00m),
            new Vehicle("Chevrolet", "Cruze", 2020, "Prata", 98000.00m),
            new Vehicle("Ford", "Fusion", 2019, "Vermelho", 92000.00m),
            new Vehicle("Nissan", "Sentra", 2020, "Cinza", 85000.00m),
            new Vehicle("Hyundai", "Elantra", 2021, "Branco", 88000.00m)
        };

        // Combina todos os veículos
        var allVehicles = vehicles.Concat(soldVehicles).ToList();
        
        Console.WriteLine($"💾 Salvando {allVehicles.Count} veículos no banco...");

        // Adiciona ao contexto
        await context.Vehicles.AddRangeAsync(allVehicles);
        var savedCount = await context.SaveChangesAsync();

        Console.WriteLine($"✅ Seed realizado com sucesso!");
        Console.WriteLine($"   📊 {vehicles.Count} veículos disponíveis");
        Console.WriteLine($"   💰 {soldVehicles.Count} veículos vendidos"); 
        Console.WriteLine($"   🎯 Total salvo: {savedCount} registros");
        Console.WriteLine($"   🎯 Total esperado: {allVehicles.Count} veículos");
    }

    /// <summary>
    /// Gera um CPF fake válido para fins de demonstração
    /// </summary>
    private static string GenerateFakeCpf()
    {
        var random = new Random();
        var cpfNumbers = new int[11];
        
        // Gera os primeiros 9 dígitos
        for (int i = 0; i < 9; i++)
        {
            cpfNumbers[i] = random.Next(0, 9);
        }
        
        // Calcula o primeiro dígito verificador
        int sum = 0;
        for (int i = 0; i < 9; i++)
        {
            sum += cpfNumbers[i] * (10 - i);
        }
        int remainder = sum % 11;
        cpfNumbers[9] = remainder < 2 ? 0 : 11 - remainder;
        
        // Calcula o segundo dígito verificador
        sum = 0;
        for (int i = 0; i < 10; i++)
        {
            sum += cpfNumbers[i] * (11 - i);
        }
        remainder = sum % 11;
        cpfNumbers[10] = remainder < 2 ? 0 : 11 - remainder;
        
        return string.Join("", cpfNumbers);
    }
}