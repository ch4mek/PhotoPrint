import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { PROBATION_GRID_COLUMNS } from '../constants';
import { ProbationGridRow } from '../types';

import {
	cellRenderer,
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
	headerCellRenderer,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { GridColumnWidthsConfiguration, HandleRowClick } from '@/types/grid';

export function useGetGridColumns(
	gridColumnsWidths: GridColumnWidthsConfiguration,
	onUserClick: HandleRowClick<ProbationGridRow>,
	enableCellClick: (row: ProbationGridRow) => boolean
) {
	const { t } = useTranslation('dashboard', { keyPrefix: 'probation' });
	const columns: Array<Column<ProbationGridRow>> = [
		getColumnWithTooltipFactory({
			key: PROBATION_GRID_COLUMNS.probationFullNameKey,
			name: t('user'),
			width: getColumnWidth(gridColumnsWidths.userFullName).width,
			renderHeaderCell: (props) => headerCellRenderer(props),
			renderCell: (props) =>
				cellWithTooltipRenderer(props, {
					onCellClick: enableCellClick(props.row) ? onUserClick : undefined,
				}),
		}),
		getColumnWithTooltipFactory({
			key: PROBATION_GRID_COLUMNS.probationDatesKey,
			name: t('date'),
			renderHeaderCell: (props) => headerCellRenderer(props),
			width: getColumnWidth(gridColumnsWidths.date).width,
			renderCell: cellRenderer,
		}),
	];

	return columns;
}
