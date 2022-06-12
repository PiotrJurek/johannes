﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JohannesWebApplication.Data;
using JohannesWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;

namespace JohannesWebApplication.Controllers
{
    public class OrderModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderModelsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrderModels
        public async Task<IActionResult> Index()
        {
              return _context.Orders != null ? 
                          View(await _context.Orders.Include("Commisioner").ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
        }

        // GET: OrderModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orderModel = await _context.Orders
                .Include("Commisioner")
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            if (await CanTakeCommision(id))
                ViewData["CanTakePrinter"] = "True";
            
            return View(orderModel);
        }

        // GET: OrderModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name, Infill, PrintFile, SizeX, SizeY, SizeZ")] OrderModelVM orderModelVM)
        {
            var filePath = "/Files/Order" + Path.GetRandomFileName() + Path.GetExtension(orderModelVM.PrintFile.FileName);
            var ordername = orderModelVM.Name;
            var infill = orderModelVM.Infill;
            var sizex = orderModelVM.SizeX;
            var sizey = orderModelVM.SizeY;
            var sizez = orderModelVM.SizeZ;
            
            var applicationUser = await _userManager.GetUserAsync(HttpContext.User);

            using (var stream = System.IO.File.Create(filePath))
            {
                await orderModelVM.PrintFile.CopyToAsync(stream);
            }
            
            OrderModel orderModel = new OrderModel();
            orderModel.PrintFilePath = filePath;
            orderModel.Name = ordername;
            orderModel.Infill = infill;
            orderModel.SizeX = sizex;
            orderModel.SizeY = sizey;
            orderModel.SizeZ = sizez;
            orderModel.Commisioner = applicationUser;

            if (ModelState.IsValid)
            {
                _context.Add(orderModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderModelVM);
        }

        // GET: OrderModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orderModel = await _context.Orders.FindAsync(id);
            if (orderModel == null)
            {
                return NotFound();
            }
            return View(orderModel);
        }

        // POST: OrderModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PrinterID,Infill,FilePath")] OrderModel orderModel)
        {
            if (id != orderModel.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderModelExists(orderModel.OrderId))
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
            return View(orderModel);
        }

        // GET: OrderModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var orderModel = await _context.Orders
                .Include("Commisioner")
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (orderModel == null)
            {
                return NotFound();
            }

            return View(orderModel);
        }

        // POST: OrderModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }
            var orderModel = await _context.Orders.FindAsync(id);
            if (orderModel != null)
            {
                _context.Orders.Remove(orderModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderModelExists(int id)
        {
          return (_context.Orders?.Any(e => e.OrderId == id)).GetValueOrDefault();
        }
        
        public async Task<IActionResult> DownloadFile(int id)
        {
            var orderModel = await _context.Orders.FindAsync(id);
            var path = orderModel.PrintFilePath;
            System.IO.FileStream fs = System.IO.File.OpenRead(path);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(path);
            return File(
                data, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));;
        }

        public async Task<bool> CanTakeCommision(int? id)
        {
            var applicationUser = await _userManager
                .GetUserAsync(HttpContext.User);
            var applicationUserDatabase = await _context.ApplicationUsers
                .Include("PrinterModel")
                .FirstOrDefaultAsync(m => m.Id == applicationUser.Id);
            var orderModel = await _context.Orders
                .Include("Commisioner")
                .Include("PotentialExecutioners")
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if ((orderModel.Commisioner == applicationUser)||
                (orderModel.CommisionExecutioner == applicationUser)||
                (orderModel.PotentialExecutioners.Contains(applicationUser)))
                return false;
            foreach (PrinterModel printer in applicationUser.PrinterModel)
            {
                if ((printer.SizeX >= orderModel.SizeX) &&
                    (printer.SizeY >= orderModel.SizeY) &&
                    (printer.SizeZ >= orderModel.SizeZ))
                    return true;
            }
            return false;
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeOrder(int id)
        {
            var applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            var orderModel = await _context.Orders
                .Include("Commisioner")
                .FirstOrDefaultAsync(m => m.OrderId == id);
            
            applicationUser.PotentialCommisions.Add(orderModel);
            orderModel.PotentialExecutioners.Add(applicationUser);
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
