using UnityEngine;

public class Ability : MonoBehaviour
{

    public virtual float CooldownDuration { get; }
    public float CurrentCooldown => Mathf.Max(0, (m_cooldownStart + CooldownDuration * AvailableCharges - Time.time) % CooldownDuration);
    private float m_cooldownStart;
    public bool IsOffCooldown => AvailableCharges > 0;

    protected virtual void Awake() => m_cooldownStart = -CooldownDuration * AvailableCharges;

    public int AvailableCharges => System.Math.Min(Charges, (int)System.Math.Floor((Time.time - m_cooldownStart) / CooldownDuration));
    public virtual int Charges => 2;

    public virtual bool CanUse { get; }
    private float m_useStartTime = float.PositiveInfinity;
    public float CurrentUseTime => IsUsing ? Time.time - m_useStartTime : 0;
    public bool IsUsing => m_inUse;
    private bool m_inUse = false;

    private bool m_ButtonPressed;
    public bool ButtonPressed
    {
        set
        {
            m_ButtonPressed = value;
            if (!m_inUse && m_isHoldingButton)
            {
                OnRelease();
                m_isHoldingButton = false;
            }
        }
        private get => m_ButtonPressed;
    }
    public virtual bool RepeatWhenHeld { get; set; }
    private bool m_isHoldingButton;

    void FixedUpdate()
    {
        if (ButtonPressed && CanUse && IsOffCooldown && !m_inUse)
        {
            m_inUse = true;
            m_isHoldingButton = true;
            m_useStartTime = Time.time;
            OnPress();
        }
        if (m_inUse && m_isHoldingButton) OnHold();
        if (m_inUse) OnUsing();
    }
    public virtual void OnPress() { }
    public virtual void OnHold() { }
    public virtual void OnRelease() { }
    public virtual void OnUsing() { }

    protected void CompleteUse()
    {
        if (!m_inUse) return;
        m_inUse = false;
        m_isHoldingButton = false;
        m_cooldownStart = Time.time - (AvailableCharges - 1) * CooldownDuration + CurrentCooldown;
        if (!RepeatWhenHeld) ButtonPressed = false;
    }
}