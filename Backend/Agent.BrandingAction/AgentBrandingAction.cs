using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using System.IO;

namespace AgentBrandingAction
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult AgentBrandingAction(Session session)
        {
            try
            {
                session.Log("Begin Agent Branding.");

                var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "Hale", "Agent");
                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                session.Log("Writing core-keys.xml...");
                using (var sw = File.CreateText(Path.Combine(basePath, "core-keys.xml")))
                {
                    session.Log(session["HALE_CORE_KEY"]);
                    sw.Write(session["HALE_CORE_KEY"]);
                }

                session.Log("Writing agent-keys.xml...");
                using (var sw = File.CreateText(Path.Combine(basePath, "agent-keys.xml")))
                {
                    session.Log(session["HALE_AGENT_KEYS"]);
                    sw.Write(session["HALE_AGENT_KEYS"]);
                }

                session.Log("Writing nemesis.yaml...");
                using (var sw = File.CreateText(Path.Combine(basePath, "nemesis.yaml")))
                {
                    session.Log(session["HALE_AGENT_NEMESIS_CONFIG"]);
                    sw.Write(session["HALE_AGENT_NEMESIS_CONFIG"]);
                }

            }
            catch (Exception e)
            {
                session.Log("Agent Branding failed:\n" + e.Message);
                return ActionResult.Failure;
            }

            session.Log("Agent Branding completed.");
            return ActionResult.Success;
        }
    }
}
