namespace DelegateDemo;

/*
 * 델리게이트 (대리자)
 *
 * int a = 10;
 *
 * public void Sum(int a , int b) { var result = a + b; }
 *
 * 델리게이트 sum = Sum;
 * sum(10, 20);
 */

class Program
{
    static void Main(string[] args)
    {
        // 플레이어 생성(플레이어 클래스 인스턴스 생성)
        Player player = new Player("Zack", 100);
        
        // 이벤트를 구독(Subscribe)
        // 무명 델리게이트
        // 람다식  (=> goes to, 람다)
        player.OnHpChanged += PlayerDamaged;
        
        // 플레이어 사망 메소드 호출
        // 이벤트를 외부 클래스에서 직접 호출할 수 없다.
        // player.OnPlayerDie?.Invoke();
        
        player.TakeDamage(20);
        player.TakeDamage(40);
        
        player.Heal(30);
        
        player.TakeDamage(40);
        
        // 이벤트 해지(Unsubscribe) : 메모리 해제
        player.OnHpChanged -= PlayerDamaged;
    }

    static void PlayerDamaged(int damage, int hp)
    {
        // 피격 이펙트 처리
        // 피격 사운드 처리
        // 애니메이션 처리
        Console.WriteLine($"데미지 {damage}, HP: {hp}");
    }
}

class Player
{
    public string Name { get; }
    private int _hp;
    
    // 델리게이트 선언
    public delegate void PlayerDieHandler();
    // 델리게이트 변수 선언
    public event PlayerDieHandler OnPlayerDie;
    
    // 이벤트의 정의와 선언 Action
    /* .NET에서 제공하는 내장 델리게이트
     * 반환형이 void 매개변수가 없는 델리게이트 : Action 
     * 반환형이 void 매개변수가 있는 델리게이트 : Action<T1, T2, T3, ... T16>
     */
    // public delegate void HpChangedHandler();
    // public event HpChangedHandler OnHpChanged;
    
    public event Action<int, int> OnHpChanged;
    
    // Func 델리게이트
    /*
     * Func<T1, T2, TResult> 
     * Func<int, float, int> => Func<현재HP, 회복비율, 회복HP>
     */
    public event Func<int, int, int> OnHealing = (currHp, healAmount) => currHp += healAmount;
    

    public Player(string name, int hp)
    {
        Name = name;
        _hp = hp;
        // 할당
        OnPlayerDie += Die;
    }

    public void Heal(int healAmount)
    {
        _hp = OnHealing(_hp, healAmount);
        Console.WriteLine($"HP 회복: {_hp} [회복량: {healAmount}]");
    }
    
    
    public void Die()
    {
        Console.WriteLine("플레이어 사망");
    }

    public void TakeDamage(int damage)
    {
        _hp -= damage;
        
        OnHpChanged?.Invoke(damage, _hp); // 이벤트 발생 Raise
        
        if (_hp <= 0)
        {
            OnPlayerDie?.Invoke(); // 이벤트 Raise
        }
    }
}