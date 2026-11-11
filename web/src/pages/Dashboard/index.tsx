import { useTranslation } from 'react-i18next';

import { PageHeader } from '@/components/PageHeader';

import { DashboardContent } from './components/DashboardContent';
import { DashboardQuickFilters } from './components/DashboardQuickFilters';
import { DashboardFiltersProvider } from './context/DashboardFiltersContext';
import { DashboardPageWrapper } from './styles';

export function DashboardPage() {
	const { t } = useTranslation('dashboard');

	return (
		<DashboardPageWrapper>
			<DashboardFiltersProvider>
				<PageHeader title={t('title')} />
				<DashboardQuickFilters />
				<DashboardContent />
			</DashboardFiltersProvider>
		</DashboardPageWrapper>
	);
}
