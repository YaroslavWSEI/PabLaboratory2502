using Infrastructure.Context;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")] // Это создаст маршрут /api/quizzes
[Authorize] // Требует авторизацию (которую мы обходим в тестах через TestAuthHandler)
public class QuizzesController : ControllerBase
{
    private readonly QuizDbContext _context;

    public QuizzesController(QuizDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuizEntity>>> GetQuizzes()
    {
        // Возвращаем список всех квизов из базы данных
        var quizzes = await _context.Quizzes
            .Include(q => q.Items) // Подгружаем вопросы
            .ThenInclude(i => i.IncorrectAnswers) // Подгружаем варианты ответов
            .ToListAsync();

        return Ok(quizzes);
    }
}