﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interview.Entities
{

    public class UserSetting : EntityBase
    {

        public UserSetting()
        {
            UserSettingEquities = new List<UserSettingEquity>();
            Equities = new List<Equity>();
        }

        public Guid ContestId { get; set; }
        public Guid? UserLanguageId { get; set;  }
        public Guid UserId { get; set; }
        public string? UserFirstname { get; set; }
        public string? UserLastname { get; set; }
        public bool? IsExternal { get; set; }
        public RoleTypes RoleType { get; set; }
        public DateTime DateInserted { get; set; }

        public UserLanguage? UserLanguage { get; set; }

        // Many to Many
        // https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many
        public List<UserSettingEquity> UserSettingEquities { get; set; }
        public List<Equity> Equities { get; set; }

    }

}
