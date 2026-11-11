import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { INTERVIEW_GRID_COLUMNS } from '../constants';
import { InterviewsGridRow } from '../types';

import {
	cellRenderer,
	cellWithTooltipRenderer,
	getColumnFactory,
	getColumnWithTooltipFactory,
	headerCellRenderer,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { GridColumnWidthsConfiguration, HandleRowClick } from '@/types/grid';

export function useGetGridColumns(
	gridColumnsWidths: GridColumnWidthsConfiguration,
	onUserCellClick: HandleRowClick<InterviewsGridRow>,
	enableCellClick: (row: InterviewsGridRow) => boolean
) {
	const { t } = useTranslation('dashboard', { keyPrefix: 'todaysInterviews' });
	const { interviewSpecializationKey, interviewFullNameKey, interviewTimeKey } =
		INTERVIEW_GRID_COLUMNS;
	const columns: Array<Column<InterviewsGridRow>> = [
		getColumnWithTooltipFactory({
			key: interviewSpecializationKey,
			name: t('specialization'),
			width: getColumnWidth(gridColumnsWidths.specialization).width,
			renderHeaderCell: headerCellRenderer,
			renderCell: cellWithTooltipRenderer,
		}),
		getColumnWithTooltipFactory({
			key: interviewFullNameKey,
			name: t('user'),
			width: getColumnWidth(gridColumnsWidths.fullName).width,
			renderHeaderCell: headerCellRenderer,
			renderCell: (props) =>
				cellWithTooltipRenderer(props, {
					onCellClick: enableCellClick(props.row) ? onUserCellClick : undefined,
				}),
		}),
		getColumnFactory({
			key: interviewTimeKey,
			name: t('time'),
			renderHeaderCell: headerCellRenderer,
			width: getColumnWidth(gridColumnsWidths.time).width,
			renderCell: cellRenderer,
		}),
	];

	return columns;
}
