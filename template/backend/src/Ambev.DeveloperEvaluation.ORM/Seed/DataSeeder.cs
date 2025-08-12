using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Common.Security;
using Bogus;
using Serilog; // Added for logging
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.ORM.Seed
{
    public class DataSeeder
    {
        private readonly IUserRepository _userRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public DataSeeder(IUserRepository userRepository, ISaleRepository saleRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _saleRepository = saleRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedDataAsync()
        {
            // Seed Users
            if ((await _userRepository.GetByEmailAsync("test@example.com", CancellationToken.None)) == null)
            {
                // Create a specific test user
                var testUser = new User
                {
                    Username = "Test User",
                    Email = "test@example.com",
                    Password = _passwordHasher.HashPassword("Password123!"),
                    Role = UserRole.Admin,
                    Status = UserStatus.Active,
                    Phone = "(11) 99999-9999"
                };
                await _userRepository.CreateAsync(testUser, CancellationToken.None);
                Log.Information("Test user 'test@example.com' created."); // Log message

                // Generate additional random users
                var users = GenerateUsers(9); // Generate 9 more users
                foreach (var user in users)
                {
                    await _userRepository.CreateAsync(user, CancellationToken.None);
                }
            }

            // Seed Sales
            if ((await _saleRepository.GetCountAsync(null, null, null, CancellationToken.None)) == 0)
            {
                var sales = GenerateSales(50);
                foreach (var sale in sales)
                {
                    await _saleRepository.CreateAsync(sale, CancellationToken.None);
                }
            }
        }

        private List<User> GenerateUsers(int count)
        {
            var userFaker = new Faker<User>()
                .CustomInstantiator(f =>
                {
                    var firstName = f.Name.FirstName();
                    var lastName = f.Name.LastName();
                    var email = f.Internet.Email(firstName, lastName);
                    var password = _passwordHasher.HashPassword("Password123!");
                    var role = f.PickRandom<UserRole>();
                    var status = f.PickRandom<UserStatus>();
                    var phone = f.Phone.PhoneNumber();
                    
                    return new User
                    {
                        Username = $"{firstName} {lastName}",
                        Email = email,
                        Password = password,
                        Role = role,
                        Status = status,
                        Phone = phone
                    };
                });

            return userFaker.Generate(count);
        }

        private List<Sale> GenerateSales(int count)
        {
            var saleFaker = new Faker<Sale>()
                .CustomInstantiator(f =>
                {
                    var sale = new Sale(
                        f.Company.CompanyName(), // Using company name as customer for variety
                        f.Address.City() // Using city as branch for simplicity
                    );

                    var numItems = f.Random.Int(1, 20); // Max 20 items per sale
                    for (int i = 0; i < numItems; i++)
                    {
                        var productName = f.Commerce.ProductName();
                        var quantity = f.Random.Int(1, 20); // Max 20 identical items
                        var unitPrice = f.Finance.Amount(10, 1000);
                        sale.AddItem(productName, quantity, unitPrice);
                    }
                    return sale;
                });

            return saleFaker.Generate(count);
        }
    }
}
