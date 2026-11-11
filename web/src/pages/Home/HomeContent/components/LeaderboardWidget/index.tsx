import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { useGetProgrammerDayResults } from '@/queries/home/useGetProgrammerDayResults';

import { LeaderboardCard } from './components/LeaderboardCard';
import { LeaderboardTrophyIcon } from './components/LeaderboardTrophyIcon';
import {
	LEADERBOARD_COLUMN_SIZE,
	LEADERBOARD_TITLE,
	LEADERBOARD_WIDGET_MAX_HEIGHT,
} from './constants';
import {
	LeaderboardColumn,
	LeaderboardEmptyMessage,
	LeaderboardList,
	LeaderboardTrophyIconWrapper,
} from './styles';

export function LeaderboardWidget() {
	const { data, isLoading } = useGetProgrammerDayResults();
	const leaderboardResults = data ?? [];
	const leaderboardResultsWithPlaces = leaderboardResults.map(
		(result, index) => ({
			place: index + 1,
			result,
		})
	);
	const leaderboardColumns = [
		{
			key: 'left',
			isReversed: false,
			results: leaderboardResultsWithPlaces.slice(0, LEADERBOARD_COLUMN_SIZE),
		},
		{
			key: 'right',
			isReversed: true,
			results: leaderboardResultsWithPlaces.slice(LEADERBOARD_COLUMN_SIZE),
		},
	];

	return (
		<Widget
			title={LEADERBOARD_TITLE}
			maxHeight={LEADERBOARD_WIDGET_MAX_HEIGHT}
			titleIcon={
				<LeaderboardTrophyIconWrapper>
					<LeaderboardTrophyIcon />
				</LeaderboardTrophyIconWrapper>
			}
		>
			<Loader {...{ isLoading }} size="small">
				{leaderboardResults.length === 0 ? (
					<LeaderboardEmptyMessage variant="m-400">
						Результатов пока нет
					</LeaderboardEmptyMessage>
				) : (
					<LeaderboardList>
						{leaderboardColumns.map(({ key, isReversed, results }) => (
							<LeaderboardColumn key={key} {...{ isReversed }}>
								{results.map(({ place, result }) => (
									<LeaderboardCard
										key={`${result.firstName}-${result.lastName}-${place}`}
										{...{ place, result }}
									/>
								))}
							</LeaderboardColumn>
						))}
					</LeaderboardList>
				)}
			</Loader>
		</Widget>
	);
}
