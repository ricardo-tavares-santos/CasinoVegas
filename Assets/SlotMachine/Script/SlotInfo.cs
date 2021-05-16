﻿using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

namespace CSFramework {
	/// <summary>
	/// A class that contains information of a slot.
	/// </summary>
	[Serializable]
	public class GameInfo {
		private CustomSlot slot;
		public int roundsCompleted = 0;
		public int freeSpins = 0;
		public int bonuses = 0;
		private static int lastbalance { get { return DataManager.Instance.Coins; } }
		public int balance=lastbalance;
//		public int balance=DataManager.Instance.Coins;
		public int bet = 1;
		public int roundBalance;
		public int roundHits;
		public int totalHits;
		public List<HitInfo> scatterHitInfos;
		public virtual int roundCost { get { return slot.currentMode.costPerLine*slot.gameInfo.bet*slot.lineManager.activeLines; } }
		public GameInfo(CustomSlot slot) { this.slot = slot; }

		internal void OnStartSpin() {
			roundHits = 0;
			roundBalance = 0;
			AddBalance(-roundCost);
		}

		internal void OnStartRound() {
			scatterHitInfos = new List<HitInfo>();
			foreach (Symbol symbol in slot.symbolManager.symbols) if (symbol.matchType == Symbol.MatchType.Scatter) scatterHitInfos.Add(new HitInfo(slot, null, symbol));
		}

		internal void OnRoundComplete() {
			roundsCompleted++;
			if (bonuses > 0 && slot.currentMode == slot.modes.bonusMode) slot.AddBonus(-1);
			else if (freeSpins > 0 && slot.currentMode == slot.modes.freeSpinMode) slot.AddFreeSpin(-1);
		}

		public void AddBalance(int amount, HitInfo info = null) {
			roundBalance += amount;
			balance += amount;
			slot.callbacks.onAddBalance.Invoke(new BalanceInfo(amount, info));
			DataManager.Instance.AddCoins (amount);
		}
		public void AddHit() {
			roundHits++;
			totalHits++;
		}
	}

	[Serializable]
	public class ReelInfo : UnityEvent<ReelInfo> {
		public Reel reel;
		public bool isFirstReel { get { return reel.index == 0; } }
		public bool isLastReel { get { return reel.index == reel.slot.reels.Length - 1; } }
		public ReelInfo() { }
		public ReelInfo(Reel reel) { this.reel = reel; }
	}

	[Serializable]
	public class SlotModeInfo : UnityEvent<SlotModeInfo> {
		public SlotMode lastMode;
		public SlotModeInfo() { }

		public SlotModeInfo(SlotMode lastMode) { this.lastMode = lastMode; }
	}

	[Serializable]
	public class BalanceInfo : UnityEvent<BalanceInfo> {
		public HitInfo hitInfo;
		public int amount;
		public BalanceInfo() { }

		public BalanceInfo(int amount, HitInfo hitInfo = null) {
			this.hitInfo = hitInfo;
			this.amount = amount;
		}
	}

	[Serializable]
	public class LineInfo : UnityEvent<LineInfo> {
		public Line line;
		public bool isLineEnabled { get { return line && line.isLineEnabled; } }

		public LineInfo() { }

		public LineInfo(Line line) { this.line = line; }
	}

	/// <summary>
	/// A class that contains line's information.
	/// Will be rest once Hit Check starts.
	/// </summary>
	[Serializable]
	public class HitInfo : UnityEvent<HitInfo> {
		/// <summary>
		/// A list of all SymbolHolders Hit Check traced.
		/// </summary>
		public SymbolHolder[] holders;

		/// <summary>
		/// A list of SymbolHolders which were actually Hit.
		/// </summary>
		public List<SymbolHolder> hitHolders = new List<SymbolHolder>();

		/// <summary>
		/// DOTween sequence that will be played if the line was a hit.
		/// You can Join/Append your own Tween to how the sequence is played. 
		/// </summary>
		public Sequence sequence;

		public Line line;
		public Symbol hitSymbol;
		public int hitChains;
		public int payout;

		private CustomSlot slot;
		private bool isProcessed;
		public bool isLineEnabled { get { return line && line.isLineEnabled; } }
		public bool isHit {
			get {
				if (!hitSymbol || hitSymbol.minChains == -1) return false;
				if (line && !isLineEnabled) return false;
				return hitChains >= hitSymbol.minChains;
			}
		}
		public HitInfo() { }

		public HitInfo(CustomSlot slot, Line line = null, Symbol symbol = null) {
			this.slot = slot;
			this.line = line;
			this.hitSymbol = symbol;
		}

		public bool ProcessHitCheck() {
			if (isProcessed) return false;
			isProcessed = true;

			if (line) {
				holders = line.GetHoldersOnPath();
				if (holders == null || holders.Length != slot.reels.Length) return false;
				ParseChains(holders);
			} else {
				List<SymbolHolder> list = slot.GetVisibleHolders();
				foreach (SymbolHolder holder in list) {
					if (holder.symbol == hitSymbol && hitSymbol.matchType == Symbol.MatchType.Scatter) {
						hitChains++;
						hitHolders.Add(holder);
					}
				}
				holders = hitHolders.ToArray();
			}

			if (isHit) slot.ProcessHit(this);
			return isHit;
		}

		internal void Reset() { hitChains = 0; }

		internal void ParseChains(SymbolHolder[] refHolders) {
			Symbol[] symbols = new Symbol[slot.reels.Length];
			for (int x = 0; x < slot.reels.Length; x++) symbols[x] = holders[x].symbol;
			ParseChains(symbols, refHolders);
		}

		internal void ParseChains(Symbol[] symbols, SymbolHolder[] refHolders = null) {
			bool chainStopped = false;
			for (int i = 0; i < symbols.Length; i++) {
				Symbol symbol = symbols[i];
				if (i == 0) hitSymbol = symbol;
				if (!chainStopped && symbol.CanMatch(hitSymbol)) {
					if (hitSymbol.matchType == Symbol.MatchType.Wild && symbol.matchType != Symbol.MatchType.Wild) hitSymbol = symbol;
					hitChains++;
					if (refHolders != null) hitHolders.Add(refHolders[i]);
				} else chainStopped = true;
			}
		}
	}
}