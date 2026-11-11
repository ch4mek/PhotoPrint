import { useCallback, useMemo } from 'react';

import { CLAIM_NAMES_EXCLUSIONS } from '../../PermissionItem/constants';
import { GROUP_NAME_FIELD, initialClaimsFilterValue } from '../constants';

import { ALL_ID } from '@/constants/filters';
import { useUserClaimsFilters } from '@/pages/Settings/components/ClaimsTabs/UserClaimsTab/context/UserClaimsFiltersContext';
import { splitValueByCapitalLetters } from '@/pages/Settings/utils';
import { useGetClaimsGroups } from '@/queries/useGetClaimsGroups';
import { SimpleConfigurableConfigurationItem } from '@/types/configuration';

export const useClaimsFiltersController = () => {
	const { selectedClaim, setSelectedClaim, resetFilters } =
		useUserClaimsFilters();

	const { data: groupClaims = [] } = useGetClaimsGroups();

	const claimsOptions: SimpleConfigurableConfigurationItem[] = useMemo(
		() => [
			initialClaimsFilterValue,
			...groupClaims.flatMap(({ name: groupName, claims }) =>
				claims.map(({ name }) => ({
					id: name,
					name:
						name in CLAIM_NAMES_EXCLUSIONS
							? CLAIM_NAMES_EXCLUSIONS[name]
							: splitValueByCapitalLetters(name),

					[GROUP_NAME_FIELD]: groupName,
				}))
			),
		],
		[groupClaims]
	);

	const selectedClaimOption = useMemo(() => {
		if (selectedClaim === null) {
			return initialClaimsFilterValue;
		}

		return (
			claimsOptions.find((item) => item.name === selectedClaim.name) ||
			initialClaimsFilterValue
		);
	}, [selectedClaim, claimsOptions]);

	const handleClaimChange = useCallback(
		(item: SimpleConfigurableConfigurationItem | null) => {
			if (!item || item.id === ALL_ID) {
				setSelectedClaim(null);

				return;
			}
			setSelectedClaim(item);
		},
		[]
	);

	const isResetDisabled = selectedClaim === null;

	return {
		claimsOptions,
		selectedClaimOption,
		handleClaimChange,
		resetFilters,
		isResetDisabled,
	};
};
