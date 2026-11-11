import { useTranslation } from 'react-i18next';

import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { DASHBOARD_WIDGET_HEIGHT } from '@/constants/dashboard';
import { useDashboardFilters } from '@/pages/Dashboard/context/DashboardFiltersContext';
import { useGetApprovedAbsences } from '@/queries/dashboard/useGetApprovedAbsences';

import { TodayAbsencesGrid } from './components/TodayAbsencesGrid';

export function TodayAbsencesWidget() {
	const { selectedDepartmentId } = useDashboardFilters();
	const { t } = useTranslation('dashboard', { keyPrefix: 'todaysAbsences' });
	const { data = [], isFetching: isLoading } = useGetApprovedAbsences({
		departmentId: selectedDepartmentId,
	});

	return (
		<Widget title={t('title')} height={DASHBOARD_WIDGET_HEIGHT}>
			<Loader size="small" {...{ isLoading }}>
				<TodayAbsencesGrid data={data} />
			</Loader>
		</Widget>
	);
}
