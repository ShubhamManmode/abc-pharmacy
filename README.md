# ABC Pharmacy

A simple Single Page Application for managing medicines and medicine sales.

## Tech Stack

- ASP.NET Core Web API (.NET 8)
- React + Vite
- JSON file storage
- REST APIs
- No database required

## Features

- View medicines
- Add medicines
- Search medicines by name
- Expiry warning: red when expiry is less than 30 days
- Low stock warning: yellow when quantity is less than 10
- Sell medicines
- Automatically reduce inventory after a sale
- View sales history
- JSON persistence on the server

## Project Structure

```text
abc-pharmacy/
├── backend/
│   └── ABCPharmacy.Api/
│       ├── Controllers/
│       ├── Data/
│       ├── Models/
│       ├── Services/
│       ├── Program.cs
│       └── ABCPharmacy.Api.csproj
├── frontend/
│   └── abc-pharmacy-ui/
│       ├── src/
│       ├── package.json
│       └── vite.config.js
└── README.md
```

## Run Backend

```bash
cd backend/ABCPharmacy.Api
dotnet restore
dotnet run
```

The API will normally start on the URL shown by the terminal.

Swagger is available in Development mode.

## Run Frontend

In another terminal:

```bash
cd frontend/abc-pharmacy-ui
npm install
npm run dev
```

Open the URL shown by Vite.

If your API uses a different URL/port, update:

```text
frontend/abc-pharmacy-ui/src/api.js
```

## GitHub

```bash
git init
git add .
git commit -m "Initial ABC Pharmacy application"
git branch -M main
git remote add origin YOUR_GITHUB_REPOSITORY_URL
git push -u origin main
```

## Important

This project intentionally uses JSON files because the assignment explicitly requires server-side JSON storage. For a production application, inventory and sales should use a transactional database.
