/*

                                                    /
                                                  .7
                                       \       , //
                                       |\.--._/|//
                                      /\ ) ) ).'/
                                     /(  \  // /
                                    /(   J`((_/ \
                                   / ) | _\     /
                                  /|)  \  eJ    L
                                 |  \ L \   L   L
                                /  \  J  `. J   L
                                |  )   L   \/   \
                               /  \    J   (\   /
             _....___         |  \      \   \```
      ,.._.-'        '''--...-||\     -. \   \
    .'.=.'                    `         `.\ [ Y
   /   /                                  \]  J
  Y / Y                                    Y   L
  | | |          \                         |   L
  | | |           Y                        A  J
  |   I           |                       /I\ /
  |    \          I             \        ( |]/|
  J     \         /._           /        -tI/ |
   L     )       /   /'-------'J           `'-:.
   J   .'      ,'  ,' ,     \   `'-.__          \
    \ T      ,'  ,'   )\    /|        ';'---7   /
     \|    ,'L  Y...-' / _.' /         \   /   /
      J   Y  |  J    .'-'   /         ,--.(   /
       L  |  J   L -'     .'         /  |    /\
       |  J.  L  J     .-;.-/       |    \ .' /
       J   L`-J   L____,.-'`        |  _.-'   |
        L  J   L  J                  ``  J    |
        J   L  |   L                     J    |
         L  J  L    \                    L    \
         |   L  ) _.'\                    ) _.'\
         L    \('`    \                  ('`    \
          ) _.'\`-....'                   `-....'
         ('`    \
          `-.___/ 
*/
namespace FT.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using FT.Model;
    
    public partial class Entity : DbContext
    {
        public Entity()
            : base("name=Entity")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Admin> Admin { get; set; }
        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<CardGroup> CardGroup { get; set; }
        public virtual DbSet<MatchInfo> MatchInfo { get; set; }
        public virtual DbSet<MatchInfoLog> MatchInfoLog { get; set; }
        public virtual DbSet<MatchUserBet> MatchUserBet { get; set; }
        public virtual DbSet<MatchUserBetContent> MatchUserBetContent { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RolePrivilege> RolePrivilege { get; set; }
        public virtual DbSet<UserAccount> UserAccount { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<RoleMenu> RoleMenu { get; set; }
        public virtual DbSet<MatchResult> MatchResult { get; set; }
        public virtual DbSet<CashRecord> CashRecord { get; set; }
        public virtual DbSet<UserBankInfo> UserBankInfo { get; set; }
        public virtual DbSet<TradeRecord> TradeRecord { get; set; }
        public virtual DbSet<LogInfo> LogInfo { get; set; }
        public virtual DbSet<SystemSetting> SystemSetting { get; set; }
        public virtual DbSet<AmountWater> AmountWater { get; set; }
        public virtual DbSet<MatchBlackList> MatchBlackList { get; set; }
        public virtual DbSet<AdminMessage> AdminMessage { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<SiteAdv> SiteAdv { get; set; }
        public virtual DbSet<UserMessage> UserMessage { get; set; }
        public virtual DbSet<XBETMatchInfo> XBETMatchInfo { get; set; }
        public virtual DbSet<XBETMatchResult> XBETMatchResult { get; set; }
        public virtual DbSet<XBETMatchInfoLog> XBETMatchInfoLog { get; set; }
        public virtual DbSet<MatchUserBetLog> MatchUserBetLog { get; set; }
        public virtual DbSet<MatchUserBetContentLog> MatchUserBetContentLog { get; set; }
        public virtual DbSet<SystemDictionary> SystemDictionary { get; set; }
        public virtual DbSet<SystemTask> SystemTask { get; set; }
        public virtual DbSet<BeiJingKL8Archive> BeiJingKL8Archive { get; set; }
        public virtual DbSet<BeiJingKL8Source> BeiJingKL8Source { get; set; }
        public virtual DbSet<GameBeijing16Source> GameBeijing16Source { get; set; }
        public virtual DbSet<GameBeijing28Source> GameBeijing28Source { get; set; }
    }
}
