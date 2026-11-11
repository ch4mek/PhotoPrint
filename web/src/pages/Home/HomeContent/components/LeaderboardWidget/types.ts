import { ProgrammerDayResult } from '@/queries/home/types';

export type LeaderboardCardProps = {
	place: number;
	result: ProgrammerDayResult;
};

export type LeaderboardHighlightProps = {
	isTopResult: boolean;
};

export type LeaderboardColumnProps = {
	isReversed: boolean;
};
