using CalapanCarRentalMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace CalapanCarRentalMVC.Controllers
{
 [Route("api/[controller]")]
 [ApiController]
 public class TraccarController : ControllerBase
 {
 private readonly ITraccarService _traccar;

 public TraccarController(ITraccarService traccar)
 {
 _traccar = traccar;
 }

 // GET: api/traccar/positions
 [HttpGet("positions")]
 public async Task<IActionResult> GetPositions(CancellationToken ct)
 {
 try
 {
 var positions = await _traccar.GetLatestPositionsAsync(ct);
 return Ok(new { success = true, positions });
 }
 catch (HttpRequestException httpEx)
 {
 return StatusCode(502, new { success = false, message = httpEx.Message });
 }
 catch (Exception ex)
 {
 return StatusCode(500, new { success = false, message = ex.Message });
 }
 }
 }
}
