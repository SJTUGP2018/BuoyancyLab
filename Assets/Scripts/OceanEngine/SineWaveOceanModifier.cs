using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BuoyancyLab.Utility;

public class SineWaveOceanModifier : MonoBehaviour {
	public SineWaveOceanGenerator generator;
	public ClampedFloatEditor waveScale;
	public ClampedFloatEditor waveSpeed;
	public ClampedFloatEditor waveLengthFactor;

	public enum FieldName {
		Scale = 0, Speed, LengthFactor
	};

	public void InitGeneratorValues()
	{
        generator.m_waveScale = waveScale.initValue;
		generator.m_waveSpeed = waveSpeed.initValue;
		generator.m_waveLengthFactor = (int)waveLengthFactor.initValue;

    }


	// Use this for initialization
	void OnEnable () {
		InitGeneratorValues();

		// float[] initValues = {generator.m_waveScale, 
		// 					  generator.m_waveSpeed, 
		// 					  generator.m_waveLengthFactor};
        ClampedFloatEditor[] editorList = {waveScale, waveSpeed, waveLengthFactor};

		for(int i = 0; i < editorList.Length; ++i)
		{
			var editor = editorList[i];
			editor.clamped = new ClampedFloat(editor.initValue, editor.minValue, editor.maxValue);

            FieldName name = (FieldName)i;
			
			Slider slider = editor.slider;
			if(slider)
			{
                slider.maxValue = editor.maxValue;
				slider.minValue = editor.minValue;
				slider.value = editor.clamped.value;

                slider.onValueChanged.AddListener
					((float value) => ChangeFloatValue(value, editor, name));
				
				if(name == FieldName.LengthFactor)
					slider.wholeNumbers = true;
			}
			Text text = editor.text;
			if(text)
			{
				SetTextValue(text, editor.clamped.value);
				if(name == FieldName.LengthFactor)
                    SetTextValue(text, editor.clamped.value, true);
			}
		}

	}

	void ChangeFloatValue(float targetValue, ClampedFloatEditor editor, FieldName name)
	{
        editor.clamped.value = targetValue;
		targetValue = editor.clamped.value;

        SetTextValue(editor.text, editor.clamped.value);

		//Debug.Log(string.Format("{0}: {1}", index, targetValue));
		switch(name)
		{
			case FieldName.Scale:
				generator.m_waveScale = targetValue;
				break;
			case FieldName.Speed:
				generator.m_waveSpeed = targetValue;
				break;
			case FieldName.LengthFactor:
				generator.m_waveLengthFactor = (int)(targetValue);
                SetTextValue(editor.text, editor.clamped.value, true);
				break;
		}
		
	}

	void SetTextValue(Text text, float value, bool isInt = false)
	{
		if(text)
		{
			if(!isInt)
				text.text = string.Format("{0:F2}", value);
			else
                text.text = string.Format("{0:D}", (int)value);
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
