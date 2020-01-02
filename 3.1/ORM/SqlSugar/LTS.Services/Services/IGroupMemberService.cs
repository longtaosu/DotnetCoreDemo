using LTS.Services.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LTS.Services.Services
{
    public interface IGroupMemberService
    {
        List<GroupMember> GetUsers();
    }
}
