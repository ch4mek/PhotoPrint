import {
	createContext,
	PropsWithChildren,
	useCallback,
	useContext,
	useMemo,
	useState,
} from 'react';

import { UserClaimsFiltersContextValues } from '../types';

import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

const UserClaimsFiltersContext = createContext<UserClaimsFiltersContextValues>(
	{} as UserClaimsFiltersContextValues
);

export function UserClaimsFiltersProvider({ children }: PropsWithChildren) {
	const [selectedClaim, setSelectedClaim] =
		useState<SimpleConfigurableConfigurationItem | null>(null);

	const resetFilters = useCallback(() => {
		setSelectedClaim(null);
	}, []);

	const contextValue: UserClaimsFiltersContextValues = useMemo(
		() => ({
			selectedClaim,
			setSelectedClaim,
			resetFilters,
		}),
		[selectedClaim, resetFilters]
	);

	return (
		<UserClaimsFiltersContext value={contextValue}>
			{children}
		</UserClaimsFiltersContext>
	);
}

export const useUserClaimsFilters = () => useContext(UserClaimsFiltersContext);
