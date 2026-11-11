import { ReactNode } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import { PERFORMANCE_GRID_COLUMNS } from '../constants';
import { PerformanceReviewGridRow } from '../types';

import {
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
	headerCellRenderer,
} from '@/components/Grid';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { DatesRenderCell } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/components/datesRenderCell';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { HandleRowClick } from '@/types/performanceReview';

export function useGetGridColumns(
	gridColumnsWidths: GridColumnWidthsConfiguration,
	onUserCellClick: HandleRowClick<PerformanceReviewGridRow>,
	enableCellClick: (row: PerformanceReviewGridRow) => boolean,
	infoDateTagContent?: ReactNode
) {
	const { t } = useTranslation('dashboard', { keyPrefix: 'performanceReview' });
	const { performanceFullNameKey, performanceDatesKey } =
		PERFORMANCE_GRID_COLUMNS;
	const columns: Array<Column<PerformanceReviewGridRow>> = [
		getColumnWithTooltipFactory({
			key: performanceFullNameKey,
			name: t('user'),
			width: getColumnWidth(gridColumnsWidths.fullName).width,
			renderHeaderCell: (props) => headerCellRenderer(props),
			renderCell: (props) =>
				cellWithTooltipRenderer(props, {
					onCellClick: enableCellClick(props.row) ? onUserCellClick : undefined,
				}),
		}),
		getColumnWithTooltipFactory({
			key: performanceDatesKey,
			name: t('date'),
			renderHeaderCell: (props) =>
				headerCellRenderer({ ...props, tooltipContent: infoDateTagContent }),
			width: getColumnWidth(gridColumnsWidths.dates).width,
			renderCell: (props) => DatesRenderCell(props),
		}),
	];

	return columns;
}
