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
import { Routes } from '@/constants/routes';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { overrideWidgetGridStyles } from '@/pages/Dashboard/utils/overrideGridStyles';
import { useGetRequestsWidget } from '@/queries/dashboard/useGetRequestsWidget';
import { RequestsWidgetRow } from '@/types/dashboard';

import { requestsGridColumnWidths } from './constants';
import { mapRequestToRow } from './helpers';
import { useRequestsWidgetColumns } from './hooks/useRequestsWidgetColumns';
import { RequestsWidgetWrapper } from './styles';

export function RequestsWidget() {
	const navigate = useCustomNavigate();
	const { selectedDepartmentId } = useDashboardFilters();
	const { t } = useTranslation('dashboard', { keyPrefix: 'requests' });
	const { data, isFetching: isLoading } =
		useGetRequestsWidget(selectedDepartmentId);

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handleUserPageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);
	const handleInquiryPageOpen = useOpenPopUpViaUrl(
		PopUpSearchParams.InquiryIdParam
	);

	const handleCrossLinkClick = useCallback(
		(row: RequestsWidgetRow) => {
			handleUserPageOpen(row.userId.toString());
		},
		[handleUserPageOpen]
	);

	const enableCellClick = useCallback(
		(row: RequestsWidgetRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const handleAbsenceCrossLinkClick = useCallback(
		(row: RequestsWidgetRow) => {
			handleInquiryPageOpen(row.inquiryId.toString());
		},
		[handleInquiryPageOpen]
	);

	const columns = useRequestsWidgetColumns(
		handleCrossLinkClick,
		handleAbsenceCrossLinkClick,
		enableCellClick
	);

	const rows: RequestsWidgetRow[] = useMemo(() => {
		if (!data) {
			return [];
		}

		return data.map(mapRequestToRow);
	}, [data]);

	const overrideDataGridStyles = overrideWidgetGridStyles(
		requestsGridColumnWidths
	);

	const handleSeeAll = () => {
		navigate({ to: `${Routes.requests}` });
	};

	const dataGridProps = {
		columns,
		rows,
		overrideDataGridStyles,
		rowHeight: DATA_GRID_ROW_HEIGHT_WITH_TAG,
		renderers: noRowsGridFallback,
		scrollToTopOnRowsChange: false,
	};

	return (
		<Widget
			title={t('title')}
			height={DASHBOARD_WIDGET_HEIGHT}
			handleClick={handleSeeAll}
		>
			<Loader isLoading={isLoading} size="small">
				<RequestsWidgetWrapper>
					<DataGridHoverRowWrapper defineHeight={false}>
						<DataGrid {...dataGridProps} />
					</DataGridHoverRowWrapper>
				</RequestsWidgetWrapper>
			</Loader>
		</Widget>
	);
}
