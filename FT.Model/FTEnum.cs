using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FT.Model
{
    /// <summary>
    /// 玩法枚举()
    /// </summary>
    public enum PlayEnum
    {
        /// <summary>
        /// 独赢
        /// </summary>
        [Description("独赢|FT.1X2|FT.1X2")]
        OnlyWinAll = 10,

        /// <summary>
        /// 半场独赢
        /// </summary>
        [Description("半场独赢|1H.1X2|1H.1X2")]
        OnlyWinHalf = 11,

        /// <summary>
        /// 让球
        /// </summary>
        [Description("让球|FT.HDP|FT.HDP")]
        LetBallAll = 12,

        /// <summary>
        /// 让球半场
        /// </summary>
        [Description("半场让球|1H.HDP|1H.HDP")]
        LetBallHalf = 13,

        /// <summary>
        /// 大小球
        /// </summary>
        [Description("大小球|FT.O/U|FT.O/U")]
        BallZiseAll = 14,

        /// <summary>
        /// 半场大小球
        /// </summary>
        [Description("半场大小球|1H.O/U|1H.O/U")]
        BallZiseHalf = 15,

        /// <summary>
        /// 单双
        /// </summary>
        [Description("单双|O/E|O/E")]
        FirstsdAll = 16,

        /// <summary>
        /// 总入球
        /// </summary>
        [Description("总入球|Total Goal|Total Goal")]
        TotalBallAll = 20,

        /// <summary>
        /// 波胆
        /// </summary>
        [Description("波胆|Correct Score|Correct Score")]
        CorrectScoreAll = 30,

        /// <summary>
        /// 半全场
        /// </summary>
        [Description("半全场|HT/FT|HT/FT")]
        HalfFull = 40,

        /// <summary>
        /// 综合全场独赢
        /// </summary>
        [Description("综合独赢|FT.1X2|FT.1X2")]
        ComOnlyWinAll = 50,

        /// <summary>
        /// 综合半场独赢
        /// </summary>
        [Description("综合半场独赢|1H.1X2|1H.1X2")]
        ComOnlyWinHalf = 51,

        /// <summary>
        /// 综合全场让球
        /// </summary>
        [Description("综合让球|FT.HDP|FT.HDP")]
        ComLetBallAll = 52,

        /// <summary>
        /// 综合半场让球
        /// </summary>
        [Description("综合半场让球|1H.HDP|1H.HDP")]
        ComLetBallHalf = 53,

        /// <summary>
        /// 综合全场大小球
        /// </summary>
        [Description("综合大小球|FT.O/U|FT.O/U")]
        ComBallZiseAll = 54,

        /// <summary>
        /// 综合半场大小球
        /// </summary>
        [Description("综合半场大小球|1H.O/U|1H.O/U")]
        ComBallZiseHalf = 55,

        /// <summary>
        /// 综合单双
        /// </summary>
        [Description("综合单双|O/E|O/E")]
        ComFirstsdAll = 56,

        /// <summary>
        /// 滚球全场独赢
        /// </summary>
        [Description("滚球全场独赢|RBFT.1X2|RBFT.1X2")]
        RollBallWinAll = 60,

        /// <summary>
        /// 滚球全场大小球
        /// </summary>
        [Description("滚球大小球|RBFT.O/U|RBFT.O/U")]
        RollBallZiseAll = 61,

        /// <summary>
        /// 滚球半全场(只有三种情况)(HN,HC,CN)
        /// </summary>
        [Description("滚球半全场|RB1x2|RB1x2")]
        RoleBallMoreChance = 62,

        /// <summary>
        /// 滚球让球
        /// </summary>
        [Description("滚球让球|RBHDP|RBHDP")]
        RoleBallHalf = 63
    }

    /// <summary>
    /// 数据锁定枚举
    /// </summary>
    public enum LockEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常|Nnormal|Nnormal")]
        Nnormal = 0,

        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结|Freeze|O congelamento de")]
        Freeze = 1,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除|Delete|Del")]
        Delete = 2
    }

    /// <summary>
    /// 菜单枚举
    /// </summary>
    public enum MenuEnum
    {
        /// <summary>
        /// 菜单
        /// </summary>
        [Description("菜单|Menu|Menu")]
        Menu = 0,

        /// <summary>
        /// 按钮
        /// </summary>
        [Description("按钮|Button|Botão")]
        Button = 1,

        /// <summary>
        /// 列
        /// </summary>
        [Description("列操作|Cell|A coluna")]
        Cell = 2
    }

    /// <summary>
    /// 会员注册来源
    /// </summary>
    public enum PlatEnum
    {
        /// <summary>
        /// 移动端
        /// </summary>
        [Description("移动端|Mobile|Mobile")]
        Mobile = 0,

        /// <summary>
        /// Web端
        /// </summary>
        [Description("Web端|Web|Web")]
        Web = 1,

        /// <summary>
        /// 客户端
        /// </summary>
        [Description("客户端|Clent|Clent")]
        Clent = 2
    }

    /// <summary>
    /// 会员注册枚举
    /// </summary>
    public enum RegisterEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功|Success|O SUCESSO Da operação")]
        Success = 0,

        /// <summary>
        /// 帐号已存在
        /// </summary>
        [Description("帐号已存在|Account already exists|A conta já existe")]
        Existing = 1,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败|Failure|A operação falhou")]
        Failure = 2
    }

    /// <summary>
    /// 交易枚举(TradeRecord表只用1，2)
    /// </summary>
    public enum TradEnum
    {
        /// <summary>
        /// 余额转账
        /// </summary>
        [Description("余额转账|Balance transfer|Transferência de saldo")]
        Transfer = 1,

        /// <summary>
        /// 余额充值
        /// </summary>
        [Description("余额充值|Balance recharge|O saldo de recarga")]
        Recharge = 2,

        /// <summary>
        /// 注单投注
        /// </summary>
        [Description("注单投注|Single bet|UMA única aposta de injeção")]
        Bet = 3,

        /// <summary>
        /// 余额提现
        /// </summary>
        [Description("余额提现|Balance cash withdrawal|O saldo")]
        Cash = 4,
        /// <summary>
        /// 比赛中奖
        /// </summary>
        [Description("比赛中奖|Win the game|Jogo de loteria")]
        MatchLottery = 5,
        /// <summary>
        /// 比赛取消
        /// </summary>
        [Description("比赛取消|Match cancel|O cancelamento do Jogo")]
        MatchCancel = 6
    }

    /// <summary>
    /// 是否结算(比赛,下注可用)
    /// </summary>
    public enum OpenEnum : byte
    {
        /// <summary>
        /// 未结算
        /// </summary>
        [Description("未结算|Not settled|Saldo")]
        Settlementing = 0,

        /// <summary>
        /// 已结算
        /// </summary>
        [Description("已结算|Settled|Já a liquidação")]
        Settlemented = 1,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消|Canceled|Foi cancelada")]
        Canceled = 2
    }

    /// <summary>
    /// 投注枚举
    /// </summary>
    public enum BetEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功|Success|O SUCESSO Da operação")]
        Success = 0,

        /// <summary>
        /// 不存在当前用户
        /// </summary>
        [Description("不存在当前用户|Does not exist for the current user|Não existem utilizadores")]
        UserNotFind = 1,

        /// <summary>
        /// 余额不足
        /// </summary>
        [Description("余额不足|Credit is running low|Saldo insuficiente")]
        BalanceNotEnough = 2,

        /// <summary>
        /// 未选择下注
        /// </summary>
        [Description("未选择下注|Did not choose to bet|Não escolha.")]
        ObjectNull = 3,

        /// <summary>
        /// 投注金额为0
        /// </summary>
        [Description("投注金额为0|Bet amount is 0|O montante Da aposta.")]
        BetAmountZero = 4,

        /// <summary>
        /// 投注已过截至时间
        /// </summary>
        [Description("投注已过截至时间|Bet over time|A aposta FOI EM tempo")]
        BetOverTime = 5,

        /// <summary>
        /// 投注额不再允许范围
        /// </summary>
        [Description("投注额不再允许范围|Betting amount is no longer allowed|A quantidade de apostas não permite")]
        BetAmountOverRange = 6,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败|Failure|A operação falhou")]
        Failure = 9
    }

    /// <summary>
    /// 语言
    /// </summary>
    public enum LanguageEnum
    {
        /// <summary>
        /// 中文
        /// </summary>
        [Description("中文")]
        cn = 0,

        /// <summary>
        /// English
        /// </summary>
        [Description("English")]
        en = 1,

        /// <summary>
        /// Portugal
        /// </summary>
        [Description("Portugal")]
        pt = 2
    }

    /// <summary>
    /// 转账，充值
    /// </summary>
    public enum TransAccountsEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功|Success|O SUCESSO Da operação")]
        Success = 0,

        /// <summary>
        /// 余额不足
        /// </summary>
        [Description("余额不足|Credit is running low|Saldo insuficiente")]
        BalanceLack = 1,

        /// <summary>
        /// 充值卡无效
        /// </summary>
        [Description("充值卡无效|Invalid card|Nenhum cartão de recarga")]
        CardInvalid = 2,

        /// <summary>
        /// 账户无效
        /// </summary>
        [Description("账户无效|Invalid account|Conta inválida")]
        UserInvalid = 3,

        /// <summary>
        /// 安全密码错误
        /// </summary>
        [Description("安全密码错误|Security password error|Código de segurança errado")]
        SafeCodeInvalid = 4,

        /// <summary>
        /// 提现记录为空
        /// </summary>
        [Description("提现记录为空|Record is empty|Para o Registro")]
        CashNullInvalid = 5,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败|Failure|A operação falhou")]
        Failure = 99,
    }

    /// <summary>
    /// 编辑枚举
    /// </summary>
    public enum EditEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功|Success|O SUCESSO Da operação")]
        Success = 0,

        /// <summary>
        /// 账户已锁定
        /// </summary>
        [Description("账户已锁定|Account is locked|Já o bloqueio de Contas")]
        IsLock = 1,

        /// <summary>
        /// 原密码错误
        /// </summary>
        [Description("原密码错误|Original password error|O erro de senha")]
        OriginalPassError = 2,

        /// <summary>
        /// 账户不存在
        /// </summary>
        [Description("账户不存在|Does not exist for the current user|A conta não existe")]
        AccountNotFind =
            3,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败|Failure|A operação falhou")]
        Failure = 99,
    }

    /// <summary>
    /// 代理商级别
    /// </summary>
    public enum LevelEnum
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无|None|SEM")]
        None = 0,

        /// <summary>
        /// 一级代理
        /// </summary>
        [Description("一级代理|Primary agent|Agente de nível 1")]
        PrimaryAgent = 1,

        /// <summary>
        /// 二级代理
        /// </summary>
        [Description("二级代理|Two agents|Agente de nível 2")]
        TwoAgent = 2,

        /// <summary>
        /// 三级代理
        /// </summary>
        [Description("三级代理|Three agents|Agentes de nível 3")]
        ThreeAgent = 3
    }

    /// <summary>
    /// 用户类别
    /// </summary>
    public enum UserTypeEnum
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        [Description("普通用户|Ordinary users|O usuário Comum")]
        Ordinary = 0,

        /// <summary>
        /// 代理商
        /// </summary>
        [Description("代理商|Agent|Agentes")]
        Agent = 1
    }

    /// <summary>
    /// 提现审核
    /// </summary>
    public enum CashAuditEnum
    {
        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核|Pending audit|A aprovação")]
        WaitAudit = 0,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核|Audited|Já a auditoria")]
        Audited = 1,

        /// <summary>
        /// 已打款
        /// </summary>
        [Description("已打款|Paymented|Já o pagamento")]
        Paymented = 2,

        /// <summary>
        /// 不通过
        /// </summary>
        [Description("不通过|No pass|Não através de")]
        NoPass = 9
    }

    /// <summary>
    /// 是否
    /// </summary>
    public enum YesNoEnum
    {
        /// <summary>
        /// 否
        /// </summary>
        [Description("否|No|Não")]
        No = 0,

        /// <summary>
        /// 是
        /// </summary>
        [Description("是|Yes|é")]
        Yes = 1
    }

    /// <summary>
    /// 投注单兑奖
    /// </summary>
    public enum PrizeEnum
    {
        /// <summary>
        /// 操作成功
        /// </summary>
        [Description("操作成功|Success|O SUCESSO Da operação")]
        Success = 0,

        /// <summary>
        /// 投注单不属于用户
        /// </summary>
        [Description("投注单不属于该账户|Betting alone does not belong to the user|A aposta não pertence a conta")]
        BetIsNotBelongUser = 1,

        /// <summary>
        /// 投注单已兑奖
        /// </summary>
        [Description("投注单已失效 |The single has been betting lottery|Já a aposta de loteria")]
        Prized = 2,

        /// <summary>
        /// 投注单不存在
        /// </summary>
        [Description("投注单不存在|Bet does not exist|A aposta não existe")]
        BetDoesNotExists = 3,

        /// <summary>
        /// 未中奖
        /// </summary>
        [Description("未中奖|Not winning|Não Ganhar")]
        NotWinning = 4,

        /// <summary>
        /// 未开奖
        /// </summary>
        [Description("未开奖|No lottery|Não houve")]
        NoLottery = 5,

        /// <summary>
        /// 操作失败
        /// </summary>
        [Description("操作失败|Failure|A operação falhou")]
        Failure = 9
    }
    /// <summary>
    /// Http Error
    /// </summary>
    public enum HttpErrorEnum
    {
        /// <summary>
        /// 参数校验不通过
        /// </summary>
        [Description("参数校验不通过|Parameter check is not passed|O parâmetro de calibração não")]
        SignError,
        /// <summary>
        /// 未配置任何参数
        /// </summary>
        [Description("未配置任何参数|No parameters are configured|SEM qualquer parâmetro de configuração")]
        AgrumentsNull,
        /// <summary>
        /// 登录信息已失效
        /// </summary>
        [Description("登录信息已失效,请重新登录|Login is invalid,please re login|Falha de logon que faça o login novamente")]
        LoginFailed,
        /// <summary>
        /// 账户在其他地方登录
        /// </summary>
        [Description("账户在其他地方登录,请重新登录|Account login in other places,please re login|EM outros locais, Contas de login, faça o login novamente")]
        AccountLoginOther
    }

    /// <summary>
    /// 数据来源
    /// </summary>
    public enum DataSourceEnum
    {
        /// <summary>
        /// HGA(皇冠)
        /// </summary>
        [Description("HGA|HGA|HGA")]
        HGA = 0,
        /// <summary>
        /// 1XBET
        /// </summary>
        [Description("XBET|XBET|XBET")]
        XBET = 1
    }

    public enum TaskRunEnum
    {
        /// <summary>
        /// 启动
        /// </summary>
        [Description("启动|Run|Run")]
        Run = 0,
        /// <summary>
        /// 停止
        /// </summary>
        [Description("停止|Stop|Stop")]
        Stop = 1
    }
    /// <summary>
    /// 注单状态
    /// </summary>
    public enum BetStatus
    {
        /// <summary>
        /// 正常注单
        /// </summary>
        [Description("正常注单|Nnormal|Nnormal")]
        Nnormal = 0,

        /// <summary>
        /// 危险注单
        /// </summary>
        [Description("危险注单|Danger|Danger")]
        Danger = 1,

        /// <summary>
        /// 已处理注单（已结算、已取消）
        /// </summary>
        [Description("已处理注单|Processed|Processed")]
        Processed = 2
    }
}
