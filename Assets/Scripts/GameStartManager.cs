using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartManager
{
    public static GameStartData LoadGameStart(string gameStartName)
    {
        GameStartData ret = new GameStartData();
        Template template = TemplateManager.FindTemplate(gameStartName, "start");
        LocalClient.universeTemplateName = template.GetValue("start", "universe");
        LocalClient.gamestartTemplateName = gameStartName;
        ret.templateName = gameStartName;
        return ret;
    }
}
