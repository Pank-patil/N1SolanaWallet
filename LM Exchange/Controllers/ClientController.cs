using LM_Exchange.Model;
using LM_Exchange.Services;
using LM_Exchange.Custom_Exception;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using LM_Exchange.Dtos;
using LM_Exchange.Models.Responses;
using Microsoft.AspNetCore.Authorization;

namespace LM_Exchange.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IMapper _mapper;
        public ClientController(IClientService clientService)
        {
            _clientService = clientService;

        }


        [Authorize]
        [HttpPost("/AddClient")]
        public async Task<ActionResult> CreateClient(ClientCreateDto dto)
        {
            try
            {

                var client = new Client
                {
                    OwnerName = dto.OwnerName,
                    Email = dto.Email,
                    aadharNumber = dto.aadharNumber,
                    MobileNumber = dto.MobileNumber,
                    Username = dto.UserName
                };

                var result = await _clientService.CreateClientAsync(client);

                var response = new ClientResponseDto
                {
                    Id = result.Id,
                    OwnerName = result.OwnerName,
                    Email = result.Email,
                    MobileNumber = result.MobileNumber
                };

                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        id = result.Id,
                        message = "client successfully added"
                    }
                };

                return Ok(res);
            }
            catch (InvalidName ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidEmail ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }

            catch (InvalidAadar ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidMobile ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to add client",
                        code = "500"
                    }
                });
            }
        }


        [Authorize]
        [HttpGet("/GetClients")]

        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            try
            {
                List<Client> result = (await _clientService.GetClientsAsync()).ToList();

                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        clients = result,
                        message = "Client list fetched successfully"
                    }
                };

                return Ok(res);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to get client list",
                        code = "500"
                    }
                });
            }
        }

        [Authorize]
        [HttpPost("/GetClientById")]
        public async Task<ActionResult<Client>> GetClientById(ClientByIdDto ClientByIdDto)
        {
            try
            {
                var client = await _clientService.GetClientByIdAsync(ClientByIdDto.Id);


                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        client.Id,
                        client.Email,
                        client.OwnerName,
                        client.MobileNumber,
                        AadharNumber = "XXXXXXXX" + client.aadharNumber[^4..]
                    }
                };
                return Ok(res);
            }
            catch (NotFoundException ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "Client id not found",
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to get client",
                        code = "500"
                    }
                });
            }
        }



        [Authorize]
        [HttpPut("/UpdateClient")]
        public async Task<IActionResult> UpdateClient(ClientUpdateDto clientUpdateDto)
        {
            try
            {
                await _clientService.UpdateClientAsync(clientUpdateDto);


                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        clients = clientUpdateDto.Id,
                        message = "Client  Updated  successfully"
                    }
                };
                return Ok(res);
            }
            catch (NotFoundException ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "Client id not found",
                        code = "400"
                    }
                });
            }
            catch (InvalidName ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidEmail ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidAadar ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InvalidMobile ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to Update client",
                        code = "500"
                    }
                });
            }
        }


        [Authorize]
        [HttpDelete("/DeleteClient")]
        public async Task<IActionResult> DeleteClient(ClientByIdDto clientByIdDto)
        {
            try
            {
                await _clientService.DeleteClientAsync(clientByIdDto.Id);


                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        clients = clientByIdDto.Id,
                        message = "Client deleted successfully"
                    }
                };
                return Ok(res);
            }
            catch (NotFoundException ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "Client id not found",
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to delete client",
                        code = "500"
                    }
                });
            }
        }


        [Authorize]
        [HttpPost("/AddMoney")]
        public async Task<IActionResult> AddMoney([FromBody] AddMoneyRequest request)
        {
           
            try
            {
                await _clientService.AddMoneyAsync(request.ClientId, request.Amount);

                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        request.ClientId,
                        message = "Money added successfully."
                    }
                };
                return Ok(res);
         
            }
            catch (InvalidAmountExpection ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to Add amount",
                        code = "500"
                    }
                });
            }
        }


        [Authorize]
        [HttpPost("/WithdrawMoney")]
        public async Task<IActionResult> WithdrawMone([FromBody] AddMoneyRequest request)
        {
            try
            {
                await _clientService.WithdrawMoneyAsync(request.ClientId, request.Amount);
                var res = new ApiResponse<object>
                {
                    data = new
                    {
                        request.ClientId,
                        message = "Amount Withdraw successfully"
                    }
                };
                return Ok(res);
                
            }
            catch (InvalidAmountExpection ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InsufficientbalanceException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to Withdraw amount",
                        code = "500"
                    }
                });
            }
        }




        [Authorize]
        [HttpPost("/SendToUser")]
        public async Task<IActionResult> TransferByUsername([FromBody] TransferByUsernameRequest request)
        {
            try
            {
                await _clientService.SendMoneyByUsernameAsync(request.SenderUsername,
                    request.ReceiverUsername, request.Amount);

                var res = new ApiResponse<object>
                {
                    data = new
                    {

                        message = request.SenderUsername + "Send money  To " + request.ReceiverUsername + "amount" + request.Amount
                    }
                };
                return Ok(res);
            }
            catch (InvalidAmountExpection ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (SelfTransationException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (NotFoundException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (InsufficientbalanceException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = ex.Message,
                        code = "400"
                    }
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new ApiErrorResponse
                {
                    error = new ApiError
                    {
                        message = "failed to delete client",
                        code = "500"
                    }
                });
            }

        }
    }
}

