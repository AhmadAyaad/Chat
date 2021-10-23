using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ChatTrials
{
    [Authorize]
    public class ChartHub : Hub
    {
        private readonly ChatDbContext _context;
        public static Dictionary<string, string> data = new Dictionary<string, string>();
        private Provider _provider;
        private readonly IHubContext<ChartHub> _hubContext;

        public ChartHub(ChatDbContext chatDbContext, Provider provider, IHubContext<ChartHub> hubContext)
        {
            Trace.WriteLine(this);
            _context = chatDbContext;
            _provider = provider;
            _hubContext = hubContext;
        }

        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var loggedInUser = Context.User.GetUsername();

                //var users = await _provider.GetRoles("NormalUser");
                //var user = users.FirstOrDefault();
                data.Add(loggedInUser, Context.ConnectionId);

                #region MyRegion

                //var role = Context?.User?.Identity?.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;
                //var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();

                //if (!_Connections.Any(u => u.Username == IdentityName))
                //{
                //    _Connections.Add(userViewModel);
                //    _ConnectionsMap.Add(IdentityName, Context.ConnectionId);ferferer';we'w;e'lrf;w'krlf;'ekgvfd,vb.cb, C?w724e275e10``137899
                //}
                //await Clients.User(user.Id).SendAsync("getProfileInfo", user);
                //await Clients.Caller.SendAsync("getProfileInfo", pharmacyName);

                #endregion MyRegion
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
            }
        }

        public async Task sendData()
        {
            //var providerName = Context.GetHttpContext().Request.Query["name"];
            var res = await _provider.GetUserByName("3alaAllah");
            //var orderId = Context.GetHttpContext().Request.Query["orderId"];
            data.TryGetValue(res.UserName, out string connId);
            await Clients.Client(connId).SendAsync("newData", 4);
        }
    }

    public class Provider
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Provider(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<IList<ApplicationUser>> GetRoles(string role)
        {
            return _userManager.GetUsersInRoleAsync(role);
        }

        public Task<ApplicationUser> GetUserByName(string username)
        {
            return _userManager.FindByNameAsync(username);
        }
    }
}