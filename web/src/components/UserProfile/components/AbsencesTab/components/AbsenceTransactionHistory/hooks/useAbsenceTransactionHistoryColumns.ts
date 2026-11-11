import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

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
	const { t } = useTranslation('profile', { keyPrefix: 'absence' });
	const columns: Column<AbsenceTransactionHistoryGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.transactionDate,
				name: t('date'),
				width: getColumnWidth(absenceTransactionHistoryWidths.transactionDate)
					.width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.inquiryTemplateName,
				name: t('requestType'),
				width: getColumnWidth(
					absenceTransactionHistoryWidths.inquiryTemplateName
				).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.description,
				name: t('description'),
				renderCell: (props) =>
					descriptionCellRenderer(props, { onRequestFormOpen }),
				width: getColumnWidth(absenceTransactionHistoryWidths.description)
					.width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.used,
				name: t('used'),
				width: getColumnWidth(absenceTransactionHistoryWidths.used).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.accrued,
				name: t('accrued'),
				width: getColumnWidth(absenceTransactionHistoryWidths.accrued).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.remainder,
				name: t('remainder'),
				width: getColumnWidth(absenceTransactionHistoryWidths.remainder).width,
			}),
			getColumnWithTooltipFactory({
				...absenceTransactionHistoryColumns.documents,
				name: t('documents'),
				width: getColumnWidth(absenceTransactionHistoryWidths.documents).width,
				renderCell: attachedDocumentCellRenderer,
			}),
		],
		[]
	);

	return columns;
}
