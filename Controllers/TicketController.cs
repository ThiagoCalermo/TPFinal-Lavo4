﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrabajoPracticoFinal_Labo4.Models;
using X.PagedList.Extensions;

namespace TrabajoPracticoFinal_Labo4.Controllers
{
	public class TicketController : Controller
	{
		private readonly AppDbContext _context;

		public TicketController(AppDbContext context)
		{
			_context = context;
		}

		// GET: Ticket
		public async Task<IActionResult> Index(string buscar,int? page)
		{
            int pageNumber = page ?? 1;
            int pageSize = 5;

            // Incluyendo las entidades relacionadas
            var ticket = _context.Tickets
                .Include(c => c.Afiliado)
                .AsQueryable();

            var ticketList = await ticket.OrderByDescending(s => s.Id).ToListAsync(); 
            var ticketsPaginadas = ticketList.ToPagedList(pageNumber, pageSize);

			return View(ticketsPaginadas);
		}

		// GET: Ticket/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ticket = await _context.Tickets
				.Include(t => t.Afiliado)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (ticket == null)
			{
				return NotFound();
			}

			return View(ticket);
		}

		// GET: Ticket/Create
		public async Task<IActionResult> Create()
		{
			ViewData["AfiliadoId"] = new SelectList(await _context.Afiliados.ToListAsync(), "Id", "Nombre");
			return View();
		}

		// POST: Ticket/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,AfiliadoId,FechaSolicitud,Observacion")] Ticket ticket)
		{
			if (ModelState.IsValid)
			{
				_context.Add(ticket);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id", ticket.AfiliadoId);
			return View(ticket);
		}

		// GET: Ticket/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

            ViewData["AfiliadoId"] = new SelectList(await _context.Afiliados.ToListAsync(), "Id", "Nombre");

            var ticket = await _context.Tickets.FindAsync(id);
			if (ticket == null)
			{
				return NotFound();
			}
			ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id", ticket.AfiliadoId);
			return View(ticket);
		}

		// POST: Ticket/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,AfiliadoId,FechaSolicitud,Observacion")] Ticket ticket)
		{
			if (id != ticket.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(ticket);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TicketExists(ticket.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			ViewData["AfiliadoId"] = new SelectList(_context.Afiliados, "Id", "Id", ticket.AfiliadoId);
			return View(ticket);
		}

		// GET: Ticket/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ticket = await _context.Tickets
				.Include(t => t.Afiliado)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (ticket == null)
			{
				return NotFound();
			}

			return View(ticket);
		}

		// POST: Ticket/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var ticket = await _context.Tickets.FindAsync(id);
			if (ticket != null)
			{
				_context.Tickets.Remove(ticket);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool TicketExists(int id)
		{
			return _context.Tickets.Any(e => e.Id == id);
		}
	}
}
