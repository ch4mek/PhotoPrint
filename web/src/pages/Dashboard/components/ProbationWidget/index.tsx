import { useTranslation } from 'react-i18next';

import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { DASHBOARD_WIDGET_HEIGHT } from '@/constants/dashboard';
import { useDashboardFilters } from '@/pages/Dashboard/context/DashboardFiltersContext';
import { useGetProbationLookup } from '@/queries/dashboard/useGetProbationLookup';

import { ProbationGrid } from './components/ProbationGrid';

export function ProbationWidget() {
	const { selectedDepartmentId } = useDashboardFilters();
	const { t } = useTranslation('dashboard', { keyPrefix: 'probation' });
	const { data, isFetching: isLoading } = useGetProbationLookup({
		departmentId: selectedDepartmentId,
	});

	return (
		<Widget title={t('title')} height={DASHBOARD_WIDGET_HEIGHT}>
			<Loader size="small" {...{ isLoading }}>
				<ProbationGrid data={data || []} />
			</Loader>
		</Widget>
	);
}
