using APIProject.Data;
using APIProject.Logging;
using APIProject.Models;
using APIProject.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace APIProject.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ILogging _logger;
        public PlayerController(ILogging logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult <IEnumerable<PlayerDto>> GetPlayers()
        {
            _logger.Log("Getting all Players", "");
            return Ok(PlayerStore.playerList);
        }

        [HttpGet("{id:int}",Name ="GetSpecificPlayer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult GetPlayers(int id)
        {
            if (id == 0)
            {
                return BadRequest(id);
            }

            var player = (PlayerStore.playerList.FirstOrDefault(u => u.Id == id));

            if (player == null)
            {
                return NotFound();
            }

            return Ok(player); 
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PlayerDto> CreatePlayer([FromBody]PlayerDto playerDto)
        {
            if (PlayerStore.playerList.FirstOrDefault(u => u.Name.ToLower() == playerDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Player Already Existis");
                return BadRequest(ModelState);
            }
            if (playerDto == null) 
            {
                return BadRequest();
            } 
            if (playerDto.Id > 0) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            playerDto.Id = PlayerStore.playerList.OrderByDescending(u => u.Id).FirstOrDefault().Id+1;
            PlayerStore.playerList.Add(playerDto);

            return CreatedAtRoute("", new { id = playerDto.Id}, playerDto);
        }

        [HttpDelete("{id:int}", Name ="DeletePlayer")]
        public IActionResult DeletePlayer(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var player = PlayerStore.playerList.FirstOrDefault(u =>u.Id == id);
            if (player == null)
            {
                return BadRequest();
            }
            PlayerStore.playerList.Remove(player);
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdatePlayer")]
        public IActionResult UpdatePlayer(int id,[FromBody] PlayerDto playerDto)
        {
            if (playerDto ==  null || id != playerDto.Id) 
            {
                return BadRequest();
            }
            var player = PlayerStore.playerList.FirstOrDefault(u => u.Id == id);
            player.Name = playerDto.Name;
            player.Country = playerDto.Country;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialPlayer")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public IActionResult UpdatePartialPlayer(int id, JsonPatchDocument<PlayerDto> patchDto) 
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var player = PlayerStore.playerList.FirstOrDefault(u => u.Id == id);
            if (player == null)
            {
                return BadRequest();
            }
            patchDto.ApplyTo(player);

            if (!TryValidateModel(player))
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }


    }
}
