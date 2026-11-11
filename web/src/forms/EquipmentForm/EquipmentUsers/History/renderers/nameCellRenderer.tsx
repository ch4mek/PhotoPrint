import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { HistoryCellWrapper } from '../styles';
import { EquipmentHistoryRow } from '../types';

import { HistoryTabs } from '@/forms/EquipmentForm/constants';
import { FullName, parseFullName } from '@/helpers/formatName';

const isFullName = (obj: any): obj is FullName =>
	typeof obj?.firstName === 'string' && typeof obj?.lastName === 'string';

export const nameCellRenderer = <T extends EquipmentHistoryRow>(
	{ row, column }: RenderCellProps<T, unknown>,
	activeTab: HistoryTabs
) => {
	if (row.showSkeleton) {
		return <Skeleton animation="wave" width="100%" />;
	}

	const cellValue = row[column.key as keyof T];
	const isFullNameCell = typeof cellValue === 'string' || isFullName(cellValue);

	if (!cellValue || !isFullNameCell) {
		return (
			<HistoryCellWrapper
				variant="s-400"
				isDeletedHistory={activeTab === HistoryTabs.DELETED}
			/>
		);
	}

	const { firstName, lastName } = parseFullName(cellValue);

	return (
		<Tooltip useIfTextCut text={`${firstName} ${lastName}`}>
			<HistoryCellWrapper
				variant="s-400"
				isDeletedHistory={activeTab === HistoryTabs.DELETED}
			>
				{firstName} {lastName}
			</HistoryCellWrapper>
		</Tooltip>
	);
};
