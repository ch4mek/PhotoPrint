import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	absenceTransactionHistoryColumns,
	absenceTransactionHistoryWidths,
} from '../constants';
import { descriptionCellRenderer } from '../renderers/descriptionCellRenderer';

import { getColumnWithTooltipFactory } from '@/components/Grid';
import { attachedDocumentCellRenderer } from '@/components/Grid/renderers/cell/attachedDocumentCellRenderer/attachedDocumentCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { AbsenceTransactionHistoryGridRow } from '@/types/absences';

export function useAbsenceTransactionHistoryColumns(
	onRequestFormOpen: (row: AbsenceTransactionHistoryGridRow) => void
) {
	const columns: Column<AbsenceTransactionHistoryGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.transactionDate,
				width: getColumnWidth(absenceTransactionHistoryWidths.transactionDate)
					.width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.inquiryTemplateName,
				width: getColumnWidth(
					absenceTransactionHistoryWidths.inquiryTemplateName
				).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.description,
				renderCell: (props) =>
					descriptionCellRenderer(props, { onRequestFormOpen }),
				width: getColumnWidth(absenceTransactionHistoryWidths.description)
					.width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.used,
				width: getColumnWidth(absenceTransactionHistoryWidths.used).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.accrued,
				width: getColumnWidth(absenceTransactionHistoryWidths.accrued).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.remainder,
				width: getColumnWidth(absenceTransactionHistoryWidths.remainder).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.documents,
				width: getColumnWidth(absenceTransactionHistoryWidths.documents).width,
				renderCell: attachedDocumentCellRenderer,
			}),
		],
		[]
	);

	return columns;
}
