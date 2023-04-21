using UnityEngine;

namespace Menu
{
    public class PlayerMenu : Menu<PlayerMenu>
    {
        [SerializeField] private StatsPanel statsPanel;
        [SerializeField] TabGroup tabGroup;
        [SerializeField] WorldMapTab m_worldMapTab;

        public override void OnStart()
        {
            base.OnStart();

            statsPanel.Initialize();
            m_worldMapTab.Initialize();
        }

        public override void CustomUpdate()
        {
            base.CustomUpdate();
        }

        public override void OnOpenMenu()
        {
            Time.timeScale = 0;
            GameEvents.Instance.PauseTrigger(true);
            tabGroup.ResetPages();
        }

        public override void OnReturnInput() => OnResumePressed();
        public override void OnPlayerMenuInput() => OnResumePressed();

        public void OnResumePressed()
        {
            Time.timeScale = 1;
            GameEvents.Instance.PauseTrigger(false);
            base.OnBackPressed();
        }
    }
}