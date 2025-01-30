# Office 365 Mailing Service

A .NET Core service for sending bulk emails through Office 365 with rate limiting and tracking capabilities. Built using Clean Architecture principles.

## Features

- Office 365 SMTP integration
- Daily email rate limiting (configurable, default 3600/day)
- Email tracking to prevent duplicate sends
- CSV-based recipient management
- HTML email template support with personalization
- Swagger API documentation
- Clean Architecture implementation
- Entity Framework Core for data persistence

## Prerequisites

- .NET 7.0 SDK
- SQL Server (or LocalDB)
- Office 365 account with SMTP access
- Visual Studio 2022 or VS Code

## Project Structure

```
MailingService/
├── src/
│   ├── MailingService.Domain/        # Enterprise business rules
│   ├── MailingService.Application/   # Application business rules
│   ├── MailingService.Infrastructure/# Data and external concerns
│   └── MailingService.Api/           # API endpoints and configuration
```

## Configuration

1. Update `appsettings.json` with your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-DB-Connection-String"
  },
  "MailSettings": {
    "Office365Username": "your-email@yourdomain.com",
    "Office365Password": "your-password",
    "FromEmail": "your-email@yourdomain.com",
    "FromName": "Your Name",
    "HtmlTemplatePath": "Templates/email-template.html",
    "CsvFilePath": "Data/recipients.csv",
    "DailyEmailLimit": 3600
  }
}
```

2. Create required directories and files:
   - `Templates/email-template.html` - Your email template
   - `Data/recipients.csv` - CSV file with recipients (format: Email,Name)

## Getting Started

1. Clone the repository:

```bash
git clone https://github.com/yourusername/MailingService.git
```

2. Navigate to the project directory:

```bash
cd MailingService
```

3. Restore dependencies:

```bash
dotnet restore
```

4. Update the database:

```bash
dotnet ef database update
```

5. Run the application:

```bash
dotnet run --project src/MailingService.Api
```

## API Endpoints

### Send Batch Emails

```http
POST /api/email/send-batch
```

Reads recipients from CSV file and sends personalized emails.

### Check Daily Count

```http
GET /api/email/daily-count
```

Returns current daily email statistics.

## CSV File Format

The recipients CSV file should follow this format:

```csv
Email,Name
recipient1@example.com,John Doe
recipient2@example.com,Jane Smith
```

## HTML Template

The HTML template supports personalization using placeholders:

```html
<html>
  <body>
    <h1>Hello {{Name}}</h1>
    <p>Your email content here.</p>
  </body>
</html>
```

## Development

- Follow Clean Architecture principles
- Use Entity Framework Core migrations for database changes
- Keep services stateless
- Handle exceptions appropriately
- Use async/await for I/O operations

## Security Considerations

- Store sensitive data (passwords, connection strings) in user secrets during development
- Use appropriate authentication/authorization in production
- Implement rate limiting per client if needed
- Monitor for abuse

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a new Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
# office365-mailing
