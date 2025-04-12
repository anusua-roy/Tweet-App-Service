# 🐦 Tweet App – Backend (C#)

This is the **backend API** for the Tweet App — a full-stack Twitter-like microblogging platform built with **ASP.NET Web API** and **MongoDB**.

---

## ✨ Features
- JWT-based user authentication
- Create, read, and delete tweets
- Like and reply to tweets
- Follow/unfollow users
- MongoDB as the database

---

## 🔧 Tech Stack
- C# (.NET Core)
- ASP.NET Web API
- MongoDB
- Swagger for API docs
- NUnit for testing

---

## 📦 Project Setup
```bash
# Restore packages
dotnet restore

# Run the app
dotnet run
```

Visit: `http://localhost:5000/swagger` for API documentation

---

## 📬 API Endpoints (Sample)
| Method | Route                    | Description             |
|--------|--------------------------|-------------------------|
| POST   | /api/auth/register       | Register user           |
| POST   | /api/auth/login          | Login user              |
| GET    | /api/tweets              | Get all tweets          |
| POST   | /api/tweets              | Create a tweet          |
| POST   | /api/tweets/{id}/like    | Like a tweet            |
| POST   | /api/tweets/{id}/reply   | Reply to a tweet        |

---

## 📄 License
Open source under the [MIT License](LICENSE).
