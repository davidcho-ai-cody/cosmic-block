using System;
using System.Collections.Generic;
using UnityEngine;
namespace CosmicBlock.Core {
 [Serializable] public readonly struct JourneyMilestone { public readonly int Score; public readonly string Name; public JourneyMilestone(int score,string name){Score=score;Name=name;} }
 public readonly struct JourneyStatus { public readonly JourneyMilestone Current; public readonly JourneyMilestone? Next; public readonly float Progress; public JourneyStatus(JourneyMilestone current,JourneyMilestone? next,float progress){Current=current;Next=next;Progress=progress;} }
 public static class JourneyProgress {
  private static readonly JourneyMilestone[] milestones={new JourneyMilestone(0,"START"),new JourneyMilestone(1000,"STAR FIELD"),new JourneyMilestone(3000,"MOON"),new JourneyMilestone(6000,"SATURN"),new JourneyMilestone(10000,"DEEP SPACE")};
  public static IReadOnlyList<JourneyMilestone> Milestones=>milestones;
  public static JourneyStatus At(int score){score=Mathf.Max(0,score);int index=0;for(int i=1;i<milestones.Length&&score>=milestones[i].Score;i++)index=i;if(index==milestones.Length-1)return new JourneyStatus(milestones[index],null,1);var current=milestones[index];var next=milestones[index+1];return new JourneyStatus(current,next,Mathf.InverseLerp(current.Score,next.Score,score));}
 }
}
