import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	REQUESTS_WIDGET_COLUMNS,
	requestsGridColumnWidths,
} from '../constants';

import {
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
} from '@/components/Grid';
import { absenceNameCellRenderer } from '@/components/Grid/renderers/cell/absenceNameCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { RequestsWidgetRow } from '@/types/dashboard';
import { HandleRowClick } from '@/types/grid';

export const useRequestsWidgetColumns = (
	onUserCellClick: HandleRowClick<RequestsWidgetRow>,
	onAbsenceCrossLinkClick: HandleRowClick<RequestsWidgetRow>,
	enableCellClick: (row: RequestsWidgetRow) => boolean
): Array<Column<RequestsWidgetRow>> => {
	const { t } = useTranslation('dashboard', { keyPrefix: 'requests' });

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: REQUESTS_WIDGET_COLUMNS.userKey,
				name: t('user'),
				width: getColumnWidth(requestsGridColumnWidths.user).width,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						onCellClick: enableCellClick(props.row)
							? onUserCellClick
							: undefined,
						isClickable: props.row.canUserBeRetrieved,
					}),
			}),
			getColumnWithTooltipFactory({
				key: REQUESTS_WIDGET_COLUMNS.absenceKey,
				name: t('requestType'),
				width: getColumnWidth(requestsGridColumnWidths.absence).width,
				renderCell: (props) =>
					absenceNameCellRenderer(props, {
						onCellClick: onAbsenceCrossLinkClick,
					}),
			}),
			getColumnWithTooltipFactory({
				key: REQUESTS_WIDGET_COLUMNS.dateFromKey,
				name: t('dateFrom'),
				width: getColumnWidth(requestsGridColumnWidths.dateFrom).width,
			}),
		],
		[onUserCellClick, onAbsenceCrossLinkClick, enableCellClick, t]
	);
};
