# Contributing to Gravy 🍲  

Welcome, and thank you for considering contributing to **Gravy**! We appreciate your time, effort, and ideas to help improve the project. 🚀  

This document outlines the guidelines for contributing, including **setting up the development environment**, **submitting issues and pull requests**, and **best practices**.

---

## 🛠️ Getting Started  

### 1. Fork the Repository  
Start by forking the repository to your GitHub account:  
- Visit the repository: [Gravy on GitHub](https://github.com/MrEshboboyev/Gravy)  
- Click on the **Fork** button.  

### 2. Clone Your Fork  
Clone the forked repository to your local machine:  

```bash  
git clone https://github.com/YourUsername/Gravy.git  
cd Gravy  
```  

### 3. Set Up the Development Environment  
Ensure you have the following installed:  
- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
- PostgreSQL for the database  
- Redis for caching (optional but recommended)  

Then restore the project dependencies:  

```bash  
dotnet restore  
```  

Set up the database:  
```bash  
dotnet ef database update  
```  

### 4. Create a Branch  
Create a branch for your changes:  

```bash  
git checkout -b feature/your-feature-name  
```  

---

## 📋 Submitting Changes  

### 1. Make Your Changes  
- Follow the coding standards outlined below.  
- Keep your changes **small, focused**, and **self-contained**.  
- Test your changes thoroughly (see [Testing](#testing)).  

### 2. Commit Your Changes  
Write clean, concise, and descriptive commit messages. Follow this format:  

```bash  
git commit -m "Short description of the change"  
```  

Example:  
```bash  
git commit -m "Add validation for order creation in OrderService"  
```  

### 3. Push Your Branch  
Push your branch to GitHub:  

```bash  
git push origin feature/your-feature-name  
```  

### 4. Open a Pull Request  
- Go to your forked repository on GitHub.  
- Click the **Compare & Pull Request** button.  
- Provide a clear and detailed description of your changes.  
- Reference any related issues (e.g., `Closes #123`).  

### 5. Code Review  
Your Pull Request will be reviewed by project maintainers. Be prepared to:  
- Address any feedback.  
- Update your PR if required.  

Once approved, your PR will be merged into the main branch. 🎉  

---

## ✅ Coding Standards  

To maintain code consistency, please follow these guidelines:  

1. **Follow Clean Code Principles**:  
   - Use meaningful variable and method names.  
   - Keep methods small and focused (Single Responsibility Principle).  

2. **Formatting**:  
   - Use consistent indentation (4 spaces).  
   - Use line breaks between logical sections.  

3. **C# Code Style**:  
   - Use **PascalCase** for class, method, and property names.  
   - Use **camelCase** for local variables and parameters.  
   - Follow the official [.NET Naming Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/naming-guidelines).  

4. **Testing**:  
   - Write unit tests for new or modified code using **xUnit**.  
   - Place test files in the `Tests` folder.  

Example of a clean commit:  
```csharp  
public class OrderService  
{  
    public async Task<Order> CreateOrderAsync(Order newOrder)  
    {  
        if (newOrder == null)  
            throw new ArgumentNullException(nameof(newOrder));  

        // Process order creation logic...  
    }  
}  
```  

---

## 🧪 Testing  

Ensure your code includes appropriate unit tests:  

1. **Add tests** for all new features or changes.  
2. Run tests before submitting a PR:  

```bash  
dotnet test  
```  

---

## 🐛 Reporting Issues  

To report bugs or request features:  

1. Check the **existing issues** to avoid duplicates.  
2. Create a **new issue** and provide:  
   - A clear, descriptive title.  
   - Steps to reproduce (if it’s a bug).  
   - Expected vs. actual behavior.  
   - Screenshots or logs (if applicable).  

---

## 🚀 Contributing New Features  

When contributing a new feature:  
1. Ensure it aligns with the project goals.  
2. Discuss your idea by opening an **issue** or **draft PR**.  
3. Follow the same process for creating branches and submitting PRs.  

---

## 🤝 Code of Conduct  

Please read and adhere to the [Code of Conduct](CODE_OF_CONDUCT.md). Be respectful, inclusive, and collaborative.  

---

## 🙌 Acknowledgments  

Thank you for helping to make **Gravy** even better! 🎉  

If you have any questions, reach out via:  
- **GitHub Issues**  
- Email: [mreshboboyev@gmail.com](mailto:mreshboboyev@gmail.com)  

---

Now go make something awesome! 🚀  
