import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';
import { CrossIcon } from '@mms/mms-ui-library/icons/base/CrossIcon';
import { Skeleton } from '@mui/material';
import { RenderCellProps } from 'react-data-grid';

import { EquipmentHistoryRow } from '../types';

import { GridCellDefaultProps } from '@/components/Grid';
import { HistoryTabs } from '@/forms/EquipmentForm/constants';
import { checkBrokenStatus } from '@/forms/EquipmentForm/utils/checkBrokenStatus';

export const deleteCellRenderer = <T extends GridCellDefaultProps>(
	{ row }: RenderCellProps<T, unknown>,
	activeTab: HistoryTabs,
	handleDelete: (equipmentHistoryId: number) => void,
	status: string
) => {
	const { returnDate, equipmentHistoryId } =
		row as unknown as EquipmentHistoryRow;

	const isBrokenStatus = checkBrokenStatus(status);

	const isReturnDateExist = !!returnDate;
	const isHistoryTabActive = activeTab === HistoryTabs.HISTORY;
	const isDeleteIconVisible =
		isHistoryTabActive && !isReturnDateExist && !isBrokenStatus;

	const onDeleteClick = () => {
		handleDelete(equipmentHistoryId);
	};

	if (!isDeleteIconVisible) {
		return null;
	}

	if (row.showSkeleton) {
		return <Skeleton animation="wave" width="100%" />;
	}

	return (
		<ActionButton onClick={onDeleteClick} size="small">
			<CrossIcon />
		</ActionButton>
	);
};
