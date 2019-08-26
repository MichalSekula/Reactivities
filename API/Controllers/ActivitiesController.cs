using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Activities;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    //nasz kontroler dziedziczy po ControllerBase, poniewaz nie przekazujemy widokow
    // a pobieramy dane z api
    public class ActivitiesController : ControllerBase {

        public IMediator _mediator { get; }
        public ActivitiesController (IMediator mediator) 
        {
            _mediator = mediator;
        }

        //Aby zoptymalizowac nasz aplikacje i ja uporzadkowac
        // stosujemy MediatR pattern, ktory za pomoca dependency injection 
        // pomaga nam odseparowac kod logiki biznesowej, od kontrolera oraz 
        // bezposrednio korzystania w kontrolerze danych z bazy danych
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> List()
        {
            return await _mediator.Send(new List.Query());
        }

        //cancellation token wykorzystywany jest gdy zadanie zostanie przerwane, 
        //wowczas trzeba w inicjalizacji funkcji w konttrolerze przekazac parametr cancelation
        //aby zatrzymac wykonywanie tasku, w innym przypadku cale rzadanie zostanie wykonane mimo przerwania requestu
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> Details(Guid id)
        {
            return await _mediator.Send(new Details.Query{Id = id});
        }

        //W MediatR pattern Command sluza do wysylania zapytania, z ktorych nic nie jest przewaznie zwracane
        // czyli tworzenie, edycja, usuwanie, 
        // do pobierania tresci oraz danych uzywamy Query i wowczas przekazujemy obiekt jaki tez zostanie zwrocony
        [HttpPost]
        public async Task<ActionResult<Unit>> Create([FromBody]Create.Command command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await _mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Edit(Guid id)
        {
            return await _mediator.Send(new Delete.Command{Id = id});
        }
    }
}