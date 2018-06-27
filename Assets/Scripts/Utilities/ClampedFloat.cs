using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BuoyancyLab.Utility
{

[System.Serializable]
public struct ClampedFloatEditor
{
	public float initValue;
	public float minValue;
	public float maxValue;
	public Slider slider;
	public Text text;
	public ClampedFloat clamped;
	
}

public struct ClampedFloat 
{
	public ClampedFloat(float _curValue, float _minValue, float _maxValue)
	{
		if(_maxValue < _minValue) _maxValue = _minValue;

		_curValue = Mathf.Clamp(_curValue, _minValue, _maxValue);
		m_value = _curValue;
		
		m_minValue = _minValue;
		m_maxValue = _maxValue;
	}

	public float minValue
	{
		get{
			return m_minValue;
		}
		set{
			float targetMin = value;
			if(m_value < targetMin) m_value = targetMin;
			if(m_maxValue < targetMin) m_maxValue = targetMin;
			m_minValue = targetMin;
		}
	}
	public float maxValue
	{
		get{
			return m_maxValue;
		}
		set{
			float targetMax = value;
			if (m_value > targetMax) m_value = targetMax;
			if (m_minValue > targetMax) m_minValue = targetMax;
			m_maxValue = targetMax;
		}
	}
	float m_minValue;
	float m_maxValue;

	public float value
	{
		get{
			return m_value;
		}
		set{
			m_value = Mathf.Clamp(value, m_minValue, m_maxValue);
		}
	}
	float m_value;

	public float normalizedValue
	{
		get
		{
			return (m_value - m_minValue) / (m_maxValue - m_minValue);
		}
	}
}

}
