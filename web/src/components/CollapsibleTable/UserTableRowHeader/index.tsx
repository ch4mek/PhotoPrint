import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import Color from 'color';
import { memo } from 'react';

import { CollapsibleTableLabels } from '../constants';
import {
	ChevronButtonWrapper,
	DurationTitleLabel,
	TableContent,
	TableTitle,
} from '../styles';

import { Bullet } from '@/components/Bullet';
import { Collapse } from '@/components/Collapse';
import { zIndexes } from '@/constants/zIndexes';
import { minutesToHours } from '@/helpers/minutesToHours';

import {
	MobileUserRowInfoWrapper,
	TableListItemButton,
	UserTableEmployeeNameContainer,
	UserTableEmployeeRowBullet,
	UserTableEmployeeRowContainer,
	UserTableRowHeaderContainer,
	UserTableRowHeaderCountContainer,
	UserTableRowHeaderDuration,
	UserTableRowHeaderDurationContainer,
	UserTableRowHeaderEmployeeContainer,
	UserTableRowHeaderWrapper,
} from './styles';
import { UserTableRowHeaderProps } from './types';

function UserTableRowHeaderComponent({
	totalTrackedInMinutes,
	countOfProjects,
	userName,
	color,
	isRowOpen,
	handleSetRowOpen,
	isDisplayFact,
}: UserTableRowHeaderProps) {
	return (
		<TableListItemButton
			onClick={handleSetRowOpen}
			disabled={!totalTrackedInMinutes}
			isRowOpen={isRowOpen}
		>
			<UserTableRowHeaderWrapper>
				<UserTableRowHeaderContainer isDisplayFact={isDisplayFact}>
					<UserTableRowHeaderCountContainer>
						<TableTitle variant="s-400">
							{CollapsibleTableLabels.PROJECTS}
						</TableTitle>
						<Typography variant="m-400">{countOfProjects}</Typography>
					</UserTableRowHeaderCountContainer>
					<UserTableRowHeaderEmployeeContainer>
						<TableTitle variant="s-400">
							{CollapsibleTableLabels.EMPLOYEE}
						</TableTitle>
						<UserTableEmployeeRowContainer>
							<UserTableEmployeeRowBullet>
								<Bullet color={Color(color).toString()} />
							</UserTableEmployeeRowBullet>
							<UserTableEmployeeNameContainer>
								<Tooltip
									zIndex={zIndexes.tooltipInCollapsibleTable}
									text={userName}
									useIfTextCut
									position="bottom"
								>
									<TableContent variant="m-400">{userName}</TableContent>
								</Tooltip>
							</UserTableEmployeeNameContainer>
						</UserTableEmployeeRowContainer>
					</UserTableRowHeaderEmployeeContainer>
					<UserTableRowHeaderDurationContainer>
						<UserTableRowHeaderDuration>
							<TableTitle variant="s-400">
								{CollapsibleTableLabels.DURATION}
								{isDisplayFact && (
									<DurationTitleLabel>
										{CollapsibleTableLabels.FACT}
									</DurationTitleLabel>
								)}
							</TableTitle>
							<Typography variant="m-400">
								{minutesToHours(totalTrackedInMinutes)}
							</Typography>
						</UserTableRowHeaderDuration>
						<Collapse isOpen={isRowOpen}>
							<MobileUserRowInfoWrapper>
								<TableTitle variant="s-400">
									{CollapsibleTableLabels.PROJECTS}
								</TableTitle>
								<Typography variant="m-400">{countOfProjects}</Typography>
								<TableTitle variant="s-400">
									{CollapsibleTableLabels.DURATION}
								</TableTitle>
								<Typography variant="m-400">
									{minutesToHours(totalTrackedInMinutes)}
								</Typography>
							</MobileUserRowInfoWrapper>
						</Collapse>
						<ChevronButtonWrapper
							isActive={isRowOpen}
							size="medium"
							disabled={!totalTrackedInMinutes}
						/>
					</UserTableRowHeaderDurationContainer>
				</UserTableRowHeaderContainer>
			</UserTableRowHeaderWrapper>
		</TableListItemButton>
	);
}

export const UserTableRowHeader = memo(UserTableRowHeaderComponent);
