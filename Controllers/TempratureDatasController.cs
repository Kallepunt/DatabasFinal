using DatabasFinal.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabasFinal.Controllers
{
    public class TempratureDatasController : Controller
    {
        private readonly EFContext _context;

        public TempratureDatasController(EFContext context)
        {
            _context = context;
        }

        // GET: TempratureDatas




        public async Task<IActionResult> SearchByDate(DateTime firstDate, DateTime secondDate, string sortOrder)
        {




            var temperatures = _context.Tempratures.GroupBy(c => new { c.Date.Date, c.Location })
                                       .Select(g => new TempratureData
                                       {
                                           Date = g.Key.Date,
                                           Location = g.Key.Location,
                                           Temperature = Math.Round(g.Average(p => p.Temperature), 1),
                                           Humidity = Convert.ToInt32(g.Average(p => p.Humidity)),
                                           MouldRisk = Math.Round(g.Average(p => p.MouldRisk))

                                       }).Where(x => x.Date >= firstDate.Date).Where(x => x.Date <= secondDate.Date);


           


            return View(await temperatures.AsNoTracking().ToListAsync());



        } 

        public async Task<IActionResult> TempDiff(string sortOrder)
        {

            ViewData["TempSortParam"] = sortOrder == "temp" ? "tempDesc" : "temp";

            List<TempratureData> Ute = new List<TempratureData>();
            List<TempratureData> Inne = new List<TempratureData>();
            List<Tempdiff> tempdiffs = new List<Tempdiff>();

            var temperatures = _context.Tempratures.GroupBy(c => new { c.Date.Date, c.Location })
                                      .Select(g => new TempratureData
                                      {
                                          Date = g.Key.Date,
                                          Location = g.Key.Location,
                                          Temperature = Math.Round(g.Average(p => p.Temperature), 1),
                                          Humidity = Convert.ToInt32(g.Average(p => p.Humidity)),
                                          MouldRisk = Math.Round(g.Average(p => p.MouldRisk))

                                      });
            foreach (var item in temperatures)
            {
                if (item.Location == "Ute")
                {
                    Ute.Add(item);
                }
                else
                {
                    Inne.Add(item);
                }
            }


            foreach (var item1 in Ute)
            {
                foreach (var item2 in Inne)
                {
                    if (item1.Date == item2.Date && item1.Location != item2.Location)
                    {

                        Tempdiff tempdiff = new Tempdiff();

                        tempdiff.TempInside = item2.Temperature;
                        tempdiff.TempOutside = item1.Temperature;
                        tempdiff.Date = item1.Date;
                        tempdiff.tempdifferense = item2.Temperature - item1.Temperature;
                        tempdiffs.Add(tempdiff);
                    }
                }
            }

            var query = tempdiffs.OrderBy(p => p.Date);

            switch (sortOrder)
            {

                case "temp":
                    query = tempdiffs.OrderBy(t => t.tempdifferense);
                    break;

                case "tempDesc":
                    query = tempdiffs.OrderByDescending(t => t.tempdifferense);
                    break;

                default:
                    query = tempdiffs.OrderBy(t => t.tempdifferense);
                    break;
            }

            return View(query);


        }



        public async Task<IActionResult> Index(string sortOrder) // All Avg data per dag
        {


            ViewData["TempSortParam"] = sortOrder == "temp" ? "tempDesc" : "temp";
            ViewData["DateSortParam"] = sortOrder == "date" ? "dateDesc" : "date";
            ViewData["HumidSortParam"] = sortOrder == "humid" ? "humidDesc" : "humid";
            ViewData["MouldSortParam"] = sortOrder == "mould" ? "mouldDesc" : "mould";


            var temperatures = _context.Tempratures.GroupBy(c => new { c.Date.Date, c.Location })
                                       .Select(g => new TempratureData
                                       {
                                           Date = g.Key.Date,
                                           Location = g.Key.Location,
                                           Temperature = Math.Round(g.Average(p => p.Temperature), 1),
                                           Humidity = Convert.ToInt32(g.Average(p => p.Humidity)),
                                           MouldRisk = Math.Round(g.Average(p => p.MouldRisk))

                                       });


            switch (sortOrder)
            {

                case "temp":
                    temperatures = temperatures.OrderBy(t => t.Temperature);
                    break;

                case "tempDesc":
                    temperatures = temperatures.OrderByDescending(t => t.Temperature);
                    break;


                case "date":
                    temperatures = temperatures.OrderBy(t => t.Date.Date);
                    break;

                case "dateDesc":
                    temperatures = temperatures.OrderByDescending(t => t.Date.Date);
                    break;


                case "humid":
                    temperatures = temperatures.OrderBy(t => t.Humidity);
                    break;

                case "humidDesc":
                    temperatures = temperatures.OrderByDescending(t => t.Humidity);
                    break;

                case "mould":
                    temperatures = temperatures.OrderBy(t => t.MouldRisk);
                    break;

                case "mouldDesc":
                    temperatures = temperatures.OrderByDescending(t => t.MouldRisk);
                    break;


                default:
                    temperatures = temperatures.OrderBy(t => t.Date.Date);
                    break;
            }


            return View(await temperatures.AsNoTracking().ToListAsync());



        }

        public async Task<IActionResult> Metrologiskvinter()
        {

            return View(await _context.Tempratures.GroupBy(c => new { c.Date.Date, Location = c.Location })
                .Select(g => new TempratureData
                {
                    Date = g.Key.Date,
                    Location = g.Key.Location,
                    Temperature = Math.Round(g.Average(p => p.Temperature), 1),
                    Humidity = Convert.ToInt32(g.Average(p => p.Humidity)),
                    MouldRisk = g.Average(p => p.MouldRisk)

                }



                ).OrderBy(x => x.Date).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tempratureData = await _context.Tempratures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tempratureData == null)
            {
                return NotFound();
            }

            return View(tempratureData);
        }   


        public async Task<IActionResult> Charts()
        {
            var temperatures = _context.Tempratures.GroupBy(c => new { c.Date.Date, Location = c.Location })
                           .Select(g => new TempratureData
                           {
                               Date = g.Key.Date,
                               Location = g.Key.Location,
                               Temperature = Math.Round(g.Average(p => p.Temperature), 1),
                               Humidity = Convert.ToInt32(g.Average(p => p.Humidity)),
                               MouldRisk = Math.Round(g.Average(p => p.MouldRisk))

                           });




            return View(await temperatures.AsNoTracking().ToListAsync());
        }



        public async Task<IActionResult> Door(string sortOrder)
        {

            ViewData["doorSortParam"] = sortOrder == "door" ? "doorDesc" : "door";

            ViewData["dateSortParam"] = sortOrder == "date" ? "dateDesc" : "date";




            var insideList = _context.Tempratures.AsEnumerable().Where(x => x.Location == "Inne").GroupBy(x =>
            {
                var timeStamp = x.Date;
                timeStamp = timeStamp.AddMinutes(-(timeStamp.Minute % 5));
                timeStamp = timeStamp.AddSeconds(-timeStamp.Second);
                timeStamp = timeStamp.AddMilliseconds(-timeStamp.Millisecond);
                return timeStamp;
            })
               .Select(g => new { Date = g.Key, AvgTemp = Math.Round(g.Average(s => s.Temperature), 1) })
               .OrderBy(x => x.Date)
               .ToList();

            var outsideList = _context.Tempratures.AsEnumerable().Where(x => x.Location == "Ute").GroupBy(x =>
            {
                var timeStamp = x.Date;
                timeStamp = timeStamp.AddMinutes(-(timeStamp.Minute % 5));
                timeStamp = timeStamp.AddSeconds(-timeStamp.Second);
                timeStamp = timeStamp.AddMilliseconds(-timeStamp.Millisecond);
                return timeStamp;
            })
            .Select(g => new { Date = g.Key, AvgTemp = Math.Round(g.Average(s => s.Temperature), 1) })
            .OrderBy(x => x.Date)
            .ToList();



            List<DoorTime> doorTimes = new List<DoorTime>();
            DateTime previousDateInside = DateTime.Now;
            double previousTempInside = 0;

            DateTime previousDateOutside = DateTime.Now;
            double previousTempOutside = 0;


            foreach (var Innedata in insideList)
            {

                var findoutdata = outsideList.Find(x => x.Date == Innedata.Date);


                if (findoutdata != null)
                {
                    if (Innedata.Date == previousDateInside.AddMinutes(5))
                    {

                        if (Innedata.AvgTemp < previousTempInside && findoutdata.AvgTemp > previousTempOutside)
                        {

                            DoorTime doorTime = new DoorTime();
                            doorTime.AvgTempIn = Innedata.AvgTemp;
                            doorTime.AvgTempOut = findoutdata.AvgTemp;
                            doorTime.DateTime = Innedata.Date;
                            doorTime.timeSpan = Innedata.Date - previousDateInside;

                            doorTimes.Add(doorTime);
                        }

                    }

                    previousDateInside = Innedata.Date;
                    previousTempInside = Innedata.AvgTemp;


                    previousDateOutside = findoutdata.Date;
                    previousTempOutside = findoutdata.AvgTemp;


                }


            }

            var q1 = doorTimes.AsEnumerable().GroupBy(d => d.DateTime.Date).Select(x => new DoorTime
            {
                DateTime = x.Key.Date,
                AvgTempIn = Math.Round(x.Average(p => p.AvgTempIn), 1),
                AvgTempOut = Math.Round(x.Average(p => p.AvgTempOut), 1),
                timeSpan = new TimeSpan(x.Sum(p => p.timeSpan.Ticks))





            }).OrderBy(x => x.DateTime);


            switch (sortOrder)
            {

                case "door":
                    q1 = q1.OrderBy(t => t.timeSpan);
                    break;

                case "doorDesc":
                    q1 = q1.OrderByDescending(t => t.timeSpan);
                    break;


                case "date":
                    q1 = q1.OrderBy(t => t.timeSpan);
                    break;

                case "dateDesc":
                    q1 = q1.OrderByDescending(t => t.timeSpan);
                    break;

                default:
                    q1 = q1.OrderBy(t => t.DateTime);
                    break;



            }


                    return View(q1);

        }





        public IActionResult Create()
        {
            return View();
        }

        // POST: TempratureDatas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Temperature,Humidity,MouldRisk,Location")] TempratureData tempratureData)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tempratureData);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tempratureData);
        }




        [HttpGet]
        public IActionResult Upload(List<TempratureData> tempratureDatas = null)
        {
            tempratureDatas = tempratureDatas == null ? new List<TempratureData>() : tempratureDatas;

            return View(tempratureDatas);

        }



        [HttpPost]
        public IActionResult Upload(IFormFile file, [FromServices] IHostingEnvironment hostingEnvironment)
        {

            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(fileName))
            {

                file.CopyTo(fileStream);
                fileStream.Flush();
            }


            var temperatureData = this.GetTemperatureList(file.FileName);


            return Upload(temperatureData);


        }


        private List<TempratureData> GetTemperatureList(string fName)
        {
            List<TempratureData> tempratureDatas = new List<TempratureData>();

            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fName;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read))
            {

                using (var reader = new StreamReader(stream))
                {
                    List<string> listA = new List<string>();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        listA.Add(values[0]);

                    }


                    foreach (var item in listA)
                    {
                        string[] tempData = item.Split(',');



                        TempratureData data = new TempratureData();

                        data.Location = tempData[1];
                        data.Date = DateTime.Parse(tempData[0]);
                        data.Temperature = double.Parse(tempData[2], CultureInfo.InvariantCulture);
                        data.Humidity = int.Parse(tempData[3]);
                        data.MouldRisk = (data.Humidity - 78) * (data.Temperature / 15) / 0.22;


                        if (data.MouldRisk < 0)
                        {
                            data.MouldRisk = 0;
                        }

                        else if (data.MouldRisk > 100)
                        {
                            data.MouldRisk = 100;
                        }

                        tempratureDatas.Add(data);


                        _context.Add(data);
                        _context.SaveChanges();


                    }
                }
            }


            return (tempratureDatas);
        }




        // GET: TempratureDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tempratureData = await _context.Tempratures.FindAsync(id);
            if (tempratureData == null)
            {
                return NotFound();
            }
            return View(tempratureData);
        }

        // POST: TempratureDatas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Temperature,Humidity,MouldRisk,Location")] TempratureData tempratureData)
        {
            if (id != tempratureData.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tempratureData);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TempratureDataExists(tempratureData.Id))
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
            return View(tempratureData);
        }

        // GET: TempratureDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tempratureData = await _context.Tempratures
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tempratureData == null)
            {
                return NotFound();
            }

            return View(tempratureData);
        }

        // POST: TempratureDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tempratureData = await _context.Tempratures.FindAsync(id);
            _context.Tempratures.Remove(tempratureData);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TempratureDataExists(int id)
        {
            return _context.Tempratures.Any(e => e.Id == id);
        }
    }
}
