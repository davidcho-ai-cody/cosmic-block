using System;
namespace CosmicBlock.Core {
 public enum RewardType { BlockRefresh,HintRecharge,GameOverRevive }
 // Future SDK adapter must report only its confirmed reward callback with a unique completion ID.
 // No provider is installed and no reward-granting method is exposed in Sprint 10.2.
 public interface IRewardedAdProvider {
  bool IsReady(RewardType type);
  void Request(RewardType type,Action<ConfirmedAdReward> onConfirmed,Action onUnavailableOrCancelled);
 }
 public sealed class ConfirmedAdReward {
  public readonly RewardType Type;public readonly string CompletionId;
  public ConfirmedAdReward(RewardType type,string completionId){Type=type;CompletionId=completionId;}
 }
}
