using UnityEngine;
using System; // Needed for Math

public class PulseGenerator : MonoBehaviour
{
    // un-optimized version
    public double frequency = 440;
    public double gain = 0.05;
    public double pulseFrequency = 4;
    private double increment;
    private double pulseIncrement;
    private double phase;
    private double sampling_frequency = 48000;
    private double pulsePhase;

    void OnAudioFilterRead(float[] data, int channels)
    {
        // update increment in case frequency has changed
        increment = frequency * 2 * Math.PI / sampling_frequency;
        pulseIncrement = pulseFrequency * 2 * Math.PI / sampling_frequency;
        double pulseAmplitude = 0;
        for (var i = 0; i < data.Length; i = i + channels)
        {
            phase = phase + increment;
            pulsePhase = pulsePhase + pulseIncrement;
            // this is where we copy audio data to make them “available” to Unity
            pulseAmplitude = (float)(SquareFunction(pulsePhase));
            data[i] = (float)(gain * pulseAmplitude * Math.Sin(phase));

            // if we have stereo, we copy the mono data to each channel
            // if we have stereo, we copy the mono data to each channel
            if (channels == 2) data[i + 1] = data[i];
            if (phase > 2 * Math.PI) phase = 0;
        }
    }

    double SquareFunction(double phase)
    {
        if ((float)(Mathf.Sin((float)phase)) >= 0)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }




}