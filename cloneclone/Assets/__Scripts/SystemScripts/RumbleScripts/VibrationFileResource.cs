#if UNITY_SWITCH
using UnityEngine;
using nn.hid;

/// <summary>
/// Simple class that wraps state and functionality related to loading and playing BNVIB files.
/// </summary>
public class VibrationFileResource
{
    private byte[] m_fileBuffer;    // Keep a reference to this to make sure it doesn't get garbage collected
    private VibrationFileInfo m_fileInfo = new VibrationFileInfo();
    private VibrationFileParserContext m_fileParserContext = new VibrationFileParserContext();

    /// <summary>
    /// Gets the VibrationFileInfo for this vibration file.
    /// </summary>
    public VibrationFileInfo FileInfo
    {
        get
        {
            return m_fileInfo;
        }
    }

    /// <summary>
    /// Returns the VibrationFileParserContext for this vibration file.
    /// </summary>
    public VibrationFileParserContext FileParserContext
    {
        get
        {
            return m_fileParserContext;
        }
    }
    

    /// <summary>
    /// Constructs the VibrationFileResource, storing a reference to fileBuffer so it's not garbage collected and parsing its contents.
    /// </summary>
    /// <param name="fileBuffer">The data read in from the BNVIB file</param>
    public VibrationFileResource(byte[] fileBuffer)
    {
        Debug.Assert(fileBuffer != null);

        m_fileBuffer = fileBuffer;
        nn.Result result = VibrationFile.Parse(ref m_fileInfo, ref m_fileParserContext, m_fileBuffer, m_fileBuffer.Length);

        // The following should always succeed unless the BNVIB data is corrupt. 
        // If this is a concern, consider adding more error checking.
        Debug.Assert(result.IsSuccess());
    }
}
#endif