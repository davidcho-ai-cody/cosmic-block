using CosmicBlock.Core;
namespace CosmicBlock.UI
{
    // Presentation metadata, deliberately independent from progression and save rules.
    public static class PlanetCollectionData
    {
        public static readonly string[] PlanetNames=System.Array.ConvertAll(PlanetDefinitions.All,p=>p.Name);
        public static readonly string[] StageNames={PlanetRestoration.NameForStage(1),PlanetRestoration.NameForStage(2),PlanetRestoration.NameForStage(3),PlanetRestoration.NameForStage(4),PlanetRestoration.NameForStage(5)};
        public static readonly string[] StageMessages={
            "빛을 잃은 행성이\n깊은 잠에 빠져 있습니다.",
            "작은 생명이 돌아와\n행성이 깨어납니다.",
            "푸른 빛과 생명이\n행성 곳곳에 퍼집니다.",
            "생명이 다시 번성하며\n푸른 별이 빛납니다.",
            "푸른 별이 되살아났습니다.\n생명의 빛으로 가득합니다."};
        public const string DiscoveredMessage="푸른 별의 복원이 끝났습니다.\n새로운 행성 탐험을 준비 중입니다.";
        public const string FinalProgressMessage="마지막 별빛이 모이면\n푸른 별의 복원이 완성됩니다.";
        public const string LockedMessage="아직 발견되지 않은 행성입니다.";
    }
}
