using System;
using System.Collections.Generic;

class Transaction
{
    public string Type { get; }
    public decimal Amount { get; }
    public DateTime Date { get; }

    public Transaction(string type, decimal amount)
    {
        Type = type;
        Amount = amount;
        Date = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Date:yyyy-MM-dd HH:mm:ss} - {Type}: {Amount} zł";
    }
}

class Account
{
    public string AccountId { get; }
    public string Owner { get; }
    public decimal Balance { get; private set; }
    private List<Transaction> transactions = new List<Transaction>();

    public Account(string accountId, string owner, decimal initialBalance)
    {
        AccountId = accountId;
        Owner = owner;
        Balance = initialBalance;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        transactions.Add(new Transaction("Wpłata", amount));
        Console.WriteLine($"Zaksięgowano wpłatę: {amount} zł");
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= Balance)
        {
            Balance -= amount;
            transactions.Add(new Transaction("Wypłata", amount));
            Console.WriteLine($"Zaksięgowano wypłatę: {amount} zł");
        }
        else
        {
            Console.WriteLine("Brak wystarczających środków!");
        }
    }

    public void Transfer(decimal amount, Account recipient)
    {
        if (amount <= Balance)
        {
            Withdraw(amount);
            recipient.Deposit(amount);
            transactions.Add(new Transaction($"Przelew do {recipient.Owner}", amount));
            recipient.transactions.Add(new Transaction($"Przelew od {Owner}", amount));
            Console.WriteLine($"Przelew {amount} zł do {recipient.Owner} zakończony sukcesem.");
        }
        else
        {
            Console.WriteLine("Brak wystarczających środków!");
        }
    }

    public void ShowTransactionHistory()
    {
        Console.WriteLine($"\nHistoria transakcji dla konta {Owner}:");
        foreach (var transaction in transactions)
        {
            Console.WriteLine(transaction);
        }
    }
}

class Bank
{
    private Dictionary<string, Account> accounts = new Dictionary<string, Account>();

    public void CreateAccount(string accountId, string owner, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(accountId) || !Program.IsNumeric(accountId))
        {
            Console.WriteLine("Błąd: Numer konta musi składać się wyłącznie z cyfr!");
            return;
        }
        if (accounts.ContainsKey(accountId))
        {
            Console.WriteLine($"Błąd: Konto o numerze {accountId} już istnieje! Wybierz inny numer.");
            return;
        }
        if (initialBalance < 0)
        {
            Console.WriteLine("Błąd: Saldo początkowe nie może być ujemne!");
            return;
        }

        accounts[accountId] = new Account(accountId, owner, initialBalance);
        Console.WriteLine($"Utworzono konto dla {owner} z numerem {accountId} i saldem {initialBalance} zł.");
    }

    public Account GetAccount(string accountId)
    {
        accounts.TryGetValue(accountId, out var account);
        return account;
    }

    public void ListAccounts()
    {
        if (accounts.Count > 0)
        {
            Console.WriteLine("\nLista wszystkich kont w banku:");
            foreach (var account in accounts.Values)
            {
                Console.WriteLine($"Numer konta: {account.AccountId}, Właściciel: {account.Owner}, Saldo: {account.Balance} zł");
            }
        }
        else
        {
            Console.WriteLine("\nBrak kont w banku.");
        }
    }
}

class Program
{
    public static bool IsNumeric(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsDigit(c))
                return false;
        }
        return true;
    }

    static void Main()
    {
        Bank bank = new Bank();

        while (true)
        {
            Console.WriteLine("\n--- System Bankowy ---");
            Console.WriteLine("1. Utwórz konto");
            Console.WriteLine("2. Wpłata");
            Console.WriteLine("3. Wypłata");
            Console.WriteLine("4. Przelew");
            Console.WriteLine("5. Historia transakcji");
            Console.WriteLine("6. Lista kont");
            Console.WriteLine("0. Zakończ");

            Console.Write("Wybierz opcję: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("Podaj numer konta: ");
                    string accountId = Console.ReadLine().Trim();
                    if (!IsNumeric(accountId))
                    {
                        Console.WriteLine("Błąd: Numer konta musi składać się wyłącznie z cyfr!");
                        break;
                    }
                    if (bank.GetAccount(accountId) != null)
                    {
                        Console.WriteLine($"Błąd: Konto o numerze {accountId} już istnieje! Wybierz inny numer.");
                        break;
                    }
                    Console.Write("Podaj nazwisko właściciela: ");
                    string owner = Console.ReadLine();
                    Console.Write("Podaj początkowe saldo: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal initialBalance))
                    {
                        Console.WriteLine("Błąd: Saldo początkowe musi być liczbą!");
                        break;
                    }
                    bank.CreateAccount(accountId, owner, initialBalance);
                    break;

                case "2":
                    Console.Write("Podaj numer konta: ");
                    Account accountDeposit = bank.GetAccount(Console.ReadLine());
                    if (accountDeposit != null)
                    {
                        Console.Write("Podaj kwotę wpłaty: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal depositAmount))
                            accountDeposit.Deposit(depositAmount);
                    }
                    else Console.WriteLine("Konto nie istnieje.");
                    break;

                case "3":
                    Console.Write("Podaj numer konta: ");
                    Account accountWithdraw = bank.GetAccount(Console.ReadLine());
                    if (accountWithdraw != null)
                    {
                        Console.Write("Podaj kwotę wypłaty: ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal withdrawAmount))
                            accountWithdraw.Withdraw(withdrawAmount);
                    }
                    else Console.WriteLine("Konto nie istnieje.");
                    break;

                case "4":
                    Console.Write("Podaj numer konta: ");
                    Account senderAccount = bank.GetAccount(Console.ReadLine());
                    if (senderAccount != null)
                    {
                        Console.Write("Podaj numer konta odbiorcy: ");
                        Account recipientAccount = bank.GetAccount(Console.ReadLine());
                        if (recipientAccount != null)
                        {
                            Console.Write("Podaj kwotę przelewu: ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal transferAmount))
                                senderAccount.Transfer(transferAmount, recipientAccount);
                        }
                        else Console.WriteLine("Konto odbiorcy nie istnieje.");
                    }
                    else Console.WriteLine("Konto nie istnieje.");
                    break;

                case "5":
                    Console.Write("Podaj numer konta: ");
                    Account accountHistory = bank.GetAccount(Console.ReadLine());
                    if (accountHistory != null)
                        accountHistory.ShowTransactionHistory();
                    else Console.WriteLine("Konto nie istnieje.");
                    break;

                case "6":
                    bank.ListAccounts();
                    break;

                case "0":
                    Console.WriteLine("Zakończono działanie systemu bankowego.");
                    return;

                default:
                    Console.WriteLine("Niepoprawny wybór, spróbuj ponownie.");
                    break;
            }
        }
    }
}