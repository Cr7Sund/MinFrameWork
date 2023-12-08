I
public static partial class UIKeys
{
    // bind prefab -> view Script, viewController
    public static readonly UIKey TestScenePanel = new UIKey("testscenepanel", () => new TestView(), () => new VanquishMainScenePanelVO());
}

public class UIView : IPanel, UIElement
{
    public Dict<Components> components;
    public MediatorProxy
    // LifeTime
}

public class UIContext
{
    UIView view;
    TestMediator mediator;

    // LifeTime : separate from UIView, since we do want to wait the prefab loaded
}

// Implementation
public partial interface ITestPanelUICommand
{
    void OnRankButton_OnClick();
}
// MVVM : VM = Mediator + Controller
// MVC : C = Controller

public class TestMediator : Mediator
{
    private ITestPanelUICommand _uiCmd;
    public string MiddlePositionTextText
    {
        get => _middlePositionText.text;
        set => _middlePositionText.text = value;
    }

    private IUIText _middlePositionText => MUIGetter.Get<IUIText>(this, "MiddlePositionText");


    protected override void OnInit()
    {
        base.OnInit();
        _uiCmd = (this as IUIVO).UIController as IVanquishMainScenePanelUICommand;
    }

    protected override void OnBind()
    {
        _rankButton.OnClick.AddListener(_uiCmd.OnRankButton_OnClick);
    }

    protected override void OnUnbind()
    {
        _rankButton.OnClick.RemoveListener(_uiCmd.OnRankButton_OnClick);
    }
}


public class TestPanelController : MVVMUIController, ITestPanelUICommand
{

}

// Net
// ----- 

// Auto-Generated 

public class TestScoreRequest
{
}
public class TestGoldRequest
{
}
public class TestFirstSystemResp
{
}
public class TestGoldResp
{
}

public partial class TestFirstSystemIScoreService
{
    //请求分数
    void RequestScore(TestScoreRequest req);
    void RequestGodl(TestGoldRequest req)
    }

// logic code 

public class TestFirstSystemContext
{
    public int passValue;
}

public partial class TestFirstSystemIScoreService
{
    [Inject] IEventDispatcher dispatcher;

    //收到服务器发送过来的分数
    void OnReceiveScore(TestFirstSystemResp msg, object context)
    {
        if (context is TestFirstSystemContext) { }
        var attrInfo = msg.Data.ResetAttribute;

        dispatcher.Dispatch(Demo1Event.RequestScore_Service, attrInfo);
    }
    void OnReceiveScore(TestFirstSystemResp msg, object context) { }



}