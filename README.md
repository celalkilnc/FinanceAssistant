# FinanceAssistant.API

FinanceAssistant.API is a personal finance management ASP.NET Core Web API. It enables users to manage their expenses, incomes, bills, cards, and installments while providing JWT-based authentication and user settings management.

## Features

### Database Structure
- **SQL Server** database
- **Entity Framework Core** with Code-First approach
- **Identity Tables** for user management
- **Core Tables**:
  - **Expenses**
  - **Bills**
  - **Incomes**
  - **Cards**
  - **Installments**
  - **UserSettings**

### Authentication and Security
- **JWT-based authentication**
- Login with **username/password**
- Role-based authorization ("User" role available)

### API Endpoints
#### Auth Controller
- `POST /api/auth/register`: Register a new user
- `POST /api/auth/login`: Log in

#### Expenses Controller
- `GET /api/expenses`: Retrieve all expenses
- `GET /api/expenses/{expenseId}`: Get expense details
- `POST /api/expenses`: Add a new expense
- `PUT /api/expenses/{expenseId}`: Update an expense
- `DELETE /api/expenses/{expenseId}`: Delete an expense

#### Bills Controller
- `GET /api/bills`: Retrieve all bills
- `GET /api/bills/unpaid`: Retrieve unpaid bills
- `GET /api/bills/{billId}`: Get bill details
- `POST /api/bills`: Add a new bill
- `PUT /api/bills/{billId}`: Update a bill
- `PUT /api/bills/{billId}/pay`: Pay a bill
- `DELETE /api/bills/{billId}`: Delete a bill

#### Incomes Controller
- `GET /api/incomes`: Retrieve all incomes
- `GET /api/incomes/recurring`: Retrieve recurring incomes
- `GET /api/incomes/{incomeId}`: Get income details
- `POST /api/incomes`: Add a new income
- `PUT /api/incomes/{incomeId}`: Update an income
- `DELETE /api/incomes/{incomeId}`: Delete an income

#### Cards Controller
- `GET /api/cards`: Retrieve all cards
- `GET /api/cards/{cardId}`: Get card details
- `POST /api/cards`: Add a new card
- `PUT /api/cards/{cardId}`: Update a card
- `DELETE /api/cards/{cardId}`: Delete a card

#### Installments Controller
- `GET /api/installments`: Retrieve all installments
- `GET /api/installments/active`: Retrieve active installments
- `GET /api/installments/{installmentId}`: Get installment details
- `GET /api/installments/card/{cardId}`: Retrieve installments by card
- `POST /api/installments`: Add a new installment
- `PUT /api/installments/{installmentId}`: Update an installment
- `PUT /api/installments/{installmentId}/increment`: Increment an installment amount
- `DELETE /api/installments/{installmentId}`: Delete an installment

#### Financial Reports Controller
- `GET /api/financialreports`: Retrieve all reports
- `GET /api/financialreports/{reportId}`: Get report details
- `GET /api/financialreports/monthly/{yearMonth}`: Retrieve monthly report
- `GET /api/financialreports/annual/{year}`: Retrieve annual report
- `GET /api/financialreports/custom`: Retrieve custom reports for a date range
- `DELETE /api/financialreports/{reportId}`: Delete a report

#### Notifications Controller
- `GET /api/notifications`: Retrieve all notifications
- `GET /api/notifications/unread`: Retrieve unread notifications
- `GET /api/notifications/important`: Retrieve important notifications
- `GET /api/notifications/scheduled`: Retrieve scheduled notifications
- `GET /api/notifications/{notificationId}`: Get notification details
- `PUT /api/notifications/{notificationId}/read`: Mark a notification as read
- `PUT /api/notifications/markAllRead`: Mark all notifications as read
- `DELETE /api/notifications/{notificationId}`: Delete a notification
- `DELETE /api/notifications/clearAll`: Clear all notifications

### Special Features
- **Two-factor authentication** settings via UserSettings
- **Bill payment tracking**
- **Installments and card management**
- **Recurring income tracking**

### Technical Details
- **.NET 8.0**
- **Entity Framework Core 8.0**
- **SQL Server** database
- **JWT authentication**
- **CORS configuration**: Allows all origins (development only)

### Client Application Needs
1. **User registration and login**
2. **Expense/income/bill management**
3. **Card and installment tracking**
4. **User settings management**

