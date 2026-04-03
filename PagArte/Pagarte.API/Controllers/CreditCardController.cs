using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagarte.API.DTOs.Requests;
using Pagarte.API.Interfaces;

namespace Pagarte.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/creditcard")]
    public class CreditCardController(ICreditCardService creditCardService) : BaseController
    {
        private readonly ICreditCardService _creditCardService = creditCardService;

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _creditCardService.GetByClientIdAsync(GetClientId()!);
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterCreditCardRequest request)
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _creditCardService.RegisterAsync(GetClientId()!, request);
            return Ok(response);
        }

        [HttpPut("{cardId}")]
        public async Task<IActionResult> UpdateAsync(Guid cardId, [FromBody] UpdateCreditCardRequest request)
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _creditCardService.UpdateAsync(GetClientId()!, cardId, request);
            return Ok(response);
        }

        [HttpDelete("{cardId}")]
        public async Task<IActionResult> DeleteAsync(Guid cardId)
        {
            var validation = ValidateClientId();
            if (validation != null) return validation;

            var response = await _creditCardService.DeleteAsync(GetClientId()!, cardId);
            return Ok(response);
        }
    }
}
