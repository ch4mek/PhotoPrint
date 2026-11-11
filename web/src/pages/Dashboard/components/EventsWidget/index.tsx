import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo } from 'react';
import { useTranslation } from 'react-i18next';

import { useDashboardFilters } from '../../context/DashboardFiltersContext';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { Widget } from '@/components/Widget';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { DASHBOARD_WIDGET_HEIGHT } from '@/constants/dashboard';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { overrideWidgetGridStyles } from '@/pages/Dashboard/utils/overrideGridStyles';
import { useGetEmployeeEvents } from '@/queries/dashboard/useGetEmployeeEvents';
import { EventsWidgetRow } from '@/types/dashboard';

import { eventsGridColumnWidths } from './constants';
import { mapEventToRow } from './helpers';
import { useEventsWidgetColumns } from './hooks/useEventsWidgetColumns';
import { EventsWidgetWrapper } from './styles';

export function EventsWidget() {
	const { selectedDepartmentId } = useDashboardFilters();
	const { t } = useTranslation('dashboard', { keyPrefix: 'events' });
	const { data, isFetching: isLoading } =
		useGetEmployeeEvents(selectedDepartmentId);

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrossLinkClick = useCallback(
		(row: EventsWidgetRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: EventsWidgetRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const columns = useEventsWidgetColumns(handleCrossLinkClick, enableCellClick);

	const rows: EventsWidgetRow[] = useMemo(() => {
		if (!data) {
			return [];
		}

		return [...data].map(mapEventToRow);
	}, [data]);

	const overrideDataGridStyles = overrideWidgetGridStyles(
		eventsGridColumnWidths
	);

	return (
		<Widget title={t('title')} height={DASHBOARD_WIDGET_HEIGHT}>
			<Loader isLoading={isLoading} size="small">
				<EventsWidgetWrapper>
					<DataGridHoverRowWrapper defineHeight={false}>
						<DataGrid
							columns={columns}
							rows={rows}
							overrideDataGridStyles={overrideDataGridStyles}
							rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
							renderers={noRowsGridFallback}
							scrollToTopOnRowsChange={false}
						/>
					</DataGridHoverRowWrapper>
				</EventsWidgetWrapper>
			</Loader>
		</Widget>
	);
}
