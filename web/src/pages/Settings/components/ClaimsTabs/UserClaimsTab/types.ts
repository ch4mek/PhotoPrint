import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export interface UserClaimsFiltersContextValues {
	selectedClaim: SimpleConfigurableConfigurationItem | null;
	setSelectedClaim: (name: SimpleConfigurableConfigurationItem | null) => void;
	resetFilters: () => void;
}
