import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { CollapsibleTableLabels } from '../constants';
import { DurationTitleLabel } from '../styles';

import { Divider } from '@/components/Divider';

import { MobileTaskItem } from './MobileTaskItem';
import {
	MobileTableItem,
	TaskTableList,
	TaskTableRowContainer,
	TaskTableRowDurationTitle,
	TaskTableRowWrapper,
} from './styles';
import { TaskTableRowItem } from './TaskTableRowItem';
import { TaskTableRowProps } from './types';

export function TaskTableRow({
	data,
	isDisplayFact,
	isLoading,
	isTaskRowHeader,
}: TaskTableRowProps) {
	const theme = useTheme();

	return (
		<TaskTableList>
			<Divider margin={`0 ${theme.spaces.s}px 0 ${theme.spaces.xl3}px `} />
			<TaskTableRowWrapper>
				<TaskTableRowContainer
					isTaskRowHeader={isTaskRowHeader}
					isDisplayFact={isDisplayFact}
				>
					<Typography variant="s-400">{CollapsibleTableLabels.DATE}</Typography>
					<Typography variant="s-400">
						{CollapsibleTableLabels.TASK_DESCRIPTION}
					</Typography>
					<TaskTableRowDurationTitle>
						<Typography variant="s-400">
							{CollapsibleTableLabels.DURATION}
							{isDisplayFact && (
								<DurationTitleLabel>
									{CollapsibleTableLabels.FACT}
								</DurationTitleLabel>
							)}
						</Typography>
					</TaskTableRowDurationTitle>
				</TaskTableRowContainer>
			</TaskTableRowWrapper>
			{isLoading ? (
				<>
					<TaskTableRowItem isDisplayFact={isDisplayFact} showSkeleton />
					<MobileTableItem>
						<MobileTaskItem isDisplayFact={isDisplayFact} showSkeleton />
					</MobileTableItem>
				</>
			) : (
				<>
					{data.map((item, index) => (
						<TaskTableRowItem
							key={index}
							{...item}
							isDisplayFact={isDisplayFact}
						/>
					))}
					<MobileTableItem>
						{data?.map((task, index) => (
							<MobileTaskItem
								{...task}
								key={index}
								isDisplayFact={isDisplayFact}
							/>
						))}
					</MobileTableItem>
				</>
			)}
		</TaskTableList>
	);
}
