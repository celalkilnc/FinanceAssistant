# FinanceAssistant.API

FinanceAssistant.API is a personal finance management ASP.NET Core Web API. It enables users to manage their expenses, incomes, bills, cards, installments, invoices, and notifications while providing JWT-based authentication and comprehensive financial reporting.

## Features

### Database Structure
- **PostgreSQL** database
- **Entity Framework Core** with Code-First approach
- **Identity Tables** for user management
- **Core Tables**:
  - **Regular Incomes**
  - **Invoices**
  - **Expenses**
  - **Bills**
  - **Cards**
  - **Installments**
  - **Notifications**
  - **Financial Reports**
  - **UserSettings**

### Authentication and Security
- **JWT-based authentication**
- Login with **username/password**
- Role-based authorization ("User" role available)
- Security settings management via UserSettings

### API Endpoints

#### Auth Controller
- `POST /api/auth/register`: Register a new user
- `POST /api/auth/login`: Log in

#### Regular Incomes Controller
- `GET /api/regularincomes`: Retrieve all regular incomes
- `GET /api/regularincomes/{id}`: Get regular income details
- `POST /api/regularincomes`: Add a new regular income
- `PUT /api/regularincomes/{id}`: Update a regular income
- `DELETE /api/regularincomes/{id}`: Delete a regular income

#### Invoices Controller
- `GET /api/invoices`: Retrieve all invoices
- `GET /api/invoices/unpaid`: Retrieve unpaid invoices
- `GET /api/invoices/recurring`: Retrieve recurring invoices
- `GET /api/invoices/byType/{type}`: Retrieve invoices by type
- `POST /api/invoices`: Add a new invoice
- `PUT /api/invoices/{id}`: Update an invoice
- `PUT /api/invoices/{id}/pay`: Pay an invoice
- `DELETE /api/invoices/{id}`: Delete an invoice

#### Expenses Controller
- `GET /api/expenses`: Retrieve all expenses
- `GET /api/expenses/{id}`: Get expense details
- `POST /api/expenses`: Add a new expense
- `PUT /api/expenses/{id}`: Update an expense
- `DELETE /api/expenses/{id}`: Delete an expense

#### Bills Controller
- `GET /api/bills`: Retrieve all bills
- `GET /api/bills/unpaid`: Retrieve unpaid bills
- `GET /api/bills/{id}`: Get bill details
- `POST /api/bills`: Add a new bill
- `PUT /api/bills/{id}`: Update a bill
- `PUT /api/bills/{id}/pay`: Pay a bill
- `DELETE /api/bills/{id}`: Delete a bill

#### Cards Controller
- `GET /api/cards`: Retrieve all cards
- `GET /api/cards/{id}`: Get card details
- `POST /api/cards`: Add a new card
- `PUT /api/cards/{id}`: Update a card
- `DELETE /api/cards/{id}`: Delete a card

#### Installments Controller
- `GET /api/installments`: Retrieve all installments
- `GET /api/installments/active`: Retrieve active installments
- `GET /api/installments/{id}`: Get installment details
- `GET /api/installments/card/{cardId}`: Retrieve installments by card
- `POST /api/installments`: Add a new installment
- `PUT /api/installments/{id}`: Update an installment
- `PUT /api/installments/{id}/increment`: Increment an installment amount
- `DELETE /api/installments/{id}`: Delete an installment

#### Notifications Controller
- `GET /api/notifications`: Retrieve all notifications
- `GET /api/notifications/unread`: Retrieve unread notifications
- `GET /api/notifications/important`: Retrieve important notifications
- `GET /api/notifications/scheduled`: Retrieve scheduled notifications
- `PUT /api/notifications/{id}/read`: Mark a notification as read
- `PUT /api/notifications/markAllRead`: Mark all notifications as read
- `DELETE /api/notifications/{id}`: Delete a notification
- `DELETE /api/notifications/clearAll`: Clear all notifications

#### Financial Reports Controller
- `GET /api/financialreports`: Retrieve all reports
- `GET /api/financialreports/monthly/{period}`: Retrieve monthly report
- `GET /api/financialreports/annual/{year}`: Retrieve annual report
- `GET /api/financialreports/custom`: Retrieve custom reports for a date range
- `DELETE /api/financialreports/{id}`: Delete a report

### Special Features
- **Regular income tracking** with automatic payment date calculation
- **Invoice management** with recurring payment support
- **Bill payment tracking** with notification system
- **Installments and card management**
- **Comprehensive financial reporting** with monthly and annual analysis
- **Notification system** for important events and due dates
- **User settings management** for personalization

### Technical Details
- **.NET 8.0**
- **Entity Framework Core 8.0**
- **PostgreSQL** database
- **JWT authentication**
- **CORS configuration**: Allows all origins (development only)

### Client Application Requirements
1. **User authentication and profile management**
2. **Dashboard with financial overview**
3. **Regular income and invoice management**
4. **Expense and bill tracking**
5. **Card and installment monitoring**
6. **Financial reporting and analytics**
7. **Notification management**
8. **User settings customization**

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 15 or later
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run the following commands:
```bash
cd FinanceAssistant.API
dotnet restore
dotnet ef database update
dotnet run
```

### Development
- API runs on `http://localhost:5025` by default
- Swagger UI available at `/swagger`
- Use Postman collection for testing (available in repository)

## Documentation
Detailed API documentation is available in the `API_DOCUMENTATION.md` file, which includes:
- Complete endpoint descriptions
- Request/response examples
- Client implementation guidelines
- Security considerations
- Performance optimization tips

