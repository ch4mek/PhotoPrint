import { PageWrapper } from '@/pages/styles';

import { UserClaimsFilters } from './components/UserClaimsFilters';
import { UserClaimsGrid } from './components/UserClaimsGrid';
import { UserClaimsFiltersProvider } from './context/UserClaimsFiltersContext';

export function UserClaimsTab() {
	return (
		<PageWrapper>
			<UserClaimsFiltersProvider>
				<UserClaimsFilters />
				<UserClaimsGrid />
			</UserClaimsFiltersProvider>
		</PageWrapper>
	);
}
