using E_commerce.Domain.Entities.BasketModule;
using E_commerce.Domain.Entities.IdentityModule;
using E_commerce.Domain.Entities.OrderModule;
using E_commerce.Domain.Entities.ProductModule;
using E_commerce.Persistence.Data.DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Persistence.Data.DataSeed
{
    /// <summary>
    /// Realistic demo data so the store (and later the admin dashboard) doesn't look empty:
    ///   dotnet run --project E-Commerce.Web -- --seed-demo
    /// - Creates 2 demo logins (admin@demo.store / customer@demo.store) + more customers, all with the
    ///   password from DemoData:Password (a secret: user-secrets locally, env var on the server).
    /// - Each customer gets a saved address and 0-6 orders spread over the last 90 days, with statuses that
    ///   make sense for their age (old orders are delivered, today's are pending...).
    /// - The demo customer also gets an open basket.
    /// Runs once: if the demo accounts already exist it does nothing.
    /// </summary>
    public static class DemoDataSeeding
    {
        public const string EmailDomain = "demo.store";
        public const string AdminEmail = "admin@demo.store";
        public const string CustomerEmail = "customer@demo.store";
        public const string DemoBasketId = "demo-customer-basket";

        public const int CustomerCount = 30; // including customer@demo.store
        public const int HistoryDays = 90;

        public static async Task<string> SeedAsync(
            StoreDbContext db,
            UserManager<ApplicationUser> userManager,
            string? password,
            DateTimeOffset utcNow,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "Demo data not seeded: set DemoData:Password.";

            if (await db.Users.AnyAsync(U => U.Email!.EndsWith("@" + EmailDomain), ct))
                return "Demo data already exists.";

            var products = await db.Products.OrderBy(P => P.ID).ToListAsync(ct);
            var deliveryMethods = await db.DeliveryMethods.OrderBy(D => D.ID).ToListAsync(ct);
            if (products.Count == 0 || deliveryMethods.Count == 0)
                return "Demo data not seeded: the catalog is empty (start the API once to seed it).";

            // All or nothing: UserManager uses this same DbContext, so the accounts are inside the transaction too.
            await using var transaction = await db.Database.BeginTransactionAsync(ct);

            var builder = new DemoBuilder(utcNow, products, deliveryMethods);

            await CreateAccountAsync(userManager, AdminEmail, "Demo Admin", AppRoles.Admin, password, utcNow.AddDays(-HistoryDays - 30), null);

            var customers = new List<ApplicationUser>();
            foreach (var person in builder.People(CustomerCount))
            {
                var user = await CreateAccountAsync(userManager, person.Email, person.FullName, AppRoles.Customer,
                    password, person.CreatedAt, person.Phone);
                user.Address = person.Address;
                customers.Add(user);
            }

            var orders = builder.Orders(customers);
            db.Orders.AddRange(orders);
            db.Baskets.Add(builder.Basket(customers[0]));
            await db.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return $"Demo data created: {customers.Count} customers, {orders.Count} orders "
                + $"({orders.Sum(O => O.Items.Count)} order lines), 1 open basket. Logins: {AdminEmail}, {CustomerEmail}.";
        }

        private static async Task<ApplicationUser> CreateAccountAsync(UserManager<ApplicationUser> userManager,
            string email, string displayName, string role, string password, DateTimeOffset createdAt, string? phone)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                DisplayName = displayName,
                PhoneNumber = phone,
                CreatedAt = createdAt,
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"Could not create the demo account {email}: " + string.Join(" ", result.Errors.Select(E => E.Description)));

            await userManager.AddToRoleAsync(user, role);
            return user;
        }

        private sealed record Person(string Email, string FullName, string Phone, DateTimeOffset CreatedAt, Address Address);

        /// <summary>Builds the objects in memory; SeedAsync saves them.</summary>
        private sealed class DemoBuilder(DateTimeOffset now, List<Product> products, List<DeliveryMethod> deliveryMethods)
        {
            private static readonly string[] MaleNames = ["Ahmed", "Mohamed", "Youssef", "Mostafa", "Mahmoud", "Hassan", "Amr", "Tarek", "Khaled", "Ali", "Ziad", "Hazem", "Sherif", "Adham", "Seif"];
            private static readonly string[] FemaleNames = ["Nour", "Mariam", "Salma", "Yasmin", "Habiba", "Farida", "Aya", "Rana", "Dina", "Menna", "Hana", "Laila", "Malak", "Reem", "Nada"];
            private static readonly string[] LastNames = ["Hassan", "Mahmoud", "Ibrahim", "Mostafa", "Adel", "Fathy", "Samir", "Nabil", "Gamal", "Saad", "Fouad", "Ashraf", "Salah", "Kamal", "Zaki"];
            private static readonly (string City, string Street)[] Streets =
            [
                ("Cairo", "Abbas El Akkad St"), ("Cairo", "Makram Ebeid St"), ("Cairo", "El Thawra St"), ("Cairo", "Road 9, Maadi"),
                ("Giza", "Faisal St"), ("Giza", "El Haram St"), ("Giza", "Gameat El Dowal St"), ("Alexandria", "Fouad St"),
                ("Alexandria", "El Geish Rd"), ("Mansoura", "El Gomhoreya St"), ("Tanta", "El Bahr St"),
            ];

            private readonly Random _random = new(2026); // fixed seed: the same data shape on every run
            private readonly HashSet<string> _emails = [];
            private readonly HashSet<string> _phones = [];

            public IEnumerable<Person> People(int count)
            {
                // The first one is the documented demo login.
                yield return NewPerson("Omar", "Hassan", CustomerEmail, now.AddDays(-HistoryDays - 10));

                for (var i = 1; i < count; i++)
                {
                    var female = i % 2 == 0;
                    var first = Pick(female ? FemaleNames : MaleNames);
                    var last = Pick(LastNames);
                    // Same name twice? → omar.hassan, omar.hassan2, omar.hassan3...
                    var baseName = $"{first}.{last}".ToLowerInvariant();
                    var name = baseName;
                    for (var n = 2; !_emails.Add(name); n++) name = baseName + n;
                    var email = $"{name}@{EmailDomain}";

                    // Accounts appear over time (more of them recently), so "new customers per month" isn't flat.
                    var createdDaysAgo = (int)(HistoryDays * 1.2 * Math.Pow(_random.NextDouble(), 0.8));
                    yield return NewPerson(first, last, email, now.AddDays(-createdDaysAgo).AddHours(-_random.Next(24)));
                }
            }

            private Person NewPerson(string first, string last, string email, DateTimeOffset createdAt)
            {
                var (city, street) = Pick(Streets);
                return new Person(email, $"{first} {last}", NewPhone(), createdAt, new Address
                {
                    FirstName = first,
                    LastName = last,
                    Country = "Egypt",
                    City = city,
                    Street = $"{_random.Next(1, 120)} {street}",
                });
            }

            public List<Order> Orders(List<ApplicationUser> customers)
            {
                var orders = new List<Order>();
                foreach (var customer in customers)
                {
                    // The demo customer always has a full history to look at; others have 0-6 orders.
                    var count = customer.Email == CustomerEmail ? 6 : _random.Next(0, 7);
                    var firstDay = Math.Min(HistoryDays, (now - customer.CreatedAt).TotalDays);
                    if (firstDay < 1) continue;

                    for (var i = 0; i < count; i++)
                    {
                        var isDemoToday = customer.Email == CustomerEmail && i == 0;
                        var daysAgo = isDemoToday
                            ? 0.2                                     // one order from today
                            : _random.NextDouble() * firstDay;
                        var order = NewOrder(customer, now.AddDays(-daysAgo));

                        // The first thing a visitor sees in "My orders" should look healthy, not "PaymentFailed".
                        if (isDemoToday)
                            order.Status = order.PaymentMethod == PaymentMethod.Card ? OrderStatus.PaymentReceived : OrderStatus.Pending;

                        orders.Add(order);
                    }
                }
                return orders;
            }

            private Order NewOrder(ApplicationUser customer, DateTimeOffset date)
            {
                var payment = _random.NextDouble() < 0.55 ? PaymentMethod.Card : PaymentMethod.CashOnDelivery;
                var delivery = PickDelivery();

                var items = products
                    .OrderBy(_ => _random.Next())
                    .Take(_random.Next(1, 4))
                    .Select(P => new OrderItem
                    {
                        // A snapshot: the order keeps the name/price it was bought at, even if the product changes later.
                        Product = new ProductItemOrdered { ProductId = P.ID, ProductName = P.Name, PictureUrl = P.PictureUrl },
                        Price = P.Price,
                        Quantity = _random.NextDouble() < 0.8 ? 1 : 2,
                    })
                    .ToList();

                var address = customer.Address!;
                return new Order
                {
                    UserId = customer.Id,
                    UserEmail = customer.Email!,
                    OrderDate = date,
                    PaymentMethod = payment,
                    Status = StatusFor(now - date, payment),
                    PaymentIntentId = payment == PaymentMethod.Card ? $"pi_demo_{Guid.NewGuid():N}"[..27] : null,
                    DeliveryMethodId = delivery.ID,
                    Address = new OrderAddress
                    {
                        FirstName = address.FirstName,
                        LastName = address.LastName,
                        Street = address.Street,
                        City = address.City,
                        Country = address.Country,
                    },
                    Items = items,
                    SubTotal = items.Sum(I => I.Price * I.Quantity),
                };
            }

            /// <summary>Old orders are finished; recent ones are still on their way.</summary>
            private OrderStatus StatusFor(TimeSpan age, PaymentMethod payment)
            {
                var roll = _random.NextDouble();
                if (age.TotalDays > 7)
                    return roll < 0.92 ? OrderStatus.Delivered : OrderStatus.Cancelled;
                if (age.TotalDays > 2)
                    return roll < 0.5 ? OrderStatus.Shipped : roll < 0.9 ? OrderStatus.Delivered : OrderStatus.Cancelled;
                if (payment == PaymentMethod.CashOnDelivery)
                    return OrderStatus.Pending; // paid at the door, nothing to confirm yet
                return roll < 0.88 ? OrderStatus.PaymentReceived : OrderStatus.PaymentFailed;
            }

            public CustomerBasket Basket(ApplicationUser owner) => new()
            {
                ID = DemoBasketId,
                UserId = owner.Id,
                DeliveryMethodId = deliveryMethods.First().ID,
                UpdatedAt = now,
                Items = products.Where(P => P.Stock > 0).Take(2)
                    .Select(P => new BasketItem { ProductId = P.ID, Quantity = 1 })
                    .ToList(),
            };

            private DeliveryMethod PickDelivery()
            {
                // Most people pick the cheap-but-not-slowest option; few pay for express.
                var roll = _random.NextDouble();
                var index = roll < 0.25 ? 0 : roll < 0.70 ? 1 : roll < 0.85 ? 2 : 3;
                return deliveryMethods[Math.Min(index, deliveryMethods.Count - 1)];
            }

            private string NewPhone()
            {
                string phone;
                do phone = Pick(["010", "011", "012", "015"]) + _random.Next(10_000_000, 99_999_999);
                while (!_phones.Add(phone));
                return phone;
            }

            private T Pick<T>(T[] items) => items[_random.Next(items.Length)];
        }
    }
}
