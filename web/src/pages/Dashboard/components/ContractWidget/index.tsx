import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback } from 'react';
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
import { useGetExpirationContracts } from '@/queries/dashboard/useGetExpirationContracts';

import { useContractExpirationGridState } from './hooks/useContractExpirationGridState';
import { ContractWidgetWrapper } from './styles';
import { ContractExpirationGridRow } from './types';

export function ContractWidget() {
	const { selectedDepartmentId } = useDashboardFilters();
	const { t } = useTranslation('dashboard', {
		keyPrefix: 'contractExpiration',
	});
	const { data, isFetching: isLoading } =
		useGetExpirationContracts(selectedDepartmentId);

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrossLinkClick = useCallback(
		(row: ContractExpirationGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: ContractExpirationGridRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const { columns, rows, overrideDataGridStyles } =
		useContractExpirationGridState(
			data || [],
			handleCrossLinkClick,
			enableCellClick
		);

	return (
		<Widget title={t('title')} height={DASHBOARD_WIDGET_HEIGHT}>
			<Loader size="small" {...{ isLoading }}>
				<ContractWidgetWrapper>
					<DataGridHoverRowWrapper defineHeight={false}>
						<DataGrid
							{...{ columns, rows, overrideDataGridStyles }}
							rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
							renderers={noRowsGridFallback}
							scrollToTopOnRowsChange={false}
						/>
					</DataGridHoverRowWrapper>
				</ContractWidgetWrapper>
			</Loader>
		</Widget>
	);
}
