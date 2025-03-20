using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;  // Added for scene switching.

public class CutsceneController : MonoBehaviour {
    [Header("Effects & Panels")]
    public GameObject scanlineEffect;    // Scanline effect panel.
    public GameObject infoBox;           // Panel for crew info.
    
    [Header("Scrolling & Typewriter Text")]
    public TextMeshProUGUI scrollingText;   // Scrolling text element.
    public TextMeshProUGUI typewriterText;    // Typewriter text element.
    
    [Header("Warning Symbol")]
    public UnityEngine.UI.Image warningSymbol; // Warning symbol image.
    
    [Header("Captain Info (Legacy)")]
    // (Not used after switching to multi-crew info; do not change any code not related to the crew switching.)
    public UnityEngine.UI.Image captainImage;    // Captain’s profile image.
    public TextMeshProUGUI captainName;            // Captain’s name text.
    public TextMeshProUGUI statusText;             // Text for cycling statuses.
    
    [Header("Crew Member Info")]
    // New arrays for unique crew member images and names (index 0 = Captain, 1 = Executive Officer, 2 = Chief Engineer, 3 = Chief Scientist).
    public UnityEngine.UI.Image[] crewMemberImages;
    public TextMeshProUGUI[] crewMemberNames;
    
    [Header("Typing Sound")]
    public AudioSource typingAudio; // AudioSource with your looping typing .wav.
    
    [Header("Timing Parameters")]
    public float charDelay = 0.005f;           // Delay per character.
    public float dotCycleDelay = 0.1f;        // Delay between dot-cycle states.
    public int dotCycleCount = 1;             // How many times to cycle dots.
    public float lineDelayBoot = 0.2f;          // Extra delay after each boot message.
    public float lineDelayShip = 0.2f;          // Extra delay after each ship status message.
    public float clearLineDelay = 0.2f;       // Delay when clearing one line.
    
    void Start() {
        // Start with audio paused so it doesn't play before text appears.
        if (typingAudio != null) {
            typingAudio.loop = true;
            typingAudio.Pause();
        }
        StartCoroutine(RunCutscene());
    }
    
    IEnumerator RunCutscene() {
        // --- Screen Turn On ---
        scanlineEffect.SetActive(true);
        yield return new WaitForSeconds(1f);
        
        // --- Activate Scrolling & Typewriter Text ---
        scrollingText.gameObject.SetActive(true);
        typewriterText.gameObject.SetActive(true);
        typewriterText.text = "";
        
        // --- Boot/Initialization Messages ---
        string[] bootMessages = {
            "Turning on main power... Failure",
            "Turning on backup power... Success",
            "Initializing system checks... Success",
            "Calibrating sensors... Success",
            "Loading navigation systems... Success"
        };
        
        foreach (string msg in bootMessages) {
            yield return StartCoroutine(TypeTextAtTop(msg, true));
            yield return new WaitForSeconds(lineDelayBoot);
        }
        
        // Pause briefly then clear boot messages one line at a time (bottom up).
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ClearTextLineByLine());
        yield return new WaitForSeconds(1f);
        
        // --- Ship Status Messages (without dot-cycle effect) ---
        string[] shipStatusMessages = {
            "Warning: Engine Failure Detected",
            "Warning: Navigation System Offline",
            "Warning: Life Support Malfunction",
            "Warning: Bridge Depressurization",
            "Warning: Radiation Levels Critical",
            "Warning: Lab Containment Breach Detected!!!"
        };
        
        foreach (string msg in shipStatusMessages) {
            yield return StartCoroutine(TypeTextAtTop(msg, false));
            yield return new WaitForSeconds(lineDelayShip);
        }
        
        // --- Flash Warning Symbol ---
        warningSymbol.gameObject.SetActive(true);
        // Get the parent GameObject of the warning symbol.
        GameObject warningParent = warningSymbol.transform.parent.gameObject;

        // Activate the parent so that its script runs.
        warningParent.SetActive(true);

        // Get the WarningFlasher component attached to the parent.
        WarningFlasher flasher = warningParent.GetComponent<WarningFlasher>();
        if (flasher != null) {
            flasher.StartFlashing();
        }
        // yield return new WaitForSeconds(1f);
        // warningSymbol.gameObject.SetActive(false);
        
        // --- Clear Ship Status Messages ---
        yield return StartCoroutine(ClearTextLineByLine());
        typewriterText.gameObject.SetActive(false);
        // scrollingText.gameObject.SetActive(false);
        
        // --- Show Crew Member Info ---
        infoBox.SetActive(true);
        // Disable legacy captain info.
        if (captainName != null) captainName.gameObject.SetActive(false);
        if (captainImage != null) captainImage.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);
        
        // Enable the first crew member info (Captain) from the new arrays.
        if (crewMemberNames != null && crewMemberNames.Length > 0) {
            crewMemberNames[0].gameObject.SetActive(true);
        }
        if (crewMemberImages != null && crewMemberImages.Length > 0) {
            crewMemberImages[0].gameObject.SetActive(true);
        }
        
        // --- Cycle Through Statuses (with caret indicator; no sound during dot cycles) ---
        yield return StartCoroutine(CycleStatus());
        
        // --- Final Crew Status Message ---
        yield return StartCoroutine(TypeTextOnStatus("Beginning defrosting... Success"));
        yield return new WaitForSeconds(2f);
        
        // --- End Cutscene ---
        infoBox.SetActive(false);
        
        // Automatically switch to the next scene.
        SceneManager.LoadScene("1 - Freezer");
    }
    
    // Wait actively: unpause audio, wait for duration, then pause.
    IEnumerator WaitActive(float duration) {
        if (typingAudio != null) typingAudio.UnPause();
        yield return new WaitForSeconds(duration);
        if (typingAudio != null) typingAudio.Pause();
    }
    
    // Wait silently: ensure audio remains paused during the wait.
    IEnumerator WaitSilent(float duration) {
        if (typingAudio != null) typingAudio.Pause();
        yield return new WaitForSeconds(duration);
    }
    
    // Types a message one character at a time at the top.
    // For boot messages, if the message contains "..." it will be replaced by a dot-cycle effect.
    IEnumerator TypeTextAtTop(string message, bool isBootMessage) {
        string previousText = typewriterText.text; // Existing lines.
        string currentLine = "";
        
        if (isBootMessage && message.Contains("...")) {
            int ellipsisIndex = message.IndexOf("...");
            string beforeDots = message.Substring(0, ellipsisIndex);
            string afterDots = message.Substring(ellipsisIndex + 3);
            
            // Type text before the dots.
            for (int i = 0; i < beforeDots.Length; i++) {
                currentLine += beforeDots[i];
                typewriterText.text = currentLine + "|" + "\n" + previousText;
                yield return WaitActive(charDelay);
            }
            
            // Dot-cycle effect (no sound during these waits).
            for (int cycle = 0; cycle < dotCycleCount; cycle++) {
                string[] dotStates = { ".", "..", "..." };
                foreach (string state in dotStates) {
                    typewriterText.text = currentLine + state + "|" + "\n" + previousText;
                    yield return WaitSilent(dotCycleDelay);
                }
            }
            
            // After cycling, type a colon.
            currentLine += ":";
            typewriterText.text = currentLine + "|" + "\n" + previousText;
            yield return WaitActive(charDelay);
            
            // Type the remaining text (e.g. " Success" or " Failure").
            for (int i = 0; i < afterDots.Length; i++) {
                currentLine += afterDots[i];
                typewriterText.text = currentLine + "|" + "\n" + previousText;
                yield return WaitActive(charDelay);
            }
        } else {
            // Normal typing character-by-character.
            for (int i = 0; i < message.Length; i++) {
                currentLine += message[i];
                typewriterText.text = currentLine + "|" + "\n" + previousText;
                yield return WaitActive(charDelay);
            }
        }
        // Remove the caret when finished.
        typewriterText.text = currentLine + "\n" + previousText;
        yield return null;
    }
    
    // Clears the typewriter text one line at a time from the bottom up.
    IEnumerator ClearTextLineByLine() {
        string fullText = typewriterText.text;
        if (string.IsNullOrEmpty(fullText))
            yield break;
        
        string[] lines = fullText.Split('\n');
        for (int i = lines.Length - 1; i >= 0; i--) {
            string newText = "";
            for (int j = 0; j < i; j++) {
                newText += lines[j] + "\n";
            }
            typewriterText.text = newText.TrimEnd('\n');
            yield return WaitActive(clearLineDelay);
        }
        yield return null;
    }
    
    // Types text on the status text one character at a time with a caret.
    IEnumerator TypeTextOnStatus(string message) {
        statusText.text = "";
        string currentLine = "";
        for (int i = 0; i < message.Length; i++) {
            currentLine += message[i];
            statusText.text = currentLine + "|";
            yield return WaitActive(charDelay);
        }
        statusText.text = currentLine;
        yield return null;
    }
    
    // Cycles through crew roles, updating the displayed crew member image and name,
    // and displaying a dot-loading effect with a caret (no sound during dot-cycle).
    IEnumerator CycleStatus() {
        string[] roles = { "Captain", "Executive Officer", "Chief Engineer", "Chief Scientist" };
        for (int i = 0; i < roles.Length; i++) {
            // Update crew info to display the current crew member.
            if (crewMemberImages != null) {
                foreach (var img in crewMemberImages) {
                    img.gameObject.SetActive(false);
                }
                if (i < crewMemberImages.Length) {
                    crewMemberImages[i].gameObject.SetActive(true);
                }
            }
            if (crewMemberNames != null) {
                foreach (var txt in crewMemberNames) {
                    txt.gameObject.SetActive(false);
                }
                if (i < crewMemberNames.Length) {
                    crewMemberNames[i].gameObject.SetActive(true);
                }
            }
            
            string baseText = roles[i] + " Status: ";
            float dotCycleDuration = 1.5f;
            float dotCycleInterval = 0.5f;
            float elapsed = 0f;
            while (elapsed < dotCycleDuration) {
                string[] cycles = { ".", "..", "..." };
                foreach (string cyc in cycles) {
                    statusText.text = baseText + cyc + "|";
                    yield return WaitSilent(dotCycleInterval);
                    elapsed += dotCycleInterval;
                    if (elapsed >= dotCycleDuration)
                        break;
                }
            }
            string finalStatus = (roles[i] == "Chief Scientist") ? " Stable" : " Deceased";
            statusText.text = baseText + finalStatus;
            yield return new WaitForSeconds(1.5f);
        }
        yield return null;
    }
}
