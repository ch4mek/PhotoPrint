import { Typography } from '@mms/mms-ui-library/components/Typography';
import { Skeleton } from '@mui/material';

import { SKELETON_WIDTH } from '../../constants';
import { ListItemButtonWrapper, TaskDescription } from '../styles';

import { DATE_FORMAT } from '@/constants/date';
import { minutesToHours } from '@/helpers/minutesToHours';
import dayjs from '@/lib/dayjs';

import {
	DateTitleContainer,
	DurationContainer,
	DurationWrapper,
	TaskDescriptionContainer,
	TaskTableRowItemContainer,
} from './styles';
import { TaskTableRowItemProps } from './types';

export function TaskTableRowItem({
	taskName,
	trackedInMinutes,
	reportForDate,
	isDisplayFact,
	showSkeleton,
}: TaskTableRowItemProps) {
	return (
		<ListItemButtonWrapper>
			<TaskTableRowItemContainer isDisplayFact={isDisplayFact}>
				<DateTitleContainer>
					{showSkeleton ? (
						<Skeleton animation="wave" width={SKELETON_WIDTH} />
					) : (
						<Typography variant="m-400">
							{dayjs(reportForDate).format(DATE_FORMAT)}
						</Typography>
					)}
				</DateTitleContainer>
				<TaskDescriptionContainer>
					{showSkeleton ? (
						<Skeleton animation="wave" width="100%" />
					) : (
						<TaskDescription>{taskName}</TaskDescription>
					)}
				</TaskDescriptionContainer>
				<DurationWrapper>
					<DurationContainer>
						{showSkeleton ? (
							<Skeleton animation="wave" width={SKELETON_WIDTH} />
						) : (
							<Typography variant="m-400">
								{minutesToHours(trackedInMinutes)}
							</Typography>
						)}
					</DurationContainer>
				</DurationWrapper>
			</TaskTableRowItemContainer>
		</ListItemButtonWrapper>
	);
}
