using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagarte.API.DTOs.Requests;
using Pagarte.API.Interfaces;

namespace Pagarte.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/payment")]
    public class PaymentController(IPaymentService paymentService) : BaseController
    {
        private readonly IPaymentService _paymentService = paymentService;

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _paymentService.GetByClientIdAsync(GetClientId()!);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _paymentService.GetByIdAsync(id, GetClientId()!);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAsync([FromBody] ProcessPaymentRequest request)
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _paymentService.ProcessPaymentAsync(GetClientId()!, request);
            return Ok(response);
        }
    }
}
