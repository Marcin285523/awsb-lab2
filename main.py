import datetime

class Transaction:
    def __init__(self, transaction_type, amount, date=None):
        self.transaction_type = transaction_type
        self.amount = amount
        self.date = date if date else datetime.datetime.now()

    def __str__(self):
        return f"{self.date.strftime('%Y-%m-%d %H:%M:%S')} - {self.transaction_type}: {self.amount} zł"

class Account:
    def __init__(self, account_id, owner, balance=0):
        self.account_id = account_id
        self.owner = owner
        self.balance = balance
        self.transactions = []

    def deposit(self, amount):
        self.balance += amount
        transaction = Transaction("Wpłata", amount)
        self.transactions.append(transaction)
        print(f"Zaksięgowano wpłatę: {amount} zł")

    def withdraw(self, amount):
        if amount <= self.balance:
            self.balance -= amount
            transaction = Transaction("Wypłata", amount)
            self.transactions.append(transaction)
            print(f"Zaksięgowano wypłatę: {amount} zł")
        else:
            print("Brak wystarczających środków!")

    def transfer(self, amount, recipient_account):
        if amount <= self.balance:
            self.withdraw(amount)
            recipient_account.deposit(amount)
            transaction = Transaction(f"Przelew do {recipient_account.owner}", amount)
            self.transactions.append(transaction)
            recipient_account.transactions.append(Transaction(f"Przelew od {self.owner}", amount))
            print(f"Przelew {amount} zł do {recipient_account.owner} zakończony sukcesem.")
        else:
            print("Brak wystarczających środków!")

    def show_transaction_history(self):
        print(f"\nHistoria transakcji dla konta {self.owner}:")
        for transaction in self.transactions:
            print(transaction)

class Bank:
    def __init__(self):
        self.accounts = {}

    def create_account(self, account_id, owner, initial_balance):
        if not account_id.strip() or not account_id.isdigit():
            print("Błąd: Numer konta musi składać się wyłącznie z cyfr!")
            return
        if account_id in self.accounts:
            print("Błąd: Numer konta już istnieje! Spróbuj ponownie.")
            return
        
        try:
            initial_balance = float(initial_balance)
            if initial_balance < 0:
                print("Błąd: Saldo początkowe nie może być ujemne!")
                return
        except ValueError:
            print("Błąd: Saldo początkowe musi być liczbą!")
            return
        
        self.accounts[account_id] = Account(account_id, owner, initial_balance)
        print(f"Utworzono konto dla {owner} z numerem {account_id} i saldem {initial_balance} zł.")

    def get_account(self, account_id):
        return self.accounts.get(account_id)

    def list_accounts(self):
        if self.accounts:
            print("\nLista wszystkich kont w banku:")
            for account_id, account in self.accounts.items():
                print(f"Numer konta: {account_id}, Właściciel: {account.owner}, Saldo: {account.balance} zł")
        else:
            print("\nBrak kont w banku.")

def main():
    bank = Bank()

    while True:
        print("\n--- System Bankowy ---")
        print("1. Utwórz konto")
        print("2. Wpłata")
        print("3. Wypłata")
        print("4. Przelew")
        print("5. Historia transakcji")
        print("6. Lista kont")
        print("0. Zakończ")
        
        choice = input("Wybierz opcję: ")

        if choice == "1":
            account_id = input("Podaj numer konta: ").strip()
            if not account_id or not account_id.isdigit():
                print("Błąd: Numer konta musi składać się wyłącznie z cyfr!")
                continue
            owner = input("Podaj nazwisko właściciela: ")
            initial_balance = input("Podaj początkowe saldo: ")
            bank.create_account(account_id, owner, initial_balance)

        elif choice in ["2", "3", "4", "5"]:
            account_id = input("Podaj numer konta: ")
            account = bank.get_account(account_id)
            
            if account:
                if choice == "2":
                    amount = float(input("Podaj kwotę wpłaty: "))
                    account.deposit(amount)
                elif choice == "3":
                    amount = float(input("Podaj kwotę wypłaty: "))
                    account.withdraw(amount)
                elif choice == "4":
                    recipient_id = input("Podaj numer konta odbiorcy: ")
                    recipient_account = bank.get_account(recipient_id)
                    if recipient_account:
                        amount = float(input("Podaj kwotę przelewu: "))
                        account.transfer(amount, recipient_account)
                    else:
                        print("Konto odbiorcy nie istnieje.")
                elif choice == "5":
                    account.show_transaction_history()
            else:
                print("Konto nie istnieje.")

        elif choice == "6":
            bank.list_accounts()

        elif choice == "0":
            print("Zakończono działanie systemu bankowego.")
            break

        else:
            print("Niepoprawny wybór, spróbuj ponownie.")

if __name__ == "__main__":
    main()