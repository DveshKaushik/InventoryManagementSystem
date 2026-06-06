"The backend is built on ASP.NET MVC with C#, following the MVC architectural pattern. I used raw SQL with parameterized queries against SQL Server to prevent SQL injection. Services like DatabaseHelper and GroqService are registered using Dependency Injection in Program.cs. The AI features use Groq's API to run the LLaMA 3.3 model for stock analysis and natural language queries."




Browser (Bootstrap + Chart.js)
        ↓ HTTP Request
ASP.NET MVC Controller (C#)
        ↓ calls
DatabaseHelper
        ↓ SQL Query
SQL Server (SSMS)
        ↓ returns data
C# Model objects
        ↓ passed to
Razor View (.cshtml)
        ↓ renders HTML
Browser displays page

AI Features:
Controller → GroqService → 
Groq API → LLaMA 3.3 model → 
AI response → View