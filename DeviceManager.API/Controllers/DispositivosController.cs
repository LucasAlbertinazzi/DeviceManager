using DeviceManager.API.Models;
using DeviceManager.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DispositivosController : ControllerBase
    {
        private readonly DispositivoService _service;

        public DispositivosController(DispositivoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var item = await _service.GetByIdAsync(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Dispositivo dispositivo)
        {
            // Validação de unicidade
            var todos = await _service.GetAllAsync();
            if (todos.Any(d => d.CodigoReferencia == dispositivo.CodigoReferencia))
                return Conflict("Código de referência já existe.");

            var criado = await _service.CreateAsync(dispositivo);
            return CreatedAtAction(nameof(Get), new { id = criado.Id }, criado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Dispositivo dispositivo)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            dispositivo.Id = id;
            await _service.UpdateAsync(id, dispositivo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existente = await _service.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }


        [HttpPost("sync")]
        public async Task<IActionResult> Sincronizar([FromBody] List<Dispositivo> dispositivos)
        {
            try
            {

                Console.WriteLine($"Recebido {dispositivos?.Count ?? 0} dispositivos");

                foreach (var item in dispositivos ?? new List<Dispositivo>())
                {
                    Console.WriteLine($" > {item.Descricao} | {item.CodigoReferencia} | {item.DataCriacao} | {item.DataAtualizacao}");
                }

                var logs = new List<string>();
                var existentes = await _service.GetAllAsync();

                foreach (var item in dispositivos)
                {
                    var existente = existentes.FirstOrDefault(e => e.CodigoReferencia == item.CodigoReferencia);

                    if (existente == null)
                    {
                        // Novo
                        item.Id = null; // MongoDB gera ID automaticamente
                        item.DataCriacao = DateTime.UtcNow;
                        await _service.CreateAsync(item);
                        logs.Add($"[INSERIDO] {item.Descricao}");
                    }
                    else if (item.IsDeleted)
                    {
                        // Deletado
                        await _service.DeleteAsync(existente.Id);
                        logs.Add($"[EXCLUÍDO] {item.Descricao}");
                    }
                    else if (item.DataAtualizacao.HasValue && (!existente.DataAtualizacao.HasValue || item.DataAtualizacao > existente.DataAtualizacao))
                    {
                        item.Id = existente.Id;
                        await _service.UpdateAsync(existente.Id, item);
                        logs.Add($"[ATUALIZADO] {item.Descricao}");
                    }
                    else if (item.DataAtualizacao.HasValue && existente.DataAtualizacao.HasValue && item.DataAtualizacao < existente.DataAtualizacao)
                    {
                        logs.Add($"[CONFLITO] {item.Descricao}: dados locais mais antigos que os do servidor");
                    }
                    else
                    {
                        logs.Add($"[IGNORADO] {item.Descricao} (sem mudanças)");
                    }

                }

                Console.WriteLine("=== LOG SINCRONIZAÇÃO ===");
                logs.ForEach(log => Console.WriteLine(log));

                return Ok(new
                {
                    Sucesso = true,
                    TotalRecebidos = dispositivos.Count,
                    Logs = logs
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERRO NO SYNC: {ex}");
                return StatusCode(500, $"Erro ao sincronizar: {ex.Message}");
            }
        }


        [HttpGet("por-referencia/{codigo}")]
        public async Task<ActionResult<Dispositivo>> GetByCodigoReferencia(string codigo)
        {
            var dispositivo = await _service.GetByCodigoReferenciaAsync(codigo);

            if (dispositivo == null)
                return NotFound();

            return Ok(dispositivo);
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("pong");

    }
}
