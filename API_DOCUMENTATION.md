# Finance Assistant API Documentation

## Overview
Finance Assistant API, kişisel finans yönetimi için geliştirilmiş kapsamlı bir RESTful API'dir. JWT tabanlı kimlik doğrulama kullanır ve PostgreSQL veritabanı ile çalışır.

## Base URL
```
http://localhost:5025/api
```

## Authentication
API, JWT (JSON Web Token) tabanlı kimlik doğrulama kullanır. Tüm istekler (auth endpoints hariç) Authorization header'ında Bearer token gerektirmektedir.

```javascript
headers: {
  'Authorization': 'Bearer your_jwt_token',
  'Content-Type': 'application/json'
}
```

## Core Features

### 1. Authentication
- Register: `/auth/register` (POST)
- Login: `/auth/login` (POST)

#### Register Request Body
```json
{
  "username": "string",
  "email": "string",
  "password": "string",
  "firstName": "string",
  "lastName": "string"
}
```

#### Login Request Body
```json
{
  "username": "string",
  "password": "string",
  "rememberMe": boolean
}
```

### 2. Regular Incomes
Düzenli gelirlerin yönetimi (maaş, kira geliri vb.)

- Get All: `/regularincomes` (GET)
- Get By Id: `/regularincomes/{id}` (GET)
- Create: `/regularincomes` (POST)
- Update: `/regularincomes/{id}` (PUT)
- Delete: `/regularincomes/{id}` (DELETE)

#### Regular Income Model
```json
{
  "name": "string",
  "amount": number,
  "dayOfMonth": number,
  "description": "string",
  "isActive": boolean
}
```

### 3. Invoices (Faturalar)
Fatura yönetimi (elektrik, su, doğalgaz vb.)

- Get All: `/invoices` (GET)
- Get Unpaid: `/invoices/unpaid` (GET)
- Get Recurring: `/invoices/recurring` (GET)
- Get By Type: `/invoices/byType/{type}` (GET)
- Create: `/invoices` (POST)
- Update: `/invoices/{id}` (PUT)
- Pay: `/invoices/{id}/pay` (PUT)
- Delete: `/invoices/{id}` (DELETE)

#### Invoice Model
```json
{
  "title": "string",
  "provider": "string",
  "amount": number,
  "dueDate": "date",
  "invoiceNumber": "string",
  "invoiceType": "string",
  "period": "string",
  "isRecurring": boolean,
  "recurringDay": number,
  "notes": "string"
}
```

### 4. Cards & Installments
Kredi kartları ve taksitli ödemelerin yönetimi

#### Cards
- Get All: `/cards` (GET)
- Get By Id: `/cards/{id}` (GET)
- Create: `/cards` (POST)
- Update: `/cards/{id}` (PUT)
- Delete: `/cards/{id}` (DELETE)

#### Card Model
```json
{
  "name": "string",
  "lastFourDigits": "string",
  "expiryDate": "date",
  "cardType": "string",
  "limit": number
}
```

#### Installments
- Get All: `/installments` (GET)
- Get Active: `/installments/active` (GET)
- Get By Card: `/installments/card/{cardId}` (GET)
- Create: `/installments` (POST)
- Update: `/installments/{id}` (PUT)
- Increment: `/installments/{id}/increment` (PUT)
- Delete: `/installments/{id}` (DELETE)

#### Installment Model
```json
{
  "cardId": number,
  "description": "string",
  "totalAmount": number,
  "totalInstallments": number,
  "currentInstallmentNumber": number,
  "startDate": "date",
  "isCompleted": boolean
}
```

### 5. Bills
Genel fatura ve ödemelerin yönetimi

- Get All: `/bills` (GET)
- Get Unpaid: `/bills/unpaid` (GET)
- Create: `/bills` (POST)
- Update: `/bills/{id}` (PUT)
- Pay: `/bills/{id}/pay` (PUT)
- Delete: `/bills/{id}` (DELETE)

#### Bill Model
```json
{
  "title": "string",
  "amount": number,
  "dueDate": "date",
  "description": "string",
  "billType": "string",
  "isPaid": boolean
}
```

### 6. Expenses
Genel harcamaların yönetimi

- Get All: `/expenses` (GET)
- Get By Id: `/expenses/{id}` (GET)
- Create: `/expenses` (POST)
- Update: `/expenses/{id}` (PUT)
- Delete: `/expenses/{id}` (DELETE)

#### Expense Model
```json
{
  "description": "string",
  "amount": number,
  "date": "date",
  "category": "string",
  "paymentMethod": "string"
}
```

### 7. Notifications
Bildirim yönetimi

- Get All: `/notifications` (GET)
- Get Unread: `/notifications/unread` (GET)
- Get Important: `/notifications/important` (GET)
- Get Scheduled: `/notifications/scheduled` (GET)
- Mark as Read: `/notifications/{id}/read` (PUT)
- Mark All as Read: `/notifications/markAllRead` (PUT)
- Delete: `/notifications/{id}` (DELETE)
- Clear All: `/notifications/clearAll` (DELETE)

#### Notification Model
```json
{
  "title": "string",
  "message": "string",
  "type": "string",
  "isRead": boolean,
  "isImportant": boolean,
  "scheduledFor": "date"
}
```

### 8. Financial Reports
Finansal raporlama ve analiz

- Get All: `/financialreports` (GET)
- Get Monthly: `/financialreports/monthly/{period}` (GET)
- Get Annual: `/financialreports/annual/{year}` (GET)
- Get Custom: `/financialreports/custom` (GET)
- Delete: `/financialreports/{id}` (DELETE)

#### Financial Report Response
```json
{
  "title": "string",
  "type": "string",
  "startDate": "date",
  "endDate": "date",
  "totalIncome": number,
  "totalExpense": number,
  "balance": number,
  "period": "string",
  "categorySummary": {
    "category": number
  },
  "monthlyTrend": {
    "period": number
  }
}
```

## Error Handling
API standart HTTP durum kodlarını kullanır:
- 200: Başarılı
- 201: Oluşturuldu
- 400: Hatalı İstek
- 401: Yetkisiz
- 403: Yasaklı
- 404: Bulunamadı
- 500: Sunucu Hatası

Hata yanıtları şu formatta döner:
```json
{
  "message": "string",
  "details": "string"
}
```

## Client Önerileri

### Teknoloji Stack'i
- React + TypeScript
- State Management: Redux Toolkit veya React Query
- UI Framework: Material-UI veya Ant Design
- Chart Library: Recharts veya Chart.js
- Form Management: Formik + Yup
- HTTP Client: Axios

### Önerilen Sayfa Yapısı
1. Auth Pages
   - Login
   - Register
   - Forgot Password

2. Dashboard
   - Genel Bakış
   - Gelir/Gider Özeti
   - Son İşlemler
   - Yaklaşan Ödemeler
   - Bildirimler

3. Regular Income Management
   - Liste Görünümü
   - Ekleme/Düzenleme Formu
   - Detay Görünümü

4. Invoice Management
   - Liste Görünümü
   - Ekleme/Düzenleme Formu
   - Fatura Detayları
   - Ödeme Sayfası

5. Cards & Installments
   - Kart Listesi
   - Kart Detayları
   - Taksit Listesi
   - Taksit Detayları

6. Bills & Expenses
   - Liste Görünümü
   - Ekleme/Düzenleme Formu
   - Kategori Bazlı Görünüm
   - Ödeme Sayfası

7. Reports & Analytics
   - Aylık Rapor
   - Yıllık Rapor
   - Özel Dönem Raporu
   - Grafikler ve Analizler

8. Settings
   - Profil Ayarları
   - Bildirim Ayarları
   - Kategori Yönetimi
   - Güvenlik Ayarları

### Önemli Özellikler
1. Responsive Design
2. Dark/Light Theme
3. Offline Support
4. PWA Support
5. Error Boundaries
6. Loading States
7. Toast Notifications
8. Form Validations
9. Data Caching
10. Route Protection

### Güvenlik Önlemleri
1. JWT Token Management
2. HTTP-only Cookies
3. XSS Protection
4. CSRF Protection
5. Input Sanitization
6. Error Handling
7. Rate Limiting
8. Session Management

## Development Guidelines
1. Component-Based Architecture
2. TypeScript Strict Mode
3. ESLint + Prettier
4. Unit Testing (Jest + React Testing Library)
5. Git Flow
6. CI/CD Pipeline
7. Documentation
8. Code Review Process

## Performance Optimization
1. Code Splitting
2. Lazy Loading
3. Memoization
4. Virtual Scrolling
5. Image Optimization
6. Bundle Size Optimization
7. Caching Strategies
8. Performance Monitoring 