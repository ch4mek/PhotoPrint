import { Skeleton } from '@mms/mms-ui-library/components/Skeleton';
import { Tooltip } from '@mms/mms-ui-library/components/Tooltip';
import { RenderCellProps } from 'react-data-grid';

import {
	STATIC_CROSS_LINK_CELL_CLASS,
	TOOLTIP_FALLBACK_PLACEMENTS,
} from '@/components/Grid/constants';
import { CellWrapper } from '@/components/Grid/renderers/styles';
import { zIndexes } from '@/constants/zIndexes';
import {
	AbsenceTransactionHistoryGridRow,
	AbsenceTransactionHistoryType,
} from '@/types/absences';

interface DescriptionCellRendererOptions {
	onRequestFormOpen: (row: AbsenceTransactionHistoryGridRow) => void;
}

export const descriptionCellRenderer = (
	{ row }: RenderCellProps<AbsenceTransactionHistoryGridRow, unknown>,
	{ onRequestFormOpen }: DescriptionCellRendererOptions
) => {
	const { showSkeleton, description, correctionDescription, disabled } = row;
	const isLoading = showSkeleton;
	const isRequest = row.transactionType === AbsenceTransactionHistoryType.Using;
	const isCorrection = !!row.correctionDescription;

	const tooltipText = isCorrection ? correctionDescription : description;

	const handleClick = () => {
		if (isRequest && !disabled) {
			onRequestFormOpen(row);
		}
	};

	const isClickAllowed = isRequest && !disabled;

	if (isLoading) {
		return <Skeleton visible />;
	}

	return (
		<Tooltip
			zIndex={zIndexes.gridTooltip}
			text={tooltipText}
			fallbackPlacements={TOOLTIP_FALLBACK_PLACEMENTS}
			useIfTextCut={!isCorrection}
			position="bottom"
		>
			<CellWrapper
				onClick={handleClick}
				disabled={disabled}
				className={isClickAllowed ? STATIC_CROSS_LINK_CELL_CLASS : undefined}
			>
				{description}
			</CellWrapper>
		</Tooltip>
	);
};
