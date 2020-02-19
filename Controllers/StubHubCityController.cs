using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using concertTicketWebCoreMVC.Models;
using concertTicketWebCoreMVC.Data;
namespace concertTicketWebCoreMVC.Controllers
{
    public class StubHubCityController : Controller
    {
        private readonly Data.stubhubApiContext _context;

        public StubHubCityController(Data.stubhubApiContext context)
        {
            _context = context;
        }
        
        // GET: StubHubCity
        public async Task<IActionResult> Index(string? id)
        {
            return View(await _context.StubHubCity.ToListAsync());
        }

        // GET: StubHubCity/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var StubHubCity = await _context.StubHubCity
                .FirstOrDefaultAsync(m => m.Index == id);
            if (StubHubCity == null)
            {
                return NotFound();
            }

            return View(StubHubCity);
        }

        // GET: StubHubCity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StubHubCity/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Index,GeoNameId,StubHubCity,StateCode,State,CountryCode,Country,Latitude,Longitude,Score,TimeZoneId,TimeZoneRawOffset,TimeZoneDisplayOffset")] StubHubCity StubHubCity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(StubHubCity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(StubHubCity);
        }

        // GET: StubHubCity/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var StubHubCity = await _context.StubHubCity.FindAsync(id);
            if (StubHubCity == null)
            {
                return NotFound();
            }
            return View(StubHubCity);
        }

        // POST: StubHubCity/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Index,GeoNameId,StubHubCity,StateCode,State,CountryCode,Country,Latitude,Longitude,Score,TimeZoneId,TimeZoneRawOffset,TimeZoneDisplayOffset")] StubHubCity StubHubCity)
        {
            if (id != StubHubCity.Index)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(StubHubCity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StubHubCityExists(StubHubCity.Index))
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
            return View(StubHubCity);
        }

        // GET: StubHubCity/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var StubHubCity = await _context.StubHubCity
                .FirstOrDefaultAsync(m => m.Index == id);
            if (StubHubCity == null)
            {
                return NotFound();
            }

            return View(StubHubCity);
        }

        // POST: StubHubCity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var StubHubCity = await _context.StubHubCity.FindAsync(id);
            _context.StubHubCity.Remove(StubHubCity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StubHubCityExists(long id)
        {
            return _context.StubHubCity.Any(e => e.Index == id);
        }
    }
}
