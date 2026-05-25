using Infrastructure.Context;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
        var quizzes = await _context.Quizzes
            .Include(q => q.Items)
            .ThenInclude(i => i.IncorrectAnswers)
            .ToListAsync();

        return Ok(quizzes);
    }
}