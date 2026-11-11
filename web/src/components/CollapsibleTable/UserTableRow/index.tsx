import { UserTableRowHeader } from '../UserTableRowHeader';

import { Collapse } from '@/components/Collapse';

import { TableRowWrapper } from './styles';
import type { UserTableRowProps } from './types';

export function UserTableRow({
	row,
	children,
	isDisplayFact,
	handleSetRowOpen,
	isRowOpen,
}: UserTableRowProps) {
	const { countOfProjects, userName, totalTrackedInMinutes, color } = row;

	return (
		<TableRowWrapper>
			<UserTableRowHeader
				totalTrackedInMinutes={totalTrackedInMinutes}
				countOfProjects={countOfProjects}
				userName={userName}
				color={color}
				handleSetRowOpen={handleSetRowOpen}
				isRowOpen={isRowOpen}
				isDisplayFact={isDisplayFact}
			/>
			<Collapse isOpen={isRowOpen}>{children}</Collapse>
		</TableRowWrapper>
	);
}
