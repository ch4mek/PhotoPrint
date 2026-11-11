import { useTranslation } from 'react-i18next';

import { InfoInterviewStatus } from '@/components/InfoInterviewStatus';
import { Loader } from '@/components/Loader';
import { Widget } from '@/components/Widget';
import { DASHBOARD_WIDGET_HEIGHT } from '@/constants/dashboard';
import { Routes } from '@/constants/routes';
import { staffingTabsIds } from '@/constants/staffing';
import { STAFFING_TAB_INDEX_STORAGE_KEY } from '@/constants/storageKeys';
import { useAuth } from '@/context/AuthContext';
import { setItemToStorage } from '@/helpers/storage';
import { useCustomNavigate } from '@/hooks/useCustomNavigate';
import { useDashboardFilters } from '@/pages/Dashboard/context/DashboardFiltersContext';
import { useGetTodaysInterviews } from '@/queries/dashboard/useGetTodaysInterviews';

import { InterviewsGrid } from './components/PerformanceReviewGrid';

export function InterviewsWidget() {
	const { id: currentUserId } = useAuth();
	const { t } = useTranslation('dashboard', { keyPrefix: 'todaysInterviews' });
	const navigate = useCustomNavigate();

	const { selectedDepartmentId } = useDashboardFilters();
	const { data = [], isFetching: isLoading } = useGetTodaysInterviews({
		departmentId: selectedDepartmentId,
	});

	const handleRedirectToInterview = () => {
		setItemToStorage(
			`${STAFFING_TAB_INDEX_STORAGE_KEY}/${currentUserId}`,
			staffingTabsIds.interviewId
		);
		navigate({ to: Routes.staffing });
	};

	return (
		<Widget
			title={t('title')}
			height={DASHBOARD_WIDGET_HEIGHT}
			handleClick={handleRedirectToInterview}
			tooltipContent={<InfoInterviewStatus />}
		>
			<Loader size="small" {...{ isLoading }}>
				<InterviewsGrid {...{ data }} />
			</Loader>
		</Widget>
	);
}
