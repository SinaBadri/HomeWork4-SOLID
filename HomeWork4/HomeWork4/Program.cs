using System;
using System.Collections.Generic;

// -------------------- Abstractions --------------------

// اصل DIP: وابستگی به abstraction
public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal price);
}

public interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

// -------------------- Discount Implementations --------------------

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percent;

    public PercentageDiscount(decimal percent)
    {
        _percent = percent;
    }

    public decimal ApplyDiscount(decimal price)
    {
        return price - (price * _percent / 100);
    }
}

public class FixedAmountDiscount : IDiscountStrategy
{
    private readonly decimal _amount;

    public FixedAmountDiscount(decimal amount)
    {
        _amount = amount;
    }

    public decimal ApplyDiscount(decimal price)
    {
        return price - _amount;
    }
}

// -------------------- Email Service --------------------

public class EmailService : IEmailService
{
    public void SendEmail(string to, string subject, string body)
    {
        Console.WriteLine("Email sent.");
    }
}

// -------------------- Domain --------------------

public class Product
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class Invoice
{
    public List<Product> Products { get; set; } = new List<Product>();
    public decimal TotalPrice { get; set; }
}

// -------------------- Service --------------------

// وابستگی‌ها از بیرون تزریق می‌شوند (Constructor Injection)
public class PurchaseService
{
    private readonly IDiscountStrategy _discountStrategy;
    private readonly IEmailService _emailService;

    public PurchaseService(IDiscountStrategy discountStrategy, IEmailService emailService)
    {
        _discountStrategy = discountStrategy;
        _emailService = emailService;
    }

    public Invoice CreateInvoice(List<Product> products, string customerEmail)
    {
        decimal total = 0;

        foreach (var product in products)
        {
            total += _discountStrategy.ApplyDiscount(product.Price);
        }

        var invoice = new Invoice
        {
            Products = products,
            TotalPrice = total
        };

        // ارسال ایمیل تأیید
        _emailService.SendEmail(customerEmail, "Invoice Confirmation", "Your invoice is ready.");

        return invoice;
    }
}

// -------------------- Usage --------------------

class Program
{
    static void Main()
    {
        var products = new List<Product>
        {
            new Product { Name = "Product 1", Price = 100 },
            new Product { Name = "Product 2", Price = 200 }
        };

        // انتخاب نوع تخفیف بدون تغییر در سرویس
        IDiscountStrategy discount = new PercentageDiscount(10);
        // IDiscountStrategy discount = new FixedAmountDiscount(20);

        IEmailService emailService = new EmailService();

        var purchaseService = new PurchaseService(discount, emailService);
        var invoice = purchaseService.CreateInvoice(products, "customer@email.com");

        Console.WriteLine($"Total Price: {invoice.TotalPrice}");
    }
}
