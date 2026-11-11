import { useTranslation } from 'react-i18next';

import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { DASHBOARD_WIDGET_HEIGHT } from '@/constants/dashboard';
import { Routes } from '@/constants/routes';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { useDashboardFilters } from '@/pages/Dashboard/context/DashboardFiltersContext';
import { useGetPerformanceReviewLookup } from '@/queries/dashboard/useGetPerformanceReviewLookup';

import { PerformanceReviewGrid } from './components/PerformanceReviewGrid';

export function PerformanceReviewWidget() {
	const navigate = useCustomNavigate();
	const { selectedDepartmentId } = useDashboardFilters();
	const { t } = useTranslation('dashboard', { keyPrefix: 'performanceReview' });
	const { data, isFetching: isLoading } = useGetPerformanceReviewLookup({
		departmentId: selectedDepartmentId,
	});

	const handleRedirectToPerformanceReviewPage = () => {
		navigate({
			to: `${Routes.performanceReview}`,
		});
	};

	return (
		<Widget
			title={t('title')}
			height={DASHBOARD_WIDGET_HEIGHT}
			handleClick={handleRedirectToPerformanceReviewPage}
		>
			<Loader size="small" {...{ isLoading }}>
				<PerformanceReviewGrid data={data || []} />
			</Loader>
		</Widget>
	);
}
