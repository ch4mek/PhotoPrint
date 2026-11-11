import { useTheme } from '@emotion/react';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { Skeleton } from '@mui/material';
import { memo } from 'react';

import {
	CollapsibleTableLabels,
	FULL_SKELETON_WIDTH,
	SKELETON_WIDTH,
} from '../constants';
import {
	ChevronButtonWrapper,
	DurationTitleLabel,
	TableContent,
	TableTitle,
} from '../styles';

import { Divider } from '@/components/Divider';
import { zIndexes } from '@/constants/zIndexes';
import { minutesToHours } from '@/helpers/minutesToHours';

import {
	TaskTitleContainer,
	TaskListHeaderContainer,
	TaskListHeaderWrapper,
	ProjectTitleWrapper,
	ProjectTitleContainer,
	ProjectTitle,
	DurationTitleWrapper,
	DurationTitleContainer,
	TableListItemButton,
} from './styles';
import { PartialTasksListHeaderProps } from './types';

const { tabletXS } = breakpoints;

function TaskListHeaderComponent({
	tasksCount,
	projectTitle,
	totalTrackedInMinutes,
	handleSetRowOpen,
	isRowOpen = false,
	isDisplayFact,
	showSkeleton,
	isLastItem = true,
}: PartialTasksListHeaderProps) {
	const theme = useTheme();
	const isTabletXSWidth = useHasScreenWidth(tabletXS);

	const isLastItemBorderRounded = isRowOpen || !isLastItem;

	return (
		<>
			<Divider margin={`0 ${theme.spaces.s}px 0 ${theme.spaces.xl}px`} />
			<TableListItemButton
				isLastItemBorderRounded={isLastItemBorderRounded}
				onClick={handleSetRowOpen}
			>
				<TaskListHeaderWrapper>
					<TaskListHeaderContainer isDisplayFact={isDisplayFact}>
						<TaskTitleContainer>
							<TableTitle variant="s-400">
								{CollapsibleTableLabels.TASKS}
							</TableTitle>
							{showSkeleton ? (
								<Skeleton
									width={isTabletXSWidth ? SKELETON_WIDTH : FULL_SKELETON_WIDTH}
									animation="wave"
								/>
							) : (
								<Typography variant="m-400">{tasksCount}</Typography>
							)}
						</TaskTitleContainer>
						<ProjectTitleWrapper>
							<TableTitle variant="s-400">
								{CollapsibleTableLabels.PROJECT_TITLE}
							</TableTitle>
							{showSkeleton ? (
								<Skeleton
									animation="wave"
									width={
										isTabletXSWidth
											? `calc(100% - ${theme.spaces.xl3}px)`
											: FULL_SKELETON_WIDTH
									}
								/>
							) : (
								<ProjectTitleContainer>
									<ProjectTitle>
										<Tooltip
											zIndex={zIndexes.tooltipInCollapsibleTable}
											text={projectTitle}
											useIfTextCut
											position="bottom"
										>
											<TableContent variant="m-400">
												{projectTitle}
											</TableContent>
										</Tooltip>
									</ProjectTitle>
								</ProjectTitleContainer>
							)}
						</ProjectTitleWrapper>
						<DurationTitleWrapper>
							<DurationTitleContainer>
								<TableTitle variant="s-400">
									{CollapsibleTableLabels.DURATION}
									{isDisplayFact && (
										<DurationTitleLabel>
											{CollapsibleTableLabels.FACT}
										</DurationTitleLabel>
									)}
								</TableTitle>
								{showSkeleton ? (
									<Skeleton
										width={isTabletXSWidth ? SKELETON_WIDTH : '100%'}
										animation="wave"
									/>
								) : (
									<Typography variant="m-400">
										{minutesToHours(totalTrackedInMinutes)}
									</Typography>
								)}
							</DurationTitleContainer>
						</DurationTitleWrapper>
						<ChevronButtonWrapper isActive={isRowOpen} size="medium" />
					</TaskListHeaderContainer>
				</TaskListHeaderWrapper>
			</TableListItemButton>
		</>
	);
}

export const TaskListHeader = memo(TaskListHeaderComponent);
