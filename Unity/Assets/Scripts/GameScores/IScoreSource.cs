using UnityEngine;
using System.Collections;
using UniRx;
namespace GameScores{
	public interface IScoreSource {
		Score score { get; }
		ReactiveProperty<Score> rx_score {get;}
	}
	public interface IBerryScoreSource {
		StrawberryScore berries{ get; }
	}
	public interface IBasketScoreSource {
		BasketScore baskets{ get; }
	}
}
