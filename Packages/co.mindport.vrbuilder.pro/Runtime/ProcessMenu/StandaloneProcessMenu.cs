using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRBuilder.Core;
using VRBuilder.Core.Configuration;
using VRBuilder.Unity;

namespace VRBuilder.Pro.ProcessMenu
{
    /// <summary>
    /// Standalone controller class for an example of a custom overlay with audio.
    /// </summary>
    public class StandaloneProcessMenu : BaseProcessControllerMenu
    {
        #region UI elements
        [Tooltip("Chapter picker dropdown.")]
        [SerializeField]
        private TMP_Dropdown chapterPicker = null;

        [Tooltip("The image next to a step name which is visible when a process is running.")]
        [SerializeField]
        private Image stateIndicator = null;

        [Tooltip("Name of the step that is currently executed.")]
        [SerializeField]
        private TextMeshProUGUI stepName = null;

        [Tooltip("Button that shows additional information about the step.")]
        [SerializeField]
        private Toggle stepInfoToggle = null;

        [Tooltip("Background for additional step information.")]
        [SerializeField]
        private Image stepInfoBackground = null;

        [Tooltip("Short description of the text which is visible when Info Toggle is toggled on.")]
        [SerializeField]
        private TextMeshProUGUI stepInfoText = null;

        [Tooltip("Button that starts execution of the process.")]
        [SerializeField]
        private Button startProcessButton = null;

        [Tooltip("Step picker dropdown.")]
        [SerializeField]
        private TMP_Dropdown skipStepPicker = null;

        [Tooltip("Button that resets the scene to its initial state.")]
        [SerializeField]
        private Button resetSceneButton = null;

        [Tooltip("Toggle that turns process audio on or off.")]
        [SerializeField]
        private Toggle soundToggle = null;

        [Tooltip("Image that shows the sound icon.")]
        [SerializeField]
        private Image soundImage = null;

        [Tooltip("Icon that indicates that sound is enabled.")]
        [SerializeField]
        private Sprite soundOnImage = null;

        [Tooltip("Icon that indicates that sound is disabled.")]
        [SerializeField]
        private Sprite soundOffImage = null;
        #endregion

        private FieldInfo skipStepPickerEditorValueField;

        private IStep displayedStep;
        private IProcess process;
        private IChapter lastDisplayedChapter;

        private void Awake()
        {
            ProcessRunner.Events.ProcessInitialized += SetupMenu;
        }

        private void SetupMenu(object sender, ProcessEventArgs e)
        {
            // Setup UI controls.
            SetupChapterPicker();
            SetupStepInfoToggle();
            SetupStartProcessButton();
            SetupSkipStepPicker();
            SetupResetSceneButton();
            SetupSoundToggle();

            // Update the UI.
            SetupProcessDependantUI();

            // Subscribe to events.
            SubscribeToProcessEvents();
        }

        private void Update()
        {
            IChapter currentChapter = ProcessRunner.Current == null ? null : ProcessRunner.Current.Data.Current;
            IStep currentStep = currentChapter?.Data.Current;

            if (currentChapter != lastDisplayedChapter)
            {
                lastDisplayedChapter = currentChapter;
                UpdateDisplayedChapter(currentChapter);
            }

            if (currentStep != displayedStep)
            {
                displayedStep = currentStep;
                UpdateDisplayedStep(currentStep);
            }
        }

        private void OnDisable()
        {
            UnsubscribeFromProcessEvents();
        }

        private void UpdateDisplayedStep(IStep step)
        {
            if (step == null)
            {
                // If there is no next step, clear the info text.
                stepInfoText.text = string.Empty;
                stepName.text = string.Empty;
            }
            else
            {
                // Else, assign the description of the new step.
                stepInfoText.text = step.Data.Description;
                stepName.text = step.Data.Name;
            }

            SetupSkipStepPickerOptions();
        }

        private void UpdateDisplayedChapter(IChapter chapter)
        {
            // Get a collection of available chapters.
            IList<IChapter> chapters = ProcessRunner.Current == null ? new List<IChapter>() : ProcessRunner.Current.Data.Chapters.ToList();

            // Skip all finished chapters.
            int startingIndex = chapter == null ? 0 : chapters.IndexOf(chapter);

            // Show the rest.
            PopulateChapterPickerOptions(startingIndex);
        }

        private async void SetupProcess()
        {
            // Load process from a file.
            string path = RuntimeConfigurator.Instance.GetSelectedProcess();

            // Try to load the in the PROCESS_CONFIGURATION selected process.
            try
            {
                process = await RuntimeConfigurator.Configuration.LoadProcess(path);
            }
            catch (Exception exception)
            {
                Debug.LogError($"{exception.GetType().Name}, {exception.Message}\n{exception.StackTrace}", RuntimeConfigurator.Instance.gameObject);
                return;
            }

            // Initializes the process. That will synthesize an audio for the process instructions, too.
            ProcessRunner.Initialize(process);
        }

        private void FastForwardChapters(int numberOfChapters)
        {
            // Skip if no chapters have to be fast-forwarded.
            if (numberOfChapters == 0)
            {
                return;
            }

            ProcessRunner.SkipChapters(numberOfChapters);
        }

        #region Setup UI
        private void SetupChapterPicker()
        {
            // When selected chapter has changed,
            chapterPicker.onValueChanged.AddListener(index =>
            {
                if (ProcessRunner.IsRunning == false)
                {
                    return;
                }

                // If the process hasn't started it, ignore it. We will use this value when the process starts.
                if (ProcessRunner.Current.LifeCycle.Stage == Stage.Inactive)
                {
                    return;
                }

                // Otherwise, fast forward the chapters until the selected is active.
                FastForwardChapters(index);
            });
        }

        private void SetupStepInfoToggle()
        {
            // When info toggle is pressed,
            stepInfoToggle.onValueChanged.AddListener(newValue =>
            {
                if (string.IsNullOrEmpty(stepInfoText.text))
                {
                    // Show or hide description of the step.
                    stepInfoBackground.enabled = false;
                    stepInfoText.enabled = false;
                    return;
                }

                // Show or hide description of the step.
                stepInfoBackground.enabled = newValue;
                stepInfoText.enabled = newValue;
            });
        }

        private void SetupStartProcessButton()
        {
            if (ProcessRunner.Current == null)
            {
                Debug.LogError("No process is selected.", RuntimeConfigurator.Instance.gameObject);
                return;
            }

            // When user clicks on Start Process button,
            startProcessButton.onClick.AddListener(() =>
            {
                // Subscribe to the "stage changed" event of the current process in order to change the skip step button to the start button after finishing the process.
                ProcessRunner.Current.LifeCycle.StageChanged += (sender, args) =>
                {
                    if (args.Stage == Stage.Inactive)
                    {
                        skipStepPicker.gameObject.SetActive(false);
                        startProcessButton.gameObject.SetActive(true);
                    }
                };

                //Skip all chapters before selected.
                FastForwardChapters(chapterPicker.value);

                // Start the process
                ProcessRunner.Run();
            });
        }

        private void SetupSkipStepPicker()
        {
            // Dropdown.onValueChanged won't be call if Dropdown.value is equal to the selected value. 
            // Dropdown.value can't be less than 0 or grater than Dropdown.options.Count -1.
            // This causes the dropdown to never call onValueChanged in cases when there is only 1 transition.
            // By setting the value to be out of range from the "editor" instead than from Dropdown.value we ensure that Dropdown.onValueChanged is always called.
            skipStepPickerEditorValueField = skipStepPicker.GetType().GetField("m_Value", BindingFlags.NonPublic | BindingFlags.Instance);

            // When a target step was chosen,
            skipStepPicker.onValueChanged.AddListener(index =>
            {
                // If there's an active step and it's not the last step,
                if (displayedStep != null && index < displayedStep.Data.Transitions.Data.Transitions.Count && displayedStep.LifeCycle.Stage != Stage.Inactive)
                {
                    ProcessRunner.SkipStep(displayedStep.Data.Transitions.Data.Transitions[index]);
                }
            });
        }

        private void SetupResetSceneButton()
        {
            // When user clicks on Reset Scene button,
            resetSceneButton.onClick.AddListener(() =>
            {
                // Stop all coroutines. We cannot properly interrupt a process yet.
                // For example, consider the Move Object behavior: it changes position of an object over time.
                // Even if you would reload the scene, it would still be moving that object, which will lead to unwanted result.
                CoroutineDispatcher.Instance.StopAllCoroutines();

                // Reload current scene.
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
            });
        }

        private void SetupSoundToggle()
        {
            // When sound toggle is clicked,
            soundToggle.onValueChanged.AddListener(isSoundOn =>
            {
                // Set active image for sound.
                soundImage.sprite = isSoundOn ? soundOnImage : soundOffImage;

                // Mute the instruction audio.
                RuntimeConfigurator.Configuration.InstructionPlayer.mute = isSoundOn == false;
            });
        }

        /// <summary>
        /// Subscribes to process events.
        /// </summary>
        protected virtual void SubscribeToProcessEvents()
        {
            ProcessRunner.Events.ProcessStarted += OnProcessStarted;
            ProcessRunner.Events.ProcessFinished += OnProcessFinished;
        }

        /// <summary>
        /// Unsubscribes from process events.
        /// </summary>
        protected virtual void UnsubscribeFromProcessEvents()
        {
            ProcessRunner.Events.ProcessStarted -= OnProcessStarted;
            ProcessRunner.Events.ProcessFinished -= OnProcessFinished;
        }

        /// <summary>
        /// Is called when the process started event is triggered.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="eventArgs">Event arguments.</param>
        protected virtual void OnProcessStarted(object sender, ProcessEventArgs eventArgs)
        {
            // Show the skip step button instead of the start button.
            skipStepPicker.gameObject.SetActive(true);
            startProcessButton.gameObject.SetActive(false);

            // Disable button as you have to reset scene before starting the process again.
            startProcessButton.interactable = false;
        }

        /// <summary>
        /// Is called when the process finished event is triggered.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="eventArgs">Event arguments.</param>
        protected virtual void OnProcessFinished(object sender, ProcessEventArgs eventArgs)
        {
            skipStepPicker.gameObject.SetActive(false);
            startProcessButton.gameObject.SetActive(true);
        }
        #endregion

        #region Setup process-dependant UI
        private void SetupProcessDependantUI()
        {
            SetupChapterPickerOptions();
            SetupStateIndicator();
        }

        private void SetupChapterPickerOptions()
        {
            // Show all chapters of the process.
            PopulateChapterPickerOptions(0);
        }

        private void PopulateChapterPickerOptions(int startingIndex)
        {
            // Get a collection of available chapters.
            IList<IChapter> chapters = ProcessRunner.Current?.Data.Chapters;

            if (chapters != null)
            {
                // Skip finished chapters and convert the rest to a list of chapter names.
                List<string> dropdownOptions = new List<string>();

                for (int i = startingIndex; i < chapters.Count; i++)
                {
                    dropdownOptions.Add(chapters[i].Data.Name);
                }

                // Reset the chapter picker.
                chapterPicker.ClearOptions();

                // Populate it with new options.
                chapterPicker.AddOptions(dropdownOptions);

                // Reset the selected value
                chapterPicker.value = 0;

                // If there is only one option, the dropdown is currently disabled.
                SetDropDownStatus(chapterPicker);
            }
        }

        private void SetupSkipStepPickerOptions()
        {
            // Reset the skip step picker.
            skipStepPicker.ClearOptions();

            if (displayedStep == null)
            {
                return;
            }

            // Get a collection of available transitions (one per target step).
            IList<ITransition> transitions = displayedStep.Data.Transitions.Data.Transitions.ToList();

            // Create a list with all dropdown option names.
            List<string> dropdownOptions = new List<string>();

            if (transitions.Count > 0)
            {
                // Convert the transitions to a list of target step names and use them as dropdown options.
                // null as target step means "end of chapter".
                dropdownOptions = transitions.Select(transition => (transition.Data.TargetStep != null) ? transition.Data.TargetStep.Data.Name : "End of the Chapter").ToList();
            }

            // Populate it with new options.
            skipStepPicker.AddOptions(dropdownOptions);
            skipStepPickerEditorValueField?.SetValue(skipStepPicker, dropdownOptions.Count);
        }

        private void SetupStateIndicator()
        {
            if (ProcessRunner.Current == null)
            {
                return;
            }

            stateIndicator.enabled = ProcessRunner.Current.LifeCycle.Stage == Stage.Activating;

            ProcessRunner.Events.ProcessStarted += (sender, args) => { stateIndicator.enabled = true; };
            ProcessRunner.Events.ProcessFinished += (sender, args) => { stateIndicator.enabled = false; };
        }

        private void SetDropDownStatus(TMP_Dropdown dropdown)
        {
            if (dropdown.options.Count <= 1)
            {
                ColorBlock colorBlock = dropdown.colors;
                colorBlock.normalColor = Color.gray;
                dropdown.colors = colorBlock;
                dropdown.enabled = false;
            }
        }
        #endregion
    }
}
