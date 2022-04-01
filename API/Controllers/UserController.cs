using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Cors;
using IronPython;
using IronPython.Hosting;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UsedContext _context;

        public UserController(UsedContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Users>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [EnableCors]
        [Route("/api/[controller]/Auth")]
        [HttpPost]
        public async Task<ActionResult<Boolean>> AuthenticateUsers(Users user)
        {
            var probUser = await _context.Users.FirstOrDefaultAsync(x => x.Username == user.Username);

            if (probUser is null || !BC.Verify(user.Password, probUser.Password))
            {
                return NotFound();
            }
            else
            {
                return Ok();
            }
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Users>> GetUsers(long id)
        {
            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return users;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers(long id, Users users)
        {
            if (id != users.Id)
            {
                return BadRequest();
            }

            _context.Entry(users).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Users>> PostUsers(Users users)
        {
            //Encrypts Password
            users.Password = BC.HashPassword(users.Password);

            _context.Users.Add(users);
            await _context.SaveChangesAsync();

            // return CreatedAtAction("GetUsers", new { id = users.Id }, users);
            return CreatedAtAction(nameof(GetUsers), new {id = users.Id}, users);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers(long id)
        {
            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsersExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [Route("/api/[controller]/Clustering")]
        [HttpGet]
        public IActionResult ClusterMethod(int numClusters)
        {   
            Console.WriteLine(RunPythonScript(numClusters));
            return Ok(RunPythonScript(numClusters));
        }

        [Route("/api/[controller]/FirstModelSubmit")]
        [HttpPost]
        public IActionResult FirstModelSubmit(FirstModel fs)
        {
            const double x1 = 15.813793035251104;
            const double x2 = 0.78151842;
            const double x3 = 0.06322984;
            const double x4 = -0.46158517;
            double result = 0;

            try
            {
                result = (x1 + (fs.MotherEducation * x2) + (fs.FatherEducation * x3) + (fs.StudentAge * x4));
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }

        [Route("/api/[controller]/SecondModelSubmit")]
        [HttpPost]
        public IActionResult SecondModelSubmit(SecondModel sm)
        {
            const double x1 = 15.813793035251104;
            const double x2 = 0.78151842;
            const double x3 = 0.00740238;
            const double x4 = 0.02474765;
            double result = 0;

            try
            {
                result = (x1 + (sm.MotherEducation * x2) + (sm.StudentAbsences * x3) + (sm.StudentFreeTime * x4));
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Forbid();
            }

        }

        [Route("/api/[controller]/ThirdModelSubmit")]
        [HttpPost]
        public IActionResult ThirdModelSubmit(ThirdModel ts)
        {
            const double x1 = 15.813793035251104;
            const double x2 = 0.21578577;
            const double x3 = 0.00740238;
            const double x4 = -2.20151606;
            double result = 0;
            
            try
            {
                result = (x1 + (ts.StudentStudyTime * x2) + (ts.StudentAbsences * x3) + (ts.RejectedGrades * x4));
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Forbid();
            }
        }



        private string RunPythonScript(int numClosters)
        {
            string scriptPath = @"C:\Dev\Learning_Analytics.API\API\Scripts\CopyClusterKmeans.py";
            string jsonReceived = "";
            // string pythonPath = @"C:\Users\Javi\AppData\Local\Programs\Python\Python38-32\python.exe";

            Process p = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = $" /c py {scriptPath} {numClosters}";
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;    
            // startInfo.Verb = "runas";
            p.StartInfo = startInfo;
            p.OutputDataReceived += (s,e) => jsonReceived += e.Data;
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

            return jsonReceived; 

        }



    }
}
