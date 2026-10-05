namespace CosmicBlock.UI
{
    // Presentation metadata, deliberately independent from progression and save rules.
    public static class PlanetCollectionData
    {
        public static readonly string[] PlanetNames={"푸른 별","???","???","???","???"};
        public static readonly string[] StageNames={"황폐","깨어남","회복","번성","완성"};
        public static readonly string[] StageMessages={
            "빛을 잃은 행성이\n깊은 잠에 빠져 있습니다.",
            "작은 생명이 돌아와\n행성이 깨어납니다.",
            "푸른 빛과 생명이\n행성 곳곳에 퍼집니다.",
            "생명이 다시 번성하며\n푸른 별이 빛납니다.",
            "푸른 별이 되살아났습니다.\n생명의 빛으로 가득합니다."};
        public const string LockedMessage="아직 발견되지 않은 행성입니다.";
    }
}
