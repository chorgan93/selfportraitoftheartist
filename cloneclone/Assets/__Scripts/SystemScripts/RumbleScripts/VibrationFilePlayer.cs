#if UNITY_SWITCH
using nn.hid;
using UnityEngine;

/// <summary>
/// Handles playback for a VibrationFileResource. 
/// </summary>
public class VibrationFilePlayer
{
    private VibrationFileResource m_vibrationFile; // The file that we're sampling
    private uint m_filePosition = 0;    // Where we are right now in the playback of the file
    private bool m_isPlaying = false;

    /// <summary>
    /// Constructor for VibrationFilePlayer. 
    /// </summary>
    /// <param name="vibrationFileResource">The VibrationFileResource that this VibrationFilePlayer will play.</param>
    public VibrationFilePlayer(VibrationFileResource vibrationFileResource)
    {
        Debug.Assert(vibrationFileResource != null);

        m_vibrationFile = vibrationFileResource;
    }

    /// <summary>
    /// Returns the next sample for the effect, if playing. One sample represents 1/200th of a second of playback time.
    /// </summary>
    /// <returns>
    /// If the effect is currently playing, this function returns the next sample and advances the playback position.
    /// If the effect is not currently playing or has reached the end of the effect, returns a VibrationValue
    /// with amplitudes set to 0.0 and frequecies set to resonant frequencies.
    /// </returns>
    public VibrationValue GetNextSample()
    {
        // Start with a default VibrationValue with frequencies set to resonant frequecies and amplitudes set to 0.0.
        // Amplitude is basically the same thing as volume in audio.
        // Note that using the overload of VibrationValue.Make() that takes no arguments would give us the same result
        // as the next line, but the explicit version is used for demonstration purposes. 
        VibrationValue vibrationValue = VibrationValue.Make(0.0f, VibrationValue.FrequencyLowDefault, 0.0f, VibrationValue.FrequencyHighDefault);
        if (!m_isPlaying)
        {
            return vibrationValue;
        }

        VibrationFileInfo fileInfo = m_vibrationFile.FileInfo;
        if (HasLoop() && m_filePosition >= fileInfo.loopEndPosition) // If the file has a loop, have we reached the end of it?
        {
            SetPlayPositionToLoopStart();
        }
        else if (m_filePosition >= fileInfo.sampleLength) // Have we reached the end of the data? 
        {
            SetPlayPositionToFileStart();
            m_isPlaying = false;    // Reached end of file and there's no loop. If there were a loop, we couldn't have made it past the previous 'if'.
            return vibrationValue;  // Don't want to return the first sample (0) if we're not looping, so return the default.
        }

        VibrationFileParserContext parserContext = m_vibrationFile.FileParserContext;
        VibrationFile.RetrieveValue(ref vibrationValue, (int)m_filePosition, ref parserContext);

        ++m_filePosition;

        return vibrationValue;
    }

    /// <summary>
    /// Set the playback position to the start of the effect.
    /// </summary>
    public void SetPlayPositionToFileStart()
    {
        m_filePosition = 0;
    }

    /// <summary>
    /// Set the playback position to the start of the loop.
    /// </summary>
    public void SetPlayPositionToLoopStart()
    {
        Debug.Assert(HasLoop());
        m_filePosition = m_vibrationFile.FileInfo.loopStartPosition;
    }

    /// <summary>
    /// Begin playback.
    /// </summary>
    public void Play()
    {
        m_isPlaying = true;
    }

    /// <summary>
    /// Returns whether or not the effect is currently playing.
    /// </summary>
    /// <returns>Whether or not the effect is currently playing.</returns>
    public bool IsPlaying()
    {
        return m_isPlaying;
    }

    /// <summary>
    /// Returns whether or not the effect contains a loop.
    /// </summary>
    /// <returns>Whether or not the effect contains a loop.</returns>
    public bool HasLoop()
    {
        return m_vibrationFile.FileInfo.isLoop != 0;
    }
}
#endif