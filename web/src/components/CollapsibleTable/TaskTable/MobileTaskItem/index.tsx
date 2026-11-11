import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Skeleton } from '@mui/material';

import { CollapsibleTableLabels } from '../../constants';
import { TableTitle, DurationTitleLabel } from '../../styles';
import { TaskDescription } from '../styles';
import { TaskTableRowItemProps } from '../TaskTableRowItem/types';

import { DATE_FORMAT } from '@/constants/date';
import { minutesToHours } from '@/helpers/minutesToHours';
import dayjs from '@/lib/dayjs';

import { MOBILE_ITEM_SKELETON_WIDTH } from './constants';
import { MobileTaskItemWrapper, TableTitleContainer } from './styles';

export function MobileTaskItem({
	reportForDate,
	taskName,
	trackedInMinutes,
	isDisplayFact,
	showSkeleton,
}: TaskTableRowItemProps) {
	return (
		<MobileTaskItemWrapper>
			<TableTitle variant="s-400">{CollapsibleTableLabels.DATE}</TableTitle>
			{showSkeleton ? (
				<Skeleton animation="wave" width={MOBILE_ITEM_SKELETON_WIDTH} />
			) : (
				<Typography variant="m-400">
					{dayjs(reportForDate).format(DATE_FORMAT)}
				</Typography>
			)}
			<TableTitleContainer>
				<TableTitle variant="s-400">
					{CollapsibleTableLabels.TASK_DESCRIPTION}
				</TableTitle>
				{showSkeleton ? (
					<Skeleton animation="wave" width={MOBILE_ITEM_SKELETON_WIDTH} />
				) : (
					<TaskDescription>{taskName}</TaskDescription>
				)}
			</TableTitleContainer>
			<TableTitleContainer>
				<TableTitle variant="s-400">
					{CollapsibleTableLabels.DURATION}

					{isDisplayFact && (
						<DurationTitleLabel>
							{CollapsibleTableLabels.FACT}
						</DurationTitleLabel>
					)}
				</TableTitle>
				{showSkeleton ? (
					<Skeleton animation="wave" width={MOBILE_ITEM_SKELETON_WIDTH} />
				) : (
					<Typography variant="m-400">
						{minutesToHours(trackedInMinutes)}
					</Typography>
				)}
			</TableTitleContainer>
		</MobileTaskItemWrapper>
	);
}
